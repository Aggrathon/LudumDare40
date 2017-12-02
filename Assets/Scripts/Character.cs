using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="SO/Enemy")]
public class Character : ScriptableObject {

    public int strength = 20;
    public int agility = 20;
    public int constitution = 20;
    public int intelligence = 20;
    [Space]
    public List<Equipment> equipment;
}

[System.Serializable]
public class CharacterWrapper
{
    public Character character;
    public List<EquipmentWrapper> equipment;
    public List<EquipmentWrapper> inventory;
    public int strength;
    public int agility;
    public int constitution;
    public int intelligence;
    public int health;

    public CharacterWrapper(Character character)
    {
        this.character = character;
        equipment = new List<EquipmentWrapper>();
        inventory = new List<EquipmentWrapper>();
        for (int i = 0; i < character.equipment.Count; i++)
        {
            equipment.Add(new EquipmentWrapper(character.equipment[i]));
        }
        CalculateStats();
        health = constitution;
    }

    void CalculateStats()
    {
        strength = character.strength;
        agility = character.agility;
        constitution = character.constitution;
        intelligence = character.intelligence;
        for (int i = 0; i < equipment.Count; i++)
        {
            strength += equipment[i].equipment.strength;
            agility += equipment[i].equipment.agility;
            constitution += equipment[i].equipment.constitution;
            intelligence += equipment[i].equipment.intelligence;
        }

        strength -= inventory.Count;
        agility -= inventory.Count;
        constitution -= inventory.Count;
        intelligence -= inventory.Count;
    }

    public void AddEquipment(Equipment e)
    {
        AddEquipment(new EquipmentWrapper(e));
    }

    public void AddEquipment(EquipmentWrapper e)
    {
        equipment.Add(e);
        e.NextMatch();
        strength += e.equipment.strength;
        agility += e.equipment.agility;
        constitution += e.equipment.constitution;
        intelligence += e.equipment.intelligence;
    }

    public void RemoveEquipment(EquipmentWrapper e)
    {
        if (equipment.Remove(e))
        {
            strength -= e.equipment.strength;
            agility -= e.equipment.agility;
            constitution -= e.equipment.constitution;
            intelligence -= e.equipment.intelligence;
        }
    }

    public void AddInventory(EquipmentWrapper e)
    {
        inventory.Add(e);
        strength--;
        agility--;
        constitution--;
        intelligence--;
    }

    public void RemoveInventory(EquipmentWrapper e)
    {
        if (inventory.Remove(e))
        {
            strength++;
            agility++;
            constitution++;
            intelligence++;
        }
    }

    public void NextTurn()
    {
        for (int i = 0; i < equipment.Count; i++)
        {
            equipment[i].NextTurn();
        }
    }

    public void NextMatch()
    {
        for (int i = 0; i < equipment.Count; i++)
        {
            equipment[i].NextMatch();
        }
        health = constitution;
    }
}
