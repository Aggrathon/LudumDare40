using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="SO/Enemy")]
public class Character : ScriptableObject {

	public Sprite look;
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
	public List<Equipment> upgrades;
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
		if (e == null)
			return;
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
        if (e.equipment.slot == Equipment.Slots.oneHand)
        {
            EquipmentWrapper old = null;
            for (int i = 0; i < equipment.Count - 1; i++)
            {
                if (equipment[i].equipment.slot == Equipment.Slots.bothHands)
                {
                    RemoveEquipment(equipment[i]);
                    break;
                }
                else if (equipment[i].equipment.slot == Equipment.Slots.oneHand)
                {
                    if (old == null)
                        old = equipment[i];
                    else
                    {
                        RemoveEquipment(old);
						break;
                    }
                }
            }
        }
        else if (e.equipment.slot == Equipment.Slots.bothHands)
		{
			bool hasRemoved = false;
			for (int i = 0; i < equipment.Count - 1; i++)
            {
                if (equipment[i].equipment.slot == Equipment.Slots.bothHands || equipment[i].equipment.slot == Equipment.Slots.oneHand)
                {
                    if (hasRemoved)
					{
						if (this == GameState.State.player)
							AddInventory(equipment[i]);
						RemoveEquipment(equipment[i]);
						i--;
                    }
                    else
                    {
                        RemoveEquipment(equipment[i]);
						i--;
						hasRemoved = true;
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
                    RemoveEquipment(equipment[i]);
					break;
                }
            }
        }
		if (this == GameState.State.player)
			GameState.State.battleManager.RefreshStatus();
    }

    public void RemoveEquipment(EquipmentWrapper e)
    {
        if (equipment.Remove(e))
        {
            strength -= e.equipment.strength;
            agility -= e.equipment.agility;
            constitution -= e.equipment.constitution;
            intelligence -= e.equipment.intelligence;

            if (e.durability > 0 && this == GameState.State.player)
            {
                SwapPopup.Popup(e.ToString(), (b) => {
					if (b)
					{
						AddInventory(e);
					}
					else
					{
						GameState.State.battleManager.ThrowEquipment(e);
					}
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
        }
    }

    public void AddInventory(EquipmentWrapper e)
    {
        inventory.Add(e);
        strength--;
        agility--;
        constitution--;
        intelligence--;
        if (this == GameState.State.player)
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
            if (this == GameState.State.player)
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
		for (int i = 0; i < inventory.Count; i++)
		{
			inventory[i].NextMatch();
		}
        CalculateStats();
		health = constitution;
	}

    public void NextStage()
    {
        stage++;
		int len = character.upgrades.Count / 2;
		for (int i = len*(stage-1); i < Mathf.Min(len*stage, character.upgrades.Count); i++)
		{
			AddEquipment(character.upgrades[i]);
		}
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
                if((equipment[i].equipment.type == Equipment.Type.aggressive || equipment[i].equipment.type == Equipment.Type.weapon) && equipment[i].cooldown <= 0)
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
                    if ((equipment[i].equipment.type == Equipment.Type.armor || equipment[i].equipment.type == Equipment.Type.shield) && equipment[i].cooldown <= 0)
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
