using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    protected static Inventory instance;
    protected CharacterWrapper cw;
    
	void Awake () {
        instance = this;
        gameObject.SetActive(false);
	}

    void Refresh()
    {
        string equipped = "Equipped:";
        for (int i = 0; i < cw.equipment.Count; i++)
        {
            equipped += "\n"+cw.equipment[i].ToStringLong();
        }
        transform.GetChild(3).GetComponent<Text>().text = equipped;
        int offset = 6;
        while (transform.childCount - offset < cw.inventory.Count)
            Instantiate(transform.GetChild(offset).gameObject, transform);
        for (int i = 0; i < cw.inventory.Count; i++)
        {
            var e = cw.inventory[i];
            Transform t = transform.GetChild(i + offset);
            t.GetComponent<Text>().text = e.ToStringLong();
            var o = t.GetChild(0).GetComponent<Button>().onClick;
            o.RemoveAllListeners();
            o.AddListener(() => {
                cw.RemoveInventory(e);
                cw.AddEquipment(e);
				Refresh();
            });
            o = t.GetChild(1).GetComponent<Button>().onClick;
            o.RemoveAllListeners();
            o.AddListener(() => { cw.RemoveInventory(e); Refresh(); });
            t.gameObject.SetActive(true);
        }
        for (int i = cw.inventory.Count+offset; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public static void Show(CharacterWrapper cw)
    {
        instance.cw = cw;
        instance.Refresh();
        instance.gameObject.SetActive(true);
    }

    public static void Close()
    {
        instance.gameObject.SetActive(false);
    }
}
