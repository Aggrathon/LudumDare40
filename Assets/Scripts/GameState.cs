using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattleManager))]
public class GameState : MonoBehaviour {

    public static GameState State { get; protected set; }
    public static Color playerColor = Color.white;

	public int money = 100;
    [System.NonSerialized] public List<CharacterWrapper> league;
    [SerializeField] Character[] startingLeauge;
	[System.NonSerialized] public CharacterWrapper player;

    public TournamentUI tournament;

	[System.NonSerialized] public BattleManager battleManager;
    int matchIndex;

    void Awake () {
        State = this;
        league = new List<CharacterWrapper>();
        for (int i = 0; i < startingLeauge.Length; i++)
        {
            league.Add(new CharacterWrapper(startingLeauge[i]));
			if (startingLeauge[i].name == "Player")
				player = league[league.Count - 1];
        }
        ShuffleLeage();
        battleManager = GetComponent<BattleManager>();
    }

    void ShuffleLeage()
    {
        for (int i = 0; i < league.Count; i++)
        {
            var temp = league[i];
            int rnd = Random.Range(i, league.Count);
            league[i] = league[rnd];
            league[rnd] = temp;
        }
    }

    void Start()
    {
        tournament.SetStage(league);
        battleManager.battleUI.SetActive(true);
        battleManager.CloseBattleUI();
        battleManager.Battle(league[0], league[1]);
        matchIndex = 1;
        tournament.gameObject.SetActive(true);
    }
	
    public void DefeatCharacter(CharacterWrapper c)
    {
        battleManager.CloseBattleUI();
        tournament.DefeatCombatant(c);
        if (league.Remove(c))
        {
            matchIndex++;
        }
        else
        {
            Debug.LogError("Could not find character to remove from league");
        }
        if(matchIndex >= league.Count)
        {
            for (int i = 0; i < league.Count; i++)
                league[i].NextStage();
            tournament.SetStage(league);
            if (tournament.IsTournamentOver)
                return;
            matchIndex = 1;
        }
        battleManager.Battle(league[matchIndex-1], league[matchIndex]);
    }
}
