using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupPlayer : MonoBehaviour {

    public Equipment[] inventory;
    
	void Start () {
        for (int i = 0; i < GameState.State.league.Count; i++)
        {
            var c = GameState.State.league[i];
            if (c.character.name == "Player")
            {
                for (int j = 0; j < inventory.Length; j++)
                {
                    c.inventory.Add(new EquipmentWrapper(inventory[j]));
                }
                c.CalculateStats();
            }
        }
	}
}
