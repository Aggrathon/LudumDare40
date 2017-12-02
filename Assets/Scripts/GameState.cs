using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattleManager))]
public class GameState : MonoBehaviour {

    public static GameState state { get; protected set; }
    
    public List<CharacterWrapper> league;
    public Character[] startingLeauge;


    void Awake () {
        state = this;
        league.Clear();
        for (int i = 0; i < startingLeauge.Length; i++)
        {
            league.Add(new CharacterWrapper(startingLeauge[i]));
        }
        ShuffleLeage();
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
        GetComponent<BattleManager>().Battle(league[0], league[1]);
    }
	
    public void DefeatCharacter(CharacterWrapper c)
    {
    }
}
