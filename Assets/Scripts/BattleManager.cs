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
	[Space]
	public CombatAnimation playerAnimation;
	public CombatAnimation enemyAnimation;

    CharacterWrapper player;
    CharacterWrapper enemy;

	int reward = 0;

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
		playerAnimation.gameObject.SetActive(false);
		enemyAnimation.gameObject.SetActive(false);
	}

    public void OpenBattleUI()
    {
        GameState.State.tournament.gameObject.SetActive(false);
        SwapPopup.Close();
        Inventory.Close();
		Shop.Close();
		playerAnimation.gameObject.SetActive(true);
		enemyAnimation.gameObject.SetActive(true);
		StartCoroutine(BattleStart());
    }

    IEnumerator BattleStart()
    {
        yield return new WaitForSeconds(0.1f);
        RefreshStatus();
        FlashText.Flash("Fight!", Color.red);
        yield return new WaitForSeconds(0.1f);
        battleUI.SetActive(true);
		GameState.State.tournament.gameObject.SetActive(false);
		SwapPopup.Close();
		Inventory.Close();
		Shop.Close();
	}

    IEnumerator BattleOver(CharacterWrapper looser)
	{
		int mr = GameState.State.tournament.currentStageNum * (GameState.State.tournament.currentStageNum +1) * 50;
		int ar = reward;
		yield return new WaitForSeconds(0.1f);
		CloseBattleUI();
		GameState.State.DefeatCharacter(looser);
		yield return new WaitForSeconds(0.1f);
		if (looser == player)
		{
			LostPopup.Show();
		}
		else
		{
			RewardPopup.Show(looser, ar, mr);
		}
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
            HandleAction(playerOne, playerTwo, ref oneAction, ref twoAction);
            HandleAction(playerTwo, playerOne, ref twoAction, ref oneAction);
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
		reward = 7 + player.inventory.Count*5;
		playerAnimation.SetLook(player.character.look);
		enemyAnimation.SetLook(enemy.character.look);
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
		reward += 20;
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
            playerCards.Clear();
            Equipment.Ability playerAction;
            bool remove = false;
            if (action != null)
                playerAction = action;
            else
                remove = !e.NextAction(out playerAction);
            Equipment.Ability enemyAction;
            var ee = enemy.GetAbility(out enemyAction);
            FlashText.Flash(enemyAction.name + "\n\n\n\n" + playerAction.name, Color.white);
            HandleAction(player, enemy, ref playerAction, ref enemyAction, playerAnimation, enemyAnimation);
            HandleAction(enemy, player, ref enemyAction, ref playerAction, enemyAnimation, playerAnimation);
            if (remove)
            {
                FlashText.Flash("Your "+e.equipment.name.ToLower()+" broke!", Color.red);
                player.RemoveEquipment(e);
            }
            if (ee != null)
                enemy.RemoveEquipment(ee);
            if (player.health <= 0 && enemy.health >= player.health)
            {
                StartCoroutine(BattleOver(player));
                return;
            }
            else if (enemy.health <= 0 && player.health > enemy.health)
            {
                StartCoroutine(BattleOver(enemy));
                return;
            }
			reward += 3;
            StartCoroutine(DelayedNextTurn());
        }
    }

    IEnumerator DelayedNextTurn()
    {
        yield return new WaitForSeconds(0.4f);
        player.NextTurn();
        enemy.NextTurn();
        RefreshStatus();
    }

    void HandleAction(CharacterWrapper p1, CharacterWrapper p2, ref Equipment.Ability a1, ref Equipment.Ability a2, CombatAnimation ca1 = null, CombatAnimation ca2 = null)
    {
        switch (a1.action)
        {
            case Equipment.Action.none:
                break;
            case Equipment.Action.damage:
                if(a2.action != Equipment.Action.block)
                {
                    DoDamage(p2, a1.amount);
					if (ca1 != null)
					{
						ca1.PlayAttack();
						ca2.PlayDamage();
					}
				}
				else if (p2 == player)
				{
					reward += 15;
				}
				break;
            case Equipment.Action.damageStrength:
                if (a2.action != Equipment.Action.block)
                {
                    float damage = p1.strength * a1.amount;
                    DoDamage(p2, damage);
					if (ca1 != null)
					{
						ca1.PlayAttack();
						ca2.PlayDamage();
					}
				}
				else if (p2 == player)
				{
					reward += 15;
				}
				break;
            case Equipment.Action.damageAgility:
                if (a2.action != Equipment.Action.block)
                {
                    float damage = p1.agility * a1.amount;
                    DoDamage(p2, damage);
					if (ca1 != null)
					{
						ca1.PlayAttack();
						ca2.PlayDamage();
					}
				}
				else if (p2 == player)
				{
					reward += 15;
				}
                break;
            case Equipment.Action.block:
				if (ca1 != null)
				{
					ca1.PlayBlock();
				}
				break;
			case Equipment.Action.cheer:
				if (ca1 != null)
				{
					ca1.PlayUtility();
				}
				if (p1 == player)
				{
					reward += 30;
				}
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
            if (e.equipment.type == Equipment.Type.armor)
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
