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
    public Equipment baseWeapon;
    public List<Equipment> equipment;
    [Header("AI")]
    [Range(0f, 1f)]
    public float aggressive = 0.5f;
    [Range(0f, 1f)]
    public float defensive = 0.5f;
    [Range(0f, 1f)]
    public float utility = 0.2f;
    public int strengthUpgrade = 2;
    public int agilityUpgrade = 2;
    public int constitutionUpgrade = 2;
    public int intelligenceUpgrade = 2;
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
    public int stage;

    public CharacterWrapper(Character character)
    {
        this.character = character;
        stage = 0;
        equipment = new List<EquipmentWrapper>();
        inventory = new List<EquipmentWrapper>();
        for (int i = 0; i < character.equipment.Count; i++)
        {
            equipment.Add(new EquipmentWrapper(character.equipment[i]));
        }
        CalculateStats();
		health = constitution;
	}

    public void CalculateStats()
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
        strength += character.strengthUpgrade*stage;
        agility += character.agilityUpgrade*stage;
        constitution += character.constitutionUpgrade*stage;
        intelligence += character.intelligenceUpgrade*stage;
		health = Mathf.Min(health, constitution);
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
        health = Mathf.Min(health, constitution);
        if (e.equipment.slot == Equipment.Slots.oneHand)
        {
            EquipmentWrapper old = null;
            for (int i = 0; i < equipment.Count - 1; i++)
            {
                if (equipment[i].equipment.slot == Equipment.Slots.bothHands)
                {
                    RemoveEquipment(equipment[i], e);
                    break;
                }
                else if (equipment[i].equipment.slot == Equipment.Slots.oneHand)
                {
                    if (old == null)
                        old = equipment[i];
                    else
                    {
                        RemoveEquipment(old, e);
                        break;
                    }
                }
            }
        }
        else if (e.equipment.slot == Equipment.Slots.bothHands)
        {
            bool second = false;
            for (int i = 0; i < equipment.Count - 1; i++)
            {
                if (equipment[i].equipment.slot == Equipment.Slots.bothHands || equipment[i].equipment.slot == Equipment.Slots.oneHand)
                {
                    if (second)
                    {
                        RemoveEquipment(equipment[i]);
                        AddInventory(equipment[i]);
                    }
                    else
                    {
                        second = true;
                        RemoveEquipment(equipment[i], e);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < equipment.Count - 1; i++)
            {
                if (equipment[i].equipment.slot == e.equipment.slot)
                {
                    RemoveEquipment(equipment[i], e);
                    break;
                }
            }
        }
    }

    public void RemoveEquipment(EquipmentWrapper e, EquipmentWrapper equip=null)
    {
        if (equipment.Remove(e))
        {
            strength -= e.equipment.strength;
            agility -= e.equipment.agility;
            constitution -= e.equipment.constitution;
            intelligence -= e.equipment.intelligence;

            if (e.durability > 0 && this == GameState.State.player && equip != null)
            {
                SwapPopup.Popup(e.ToString(), (b) => {
                    if (b)
                    {
                        AddInventory(e);
                        GameState.State.GetComponent<BattleManager>().EquipEquipment(equip);
                    }
                    else
                        GameState.State.GetComponent<BattleManager>().ThrowEquipment(e);
                });
            }

            if (e.equipment.type == Equipment.Type.weapon && character.baseWeapon != null)
            {
                bool hasWeapon = false;
                for (int i = 0; i < equipment.Count; i++)
                {
                    if (equipment[i].equipment.type == Equipment.Type.weapon)
                    {
                        hasWeapon = true;
                        break;
                    }
                }
                if (!hasWeapon)
                {
                    AddEquipment(character.baseWeapon);
                }
            }
            health = Mathf.Min(health, constitution);
        }
    }

    public void AddInventory(EquipmentWrapper e)
    {
        inventory.Add(e);
        strength--;
        agility--;
        constitution--;
        intelligence--;
        health = Mathf.Min(health, constitution);
        if (character.name == "Player")
            FlashText.Flash("All Stats: -1", Color.red);
    }

    public void RemoveInventory(EquipmentWrapper e)
    {
        if (inventory.Remove(e))
        {
            strength++;
            agility++;
            constitution++;
            intelligence++;
            if (character.name == "Player")
                FlashText.Flash("All Stats: +1", Color.green);
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
        CalculateStats();
		health = constitution;
	}

    public void NextStage()
    {
        stage++;
		CalculateStats();
		health = constitution;
	}

    public EquipmentWrapper GetAbility(out Equipment.Ability ability)
    {
        float sum = character.aggressive + character.defensive + character.utility;
        if (sum < 0.01f)
        {
            ability = new Equipment.Ability();
            ability.name = "Wait";
            ability.action = Equipment.Action.none;
            return null;
        }
        float rnd = Random.value*sum;
        EquipmentWrapper e = null;
        if (rnd < character.aggressive)
        {
            for (int i = 0; i < equipment.Count; i++)
            {
                if(equipment[i].equipment.type == Equipment.Type.aggressive || equipment[i].equipment.type == Equipment.Type.weapon && equipment[i].cooldown <= 0)
                {
                    e = equipment[i];
                    int r = Random.Range(0, equipment.Count);
                    equipment[i] = equipment[r];
                    equipment[r] = e;
                    break;
                }
            }
            if (e == null)
            {
                float tmp = character.aggressive;
                character.aggressive = 0f;
                e = GetAbility(out ability);
                character.aggressive = tmp;
                return e;
            }
        }
        else
        {
            rnd -= character.aggressive;
            if (rnd < character.defensive)
            {
                for (int i = 0; i < equipment.Count; i++)
                {
                    if (equipment[i].equipment.type == Equipment.Type.defensive && equipment[i].cooldown <= 0)
                    {
                        e = equipment[i];
                        int r = Random.Range(0, equipment.Count);
                        equipment[i] = equipment[r];
                        equipment[r] = e;
                        break;
                    }
                }
                if (e == null)
                {
                    float tmp = character.defensive;
                    character.defensive = 0f;
                    e = GetAbility(out ability);
                    character.defensive = tmp;
                    return e;
                }
            }
            else
            {
                for (int i = 0; i < equipment.Count; i++)
                {
                    if (equipment[i].equipment.type == Equipment.Type.utility && equipment[i].cooldown <= 0)
                    {
                        e = equipment[i];
                        int r = Random.Range(0, equipment.Count);
                        equipment[i] = equipment[r];
                        equipment[r] = e;
                        break;
                    }
                }
                if (e == null)
                {
                    float tmp = character.utility;
                    character.utility = 0f;
                    e = GetAbility(out ability);
                    character.utility = tmp;
                    return e;
                }
            }
        }
        if (!e.NextAction(out ability))
        {
           return e;
        }
        return null;
    }
}
