using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwapPopup : MonoBehaviour {

    protected static SwapPopup instance;

    Action<bool> onSelect;
    
	void Awake () {
        instance = this;
        gameObject.SetActive(false);
	}

    public void Throw()
    {
        gameObject.SetActive(false);
        onSelect(false);
    }

    public void Keep()
    {
        gameObject.SetActive(false);
        onSelect(true);
    }


    public static void Popup(string item, Action<bool> callback)
    {
        instance.transform.GetChild(0).GetComponent<Text>().text = item;
        instance.onSelect = callback;
        instance.gameObject.SetActive(true);
        Inventory.Close();
    }

    public static void Close()
    {
        instance.gameObject.SetActive(false);
    }

}
