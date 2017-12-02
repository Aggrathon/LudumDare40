using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

    public GameState state { get; protected set; }

    public CharacterWrapper enemy;
    public CharacterWrapper player;
    public List<CharacterWrapper> league;
    public Character[] startingLeauge;


    void Awake () {
        state = this;
	}
	
}
