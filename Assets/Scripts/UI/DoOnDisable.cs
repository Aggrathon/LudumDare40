using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoOnDisable : MonoBehaviour {

	public GameObject activate;
	public GameObject deactivate;

	public bool isStartTutorial;

	private void Start()
	{
		if (isStartTutorial)
		{
			if (GameState.tutorial)
			{
				GameState.tutorial = false;
			}
			else
			{
				activate = null;
				deactivate = null;
				gameObject.SetActive(false);
			}
		}
	}

	private void OnDisable()
	{
		if (activate != null)
			activate.SetActive(true);
		if (deactivate != null)
			deactivate.SetActive(false);
	}
}
