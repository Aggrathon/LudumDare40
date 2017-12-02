using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Equipment")]
public class Equipment : ScriptableObject {

    public enum Slots
    {
        head,
        body,
        rightHand,
        leftHand,
        bothHands
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
