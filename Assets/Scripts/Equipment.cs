using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="SO/Equipment")]
public class Equipment : ScriptableObject {

    public enum Slots
    {
        head,
        body,
        oneHand,
        bothHands
    }

    public enum Type
    {
        weapon,
        aggressive,
        defensive,
        utility
    }

    public enum Action
    {
        none,
        damage,
        damageStrength,
        damageAgility,
        block
    }

    public Sprite icon;
    public Slots slot;
    public Type type;
    public Ability[] actions;
    public int durability = 20;
    [Space]
    public int strength;
    public int agility;
    public int constitution;
    public int intelligence;

    [System.Serializable]
    public struct Ability
    {
        public int cooldown;
        public string name;
        public Action action;
        public float amount;
    }
}

[System.Serializable]
public class EquipmentWrapper
{
    public Equipment equipment;
    public int durability;
    public int action;
    public int cooldown;

    public EquipmentWrapper(Equipment equipment)
    {
        if (equipment == null)
        {
            Debug.LogError("Equipment Should not be null");
        }
        this.equipment = equipment;
        durability = equipment.durability;
        action = 0;
        cooldown = 0;
    }

    public void NextMatch()
    {
        action = 0;
        cooldown = 0;
    }

    public void NextTurn()
    {
        cooldown--;
    }

    public string ActionString()
    {
        return equipment.actions[action].name + " (" + durability + "/" + equipment.durability + ")";
    }

    override public string ToString()
    {
        return equipment.name + " (" + durability + "/" + equipment.durability + ")";
    }

    public bool NextAction(out Equipment.Ability ac)
    {
        ac = equipment.actions[action];
        cooldown = equipment.actions[action].cooldown+1;
        action = (action + 1) % equipment.actions.Length;
        if (ac.action != Equipment.Action.none)
            durability--;
        return durability > 0;
    }
}
