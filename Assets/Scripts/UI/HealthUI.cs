using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour {

    public void SetHealth(int amount)
    {
        while (amount > transform.childCount)
            Instantiate(transform.GetChild(0).gameObject, transform);
        for (int i = 0; i < amount; i++)
            transform.GetChild(i).gameObject.SetActive(true);
        for (int i = amount; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }
}
