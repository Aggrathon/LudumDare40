﻿using System.Collections;
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
        if (playerOne.character.name == "Player")
            PlayerBattle(playerOne, playerTwo);
        else if (playerTwo.character.name == "Player")
            PlayerBattle(playerTwo, playerOne);
        else
        {
            StartCoroutine(AIvsAI(playerOne, playerTwo));
        }
    }

    public void CloseBattleUI()
    {
        battleUI.SetActive(false);
        playerFightButton.interactable = false;
    }

    public void OpenBattleUI()
    {
        RefreshStatus();
        FlashText.Flash("Fight!", Color.red);
        battleUI.SetActive(true);
        GameState.state.tournament.gameObject.SetActive(false);
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
                GameState.state.DefeatCharacter(playerOne);
                yield break;
            }
            else if (playerTwo.health <= 0 && playerOne.health > playerTwo.health)
            {
                GameState.state.DefeatCharacter(playerTwo);
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
        RefreshStatus();
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

    void SetPlayerAction(EquipmentWrapper e)
    {
        if (e == null && player.inventory.Count >0)
        {
            //TODO swap equipment
            FlashText.Flash("Not implemented yet!", Color.red);
            RefreshStatus();
        }
        else
        {
            Equipment.Ability playerAction;
            bool remove = false;
            if (e != null)
                remove = !e.NextAction(out playerAction);
            else
            {
                playerAction = new Equipment.Ability();
                playerAction.action = Equipment.Action.none;
                playerAction.name = "Wait";
            }
            Equipment.Ability enemyAction;
            var ee = enemy.GetAbility(out enemyAction);
            FlashText.Flash(enemyAction.name + "\n\n\n" + playerAction.name, Color.white);
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
                GameState.state.DefeatCharacter(player);
                return;
            }
            else if (enemy.health <= 0 && player.health > enemy.health)
            {
                FlashText.Flash("You Won!", Color.green);
                GameState.state.DefeatCharacter(enemy);
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
                    float damage = p1.strength * a1.amount;
                    DoDamage(p2, damage);
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
                    float damage = p1.strength * a1.amount;
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
                int d = Mathf.RoundToInt(amount * 0.5f);
                c.health -= d;
                e.durability -= d;
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
