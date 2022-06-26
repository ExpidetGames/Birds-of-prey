using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GameModeManager : MonoBehaviour {
    [SerializeField] private List<GameMode> _gameModes = new List<GameMode>();
    public static Dictionary<GameModeTypes, GameMode> gameModes = new Dictionary<GameModeTypes, GameMode>();

    private void Start() {
        foreach(GameMode gm in _gameModes) {
            gameModes[gm.gameModeType] = gm;
        }
    }
}


[Serializable]
public class GameMode {
    public GameModeTypes gameModeType;
    [Header("Join settings")]
    public bool hasMaxPlayers; //Implemented
    public int maxPlayerCount; //Implemented
    public bool hasTeams;
    public int teamCount;
    [Header("Finish Conditions")]
    public bool useTime;
    public float timeInSeconds;
    [Header("Win Conditions")]
    public bool useKills;
    public int killsToWin;
    public bool usePoints;
    public int pointsToWin;

    public string getGameModeInfo() {
        return JsonConvert.SerializeObject(this);
    }
}

public enum GameModeTypes {
    OneVsOne, OneVsOneDeathmatch
}
