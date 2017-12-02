using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {

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
            //TODO AI vs AI
        }
    }

    public void PlayerBattle(CharacterWrapper player, CharacterWrapper enemy)
    {
        this.player = player;
        this.enemy = enemy;
        enemyName.text = enemy.character.name;
        RefreshStatus();
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
        playerCards.SetCards(player.equipment, player.inventory.Count > 0, SetPlayerAction);
    }

    void SetPlayerAction(EquipmentWrapper e)
    {
        if (e == null)
        {
            //TODO swap equipment
            FlashText.Flash("Not implemented yet!", Color.red);
            RefreshStatus();
        }
        else
        {
            FlashText.Flash("Not implemented yet!", Color.red);
            //TODO do turn
            if (player.health < 0 && enemy.health >= player.health)
            {
                FlashText.Flash("You Lost!", Color.red);
            }
            else if (enemy.health < 0 && player.health > enemy.health)
            {
                FlashText.Flash("You Won!", Color.green);
            }
            player.NextTurn();
            enemy.NextTurn();
            RefreshStatus();
        }
    }
}
