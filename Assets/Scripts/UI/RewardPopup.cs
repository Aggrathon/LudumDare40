using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardPopup : MonoBehaviour {

	protected static RewardPopup instance;

	public Text matchReward;
	public Text audienceReward;
	public Text total;

	CharacterWrapper looser;

	private void Awake()
	{
		instance = this;
		gameObject.SetActive(false);
	}

	public static void Show(CharacterWrapper looser, int audRew, int matRew)
	{
		instance.audienceReward.text = "" + audRew;
		instance.matchReward.text = "" + matRew;
		instance.total.text = "Total: " + (audRew + matRew) + " Denarius";
		GameState.State.money += matRew + audRew;
		instance.looser = looser;
		instance.gameObject.SetActive(true);
	}

	public void Loot()
	{
		gameObject.SetActive(false);
		var list = looser.equipment;
		list.RemoveAll(m => m.durability < 2);
		if (list.Count == 0)
		{
			Lootbox.OpenCheapSmall();
		}
		else
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i].durability -= 2;
			}
			Lootbox.Open(list, Mathf.Min(list.Count, 2));
		}
	}
}
