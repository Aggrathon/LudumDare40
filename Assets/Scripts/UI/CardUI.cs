﻿using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour {

    public Sprite changeIcon;
    public Sprite surrenderIcon;

    public void SetCards(CharacterWrapper character, Action<EquipmentWrapper, Equipment.Ability> onSelect)
    {
        bool hasAction = false;
        List<EquipmentWrapper> list = character.equipment;
        bool hasInventory = character.inventory.Count > 0;
        while (transform.childCount < list.Count+2)
            Instantiate(transform.GetChild(0).gameObject, transform);
        for (int i = 0; i < list.Count; i++)
        {
            EquipmentWrapper e = list[i];
            Transform t = transform.GetChild(i);
            t.GetChild(0).GetComponent<Image>().sprite = e.equipment.icon;
            t.GetChild(1).GetComponent<Text>().text = e.ActionString();
            Button b = t.GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => { onSelect(e, null); });
            b.interactable = e.cooldown <= 0;
            hasAction = hasAction | b.interactable;
            t.gameObject.SetActive(true);
        }
        for (int i = list.Count; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
        if (hasInventory)
        {
			Transform t = transform.GetChild(list.Count);
			t.GetChild(0).GetComponent<Image>().sprite = changeIcon;
			Button b = t.GetComponent<Button>();
			b.onClick.RemoveAllListeners();
            t.GetChild(1).GetComponent<Text>().text = "Swap Equipment";
            b.onClick.AddListener(() => { onSelect(null, null); });
			b.interactable = true;
			t.gameObject.SetActive(true);
		}
        if (!hasAction)
		{
			Transform t = transform.GetChild(list.Count+1);
			t.GetChild(0).GetComponent<Image>().sprite = changeIcon;
			Button b = t.GetComponent<Button>();
			b.onClick.RemoveAllListeners();
			Equipment.Ability playerAction = new Equipment.Ability();
            playerAction.action = Equipment.Action.none;
            playerAction.name = "Wait";
            t.GetChild(1).GetComponent<Text>().text = "Wait";
            b.onClick.AddListener(() => { onSelect(null, playerAction); });
			b.interactable = true;
			t.gameObject.SetActive(true);
		}
    }

    public void Clear()
    {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }
}
