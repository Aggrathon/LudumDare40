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
    public int strength;
    public int agility;
    public int constitution;
    public int intelligence;

    public CharacterWrapper(Character character)
    {
        this.character = character;
        equipment = new List<EquipmentWrapper>();
        for (int i = 0; i < character.equipment.Count; i++)
        {
            equipment.Add(new EquipmentWrapper(character.equipment[i]));
        }
    }

    void CalculateStats()
    {

    }
}
