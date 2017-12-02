using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public Button[] colorButtons;

    private void Start()
    {
        for (int i = 0; i < colorButtons.Length; i++)
        {
            int j = i;
            colorButtons[i].interactable = true;
            colorButtons[i].onClick.RemoveAllListeners();
            colorButtons[i].onClick.AddListener(() => { SetColor(j); });
        }
        colorButtons[0].interactable = false;
    }

    void SetColor(int j)
    {
        for (int i = 0; i < colorButtons.Length; i++)
        {
            colorButtons[i].interactable = true;
        }
        colorButtons[j].interactable = false;
    }

    public void Play()
    {
        for (int i = 0; i < colorButtons.Length; i++)
        {
            if (!colorButtons[i].interactable)
            {
                GameState.playerColor = colorButtons[i].transform.GetChild(0).GetComponent<Image>().color;
                break;
            }
        }
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
