using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lootbox : MonoBehaviour {

	protected static Lootbox instance;

	List<EquipmentWrapper> content;
	int num;

	public Equipment[] cheapLoot;
	public Equipment[] expensiveLoot;
	public Text description;

	private void Awake()
	{
		instance = this;
		gameObject.SetActive(false);
	}

	void Refresh()
	{
		description.text = "Select " + num;
		while(transform.childCount < content.Count + 2)
			Instantiate(transform.GetChild(2).gameObject, transform);
		for (int i = 0; i < content.Count; i++)
		{
			var e = content[i];
			Transform t = transform.GetChild(i + 2);
			t.GetChild(0).GetComponent<Text>().text = e.ToStringLong();
			Button b = t.GetComponent<Button>();
			b.onClick.RemoveAllListeners();
			b.onClick.AddListener(() => {
				GameState.State.player.AddInventory(e);
				content.Remove(e);
				num--;
				if (num > 0)
					Refresh();
				else
				{
					gameObject.SetActive(false);
					if (!GameState.State.battleManager.battleUI.activeSelf)
						GameState.State.tournament.gameObject.SetActive(true);
				}
			});
			t.gameObject.SetActive(true);
		}
		for (int i = content.Count+2; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
		gameObject.SetActive(true);
	}

	public static void Open(List<EquipmentWrapper> list, int num=2)
	{
		instance.content = list;
		instance.num = num;
		instance.Refresh();
	}

	public static void OpenCheapSmall()
	{
		instance.content = new List<EquipmentWrapper>();
		instance.num = 2;
		for (int i = 0; i < 2; i++)
		{
			var e = new EquipmentWrapper(instance.cheapLoot[Random.Range(0, instance.cheapLoot.Length)]);
			e.durability = (Random.Range(1, e.durability - 1) + Random.Range(1, e.durability - 1)) / 2;
			instance.content.Add(e);
		}
		instance.Refresh();
	}

	public static void OpenCheapLarge()
	{
		instance.content = new List<EquipmentWrapper>();
		instance.num = 3;
		for (int i = 0; i < 5; i++)
		{
			var e = new EquipmentWrapper(instance.cheapLoot[Random.Range(0, instance.cheapLoot.Length)]);
			e.durability = (Random.Range(1, e.durability - 1) + Random.Range(1, e.durability - 1)) / 2;
			instance.content.Add(e);
		}
		instance.Refresh();
	}

	public static void OpenExpensiveSmall()
	{
		instance.content = new List<EquipmentWrapper>();
		instance.num = 1;
		for (int i = 0; i < 2; i++)
		{
			var e = new EquipmentWrapper(instance.expensiveLoot[Random.Range(0, instance.expensiveLoot.Length)]);
			e.durability = (Random.Range(1, e.durability - 1) + Random.Range(1, e.durability - 1)) / 2;
			instance.content.Add(e);
		}
		instance.Refresh();
	}

	public static void OpenExpensiveLarge()
	{
		instance.content = new List<EquipmentWrapper>();
		instance.num = 2;
		for (int i = 0; i < 4; i++)
		{
			var e = new EquipmentWrapper(instance.expensiveLoot[Random.Range(0, instance.expensiveLoot.Length)]);
			e.durability = (Random.Range(1, e.durability - 1) + Random.Range(1, e.durability - 1)) / 2;
			instance.content.Add(e);
		}
		instance.Refresh();
	}
	
}
