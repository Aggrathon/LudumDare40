using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LostPopup : MonoBehaviour
{

	protected static LostPopup instance;

	private void Awake()
	{
		instance = this;
		gameObject.SetActive(false);
	}

	public static void Show()
	{
		GameState.State.tournament.gameObject.SetActive(true);
		instance.gameObject.SetActive(true);
	}

	public void Reset()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
