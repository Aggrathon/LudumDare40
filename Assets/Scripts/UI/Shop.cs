using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

	protected static Shop instance;

	public Text money;

	private void Awake()
	{
		instance = this;
		gameObject.SetActive(false);
	}

	void Refresh()
	{
		money.text = "You have " + GameState.State.money + " Denarius";
		transform.GetChild(3).GetComponent<Button>().interactable = GameState.State.money >= 100;
		transform.GetChild(4).GetComponent<Button>().interactable = GameState.State.money >= 200;
		transform.GetChild(5).GetComponent<Button>().interactable = GameState.State.money >= 250;
		transform.GetChild(6).GetComponent<Button>().interactable = GameState.State.money >= 400;
		gameObject.SetActive(true);
	}

	public void OLCS()
	{
		gameObject.SetActive(false);
		GameState.State.money -= 100;
		Lootbox.OpenCheapSmall();
	}

	public void OLCL()
	{
		gameObject.SetActive(false);
		GameState.State.money -= 200;
		Lootbox.OpenCheapLarge();
	}

	public void OLES()
	{
		gameObject.SetActive(false);
		GameState.State.money -= 250;
		Lootbox.OpenExpensiveSmall();
	}

	public void OLEL()
	{
		gameObject.SetActive(false);
		GameState.State.money -= 400;
		Lootbox.OpenExpensiveLarge();
	}

	public static void Open()
	{
		instance.Refresh();
	}

	public static void Close()
	{
		instance.gameObject.SetActive(false);
	}
}
