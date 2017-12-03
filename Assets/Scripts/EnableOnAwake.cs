using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnAwake : MonoBehaviour {

	public GameObject[] list;

	void Awake () {
		for (int i = 0; i < list.Length; i++)
		{
			list[i].SetActive(true);
		}	
	}
}
