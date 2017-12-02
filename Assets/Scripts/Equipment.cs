using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="SO/Equipment")]
public class Equipment : ScriptableObject {

    public enum Slots
    {
        head,
        body,
        rightHand,
        leftHand,
        bothHands
    }

    public enum Type
    {
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
        block,
        move
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
        this.equipment = equipment;
        durability = equipment.durability;
        action = 0;
        cooldown = 0;
    }
}
