using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupPlayer : MonoBehaviour {

    public Equipment[] inventory;
    
	void Start () {
		var player = GameState.State.player;
        for (int j = 0; j < inventory.Length; j++)
        {
			player.inventory.Add(new EquipmentWrapper(inventory[j]));
        }
		player.CalculateStats();
		for (int i = 0; i < player.equipment.Count; i++)
		{
			if (player.equipment[i].equipment.type == Equipment.Type.weapon)
				player.equipment[i].durability /= 2;
		}
	}
}
