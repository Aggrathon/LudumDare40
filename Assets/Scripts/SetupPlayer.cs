using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupPlayer : MonoBehaviour {

    public Equipment[] inventory;
    
	void Start () {
        for (int i = 0; i < GameState.state.league.Count; i++)
        {
            var c = GameState.state.league[i];
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
