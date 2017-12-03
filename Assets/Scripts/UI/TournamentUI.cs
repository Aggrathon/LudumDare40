using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TournamentUI : MonoBehaviour {

    public Transform[] stages;

    [System.NonSerialized] public List<CharacterWrapper> currentStage;
    [System.NonSerialized] public int currentStageNum = 0;

    private void Awake()
    {
        currentStageNum = 0;
    }

    public void SetStage(List<CharacterWrapper> stage)
    {
        currentStage = new List<CharacterWrapper>(stage);
        int len = Mathf.Min(currentStage.Count, stages[currentStageNum].childCount - 1);
        for (int i = 0; i < len; i++)
        {
            stages[currentStageNum].GetChild(i + 1).GetChild(0).GetComponent<Text>().text = stage[i].character.name;
            if(stage[i] == GameState.State.player)
            {
                stages[currentStageNum].GetChild(i + 1).GetComponent<Image>().color = GameState.playerColor;
            }
        }
        currentStageNum++;
    }

    public void DefeatCombatant(CharacterWrapper character)
    {
        int i = currentStage.IndexOf(character);
        if (i>= 0)
        {
            stages[currentStageNum - 1].GetChild(i + 1).GetComponent<Image>().color = Color.red;
        }
    }

    public bool IsTournamentOver { get { return currentStageNum >= stages.Length; } }

    public void Quit()
    {
        SceneManager.LoadScene(0);
	}

	public void Shop()
	{
		Lootbox.OpenCheapSmall();
	}

	public void ShowInventory()
	{
		Inventory.Show(GameState.State.player);
	}

}
