using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {

    public GameObject battleUI;
    public Button playerFightButton;
    public CardUI playerCards;
    public HealthUI playerHealth;
    public HealthUI enemyHealth;
    public Text enemyName;
    public Text enemyEquipment;
    public Text playerStrength;
    public Text playerAgility;
    public Text playerConstitution;
    public Text playerIntelligence;
    CharacterWrapper player;
    CharacterWrapper enemy;


    private void Start()
    {
        playerFightButton.onClick.RemoveAllListeners();
        playerFightButton.onClick.AddListener(OpenBattleUI);
    }

    public void Battle(CharacterWrapper playerOne, CharacterWrapper playerTwo)
    {
        playerOne.NextMatch();
        playerTwo.NextMatch();
        if (playerOne == GameState.State.player)
            PlayerBattle(playerOne, playerTwo);
        else if (playerTwo == GameState.State.player)
            PlayerBattle(playerTwo, playerOne);
        else
        {
            StartCoroutine(AIvsAI(playerOne, playerTwo));
        }
    }

    public void CloseBattleUI()
    {
        battleUI.SetActive(false);
        Inventory.Close();
        playerFightButton.interactable = false;
    }

    public void OpenBattleUI()
    {
        GameState.State.tournament.gameObject.SetActive(false);
        SwapPopup.Close();
        Inventory.Close();
        StartCoroutine(BattleStart());
    }

    IEnumerator BattleStart()
    {
        yield return new WaitForSeconds(0.05f);
        RefreshStatus();
        FlashText.Flash("Fight!", Color.red);
        yield return new WaitForSeconds(0.05f);
        battleUI.SetActive(true);
    }

    IEnumerator BattleOver(CharacterWrapper looser)
    {
        CloseBattleUI();
        yield return new WaitForSeconds(0.1f);
        GameState.State.DefeatCharacter(looser);
    }

    IEnumerator AIvsAI(CharacterWrapper playerOne, CharacterWrapper playerTwo)
    {
        while (true)
        {
            yield return null;
            Equipment.Ability oneAction;
            EquipmentWrapper oneEquip = playerOne.GetAbility(out oneAction);
            Equipment.Ability twoAction;
            EquipmentWrapper twoEquip = playerTwo.GetAbility(out twoAction);
            HandleAction(playerOne, playerTwo, ref oneAction, ref twoAction, false);
            HandleAction(playerTwo, playerOne, ref twoAction, ref oneAction, false);
            if (oneEquip != null)
                playerOne.RemoveEquipment(oneEquip);
            if (twoEquip != null)
                playerTwo.RemoveEquipment(twoEquip);
            if (playerOne.health <= 0 && playerTwo.health >= playerOne.health)
            {
                GameState.State.DefeatCharacter(playerOne);
                yield break;
            }
            else if (playerTwo.health <= 0 && playerOne.health > playerTwo.health)
            {
                GameState.State.DefeatCharacter(playerTwo);
                yield break;
            }
            playerOne.NextTurn();
            playerTwo.NextTurn();
        }
    }

    public void PlayerBattle(CharacterWrapper player, CharacterWrapper enemy)
    {
        this.player = player;
        this.enemy = enemy;
        enemyName.text = enemy.character.name;
        playerFightButton.interactable = true;
    }

    void RefreshStatus()
    {
        playerHealth.SetHealth(player.health);
        enemyHealth.SetHealth(enemy.health);
        string eq = "";
        for (int i = 0; i < enemy.equipment.Count; i++)
        {
            eq += enemy.equipment[i].equipment.name + "\n";
        }
        enemyEquipment.text = eq.Remove(eq.Length - 1);
        playerStrength.text = "" + player.strength;
        playerAgility.text = "" + player.agility;
        playerConstitution.text = "" + player.constitution;
        playerIntelligence.text = "" + player.intelligence;
        playerCards.SetCards(player, SetPlayerAction);
    }

    public void ThrowEquipment(EquipmentWrapper ew)
    {
        if (!battleUI.activeSelf)
            return;
        Equipment.Ability ac = new Equipment.Ability();
        ac.action = Equipment.Action.damage;
        ac.amount = ew.equipment.type == Equipment.Type.weapon ? 6 : 4;
        ac.name = "Throw " + ew.equipment.name;
        SetPlayerAction(ew, ac);
    }

    public void EquipEquipment(EquipmentWrapper e)
    {
        if (!battleUI.activeSelf)
            return;
        Equipment.Ability ac = new Equipment.Ability();
        ac.action = Equipment.Action.none;
        ac.name = "Equip " + e.equipment.name;
        SetPlayerAction(e, ac);
    }

    void SetPlayerAction(EquipmentWrapper e, Equipment.Ability action)
    {
        if (e == null && action == null)
        {
            Inventory.Show(player);
        }
        else
        {
            Equipment.Ability playerAction;
            bool remove = false;
            if (action != null)
                playerAction = action;
            else
                remove = !e.NextAction(out playerAction);
            Equipment.Ability enemyAction;
            var ee = enemy.GetAbility(out enemyAction);
            FlashText.Flash(enemyAction.name + "\n\n\n\n" + playerAction.name, Color.white);
            HandleAction(player, enemy, ref playerAction, ref enemyAction, true);
            HandleAction(enemy, player, ref enemyAction, ref playerAction, true);
            if (remove)
            {
                FlashText.Flash("Your "+e.equipment.name.ToLower()+" broke!", Color.red);
                player.RemoveEquipment(e);
            }
            if (ee != null)
                enemy.RemoveEquipment(ee);
            if (player.health <= 0 && enemy.health >= player.health)
            {
                FlashText.Flash("You Lost!", Color.red);
                StartCoroutine(BattleOver(player));
                return;
            }
            else if (enemy.health <= 0 && player.health > enemy.health)
            {
                FlashText.Flash("You Won!", Color.green);
                StartCoroutine(BattleOver(enemy));
                return;
            }
            player.NextTurn();
            enemy.NextTurn();
            RefreshStatus();
        }
    }

    void HandleAction(CharacterWrapper p1, CharacterWrapper p2, ref Equipment.Ability a1, ref Equipment.Ability a2, bool visual)
    {
        switch (a1.action)
        {
            case Equipment.Action.none:
                break;
            case Equipment.Action.damage:
                if(a2.action != Equipment.Action.block)
                {
                    DoDamage(p2, a1.amount);
                }
                break;
            case Equipment.Action.damageStrength:
                if (a2.action != Equipment.Action.block)
                {
                    float damage = p1.strength * a1.amount;
                    DoDamage(p2, damage);
                }
                break;
            case Equipment.Action.damageAgility:
                if (a2.action != Equipment.Action.block)
                {
                    float damage = p1.agility * a1.amount;
                    DoDamage(p2, damage);
                }
                break;
            case Equipment.Action.block:
                break;
            default:
                break;
        }
    }

    void DoDamage(CharacterWrapper c, float amount)
    {
        int rnd = Random.Range(0, c.equipment.Count);
        for (int i = 0; i < c.equipment.Count; i++)
        {
            var e = c.equipment[(rnd + i) % c.equipment.Count];
            if (e.equipment.type == Equipment.Type.defensive)
            {
                c.health -= Mathf.RoundToInt(amount * 0.5f);
                e.durability -= Mathf.RoundToInt(amount * 0.3f);
                if (e.durability <= 0)
                {
                    c.RemoveEquipment(e);
                    if (c == player)
                        FlashText.Flash("Your " + e.equipment.name.ToLower() + " broke!", Color.red);
                }
                return;
            }
        }
        c.health -= Mathf.RoundToInt(amount);
    }
}
