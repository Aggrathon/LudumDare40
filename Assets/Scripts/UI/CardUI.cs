using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour {

    public Sprite changeIcon;
    public Sprite surrenderIcon;

    public void SetCards(CharacterWrapper character, Action<EquipmentWrapper> onSelect)
    {
        List<EquipmentWrapper> list = character.equipment;
        bool hasInventory = character.inventory.Count > 0;
        while (transform.childCount < list.Count+1)
            Instantiate(transform.GetChild(0).gameObject, transform);
        for (int i = 0; i < list.Count; i++)
        {
            EquipmentWrapper e = list[i];
            Transform t = transform.GetChild(i);
            t.GetChild(0).GetComponent<Image>().sprite = e.equipment.icon;
            t.GetChild(1).GetComponent<Text>().text = e.ActionString();
            Button b = t.GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => { onSelect(e); });
            b.interactable = e.cooldown <= 0;
            t.gameObject.SetActive(true);
        }
        for (int i = list.Count; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
        if (hasInventory)
        {
            Transform t = transform.GetChild(list.Count);
            t.GetChild(0).GetComponent<Image>().sprite = changeIcon;
            t.GetChild(1).GetComponent<Text>().text = "Swap Equipment";
            Button b = t.GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => { onSelect(null); });
            b.interactable = true;
            t.gameObject.SetActive(true);
        }
        if (!hasInventory && list.Count == 0)
        {
            Transform t = transform.GetChild(0);
            t.GetChild(0).GetComponent<Image>().sprite = surrenderIcon;
            t.GetChild(1).GetComponent<Text>().text = "Surrender";
            Button b = t.GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => {
                FlashText.Flash("You Surrendered!", Color.red);
                GameState.state.DefeatCharacter(character);
            });
            b.interactable = true;
            t.gameObject.SetActive(true);
        }
    }
}
