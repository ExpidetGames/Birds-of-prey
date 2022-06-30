using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PregameLobbyUIManager : MonoBehaviour {

    [SerializeField] private TMP_Text roomCode;
    [SerializeField] private TMP_Text mode;
    [SerializeField] private TMP_Text readyUpButtonText;
    [SerializeField] private GameObject namePlate;
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject spawnPointsParent;

    //Contain the team Colors as keys and the actual ScrollViews as Values
    private Dictionary<string, GameObject> scrollViews = new Dictionary<string, GameObject>();
    private Dictionary<string, bool> clientReadyStates = new Dictionary<string, bool>();
    private Transform[] spawnPoints;

    void Start() {
        roomCode.text = NetworkedVariables.roomId;
        mode.text = NetworkedVariables.currentGameMode.ToString();
        readyUpButtonText.text = (NetworkedVariables.isRoomCreator) ? "Start the game" : "Ready up";
        spawnPoints = spawnPointsParent.GetComponentsInChildren<RectTransform>();
        if(GameModeManager.gameModes[NetworkedVariables.currentGameMode].hasTeams) {
            for(int i = 0; i < GameModeManager.gameModes[NetworkedVariables.currentGameMode].teamCount; i++) {
                GameObject newScrollView = Instantiate(scrollView);
                newScrollView.GetComponent<RectTransform>().SetParent(this.gameObject.transform);
                newScrollView.GetComponent<RectTransform>().SetPositionAndRotation(spawnPoints[i].position, Quaternion.identity);
                scrollViews.Add(GameModeManager.gameModes[NetworkedVariables.currentGameMode].teamColors[i], newScrollView);
            }
        }
    }

    public void intialize() {

    }

    public void updateClient(string clientId, bool newReadyState) {
        clientReadyStates[clientId] = newReadyState;

    }

    public void leaveRoom() {
        if(NetworkedVariables.inGame) {
            TCPClient.callStack.Add("{\"type\":\"clientDisconnected\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId.ToString() + "\"}");
            NetworkedVariables.inGame = false;
        }
    }

    public void readyUp() {
        if(NetworkedVariables.isRoomCreator) {
            TCPClient.callStack.Add("{\"type\":\"startGame\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId.ToString() + "\"}");
        } else {
            TCPClient.callStack.Add("{\"type\":\"" + ((clientReadyStates[NetworkedVariables.playerId]) ? "unready" : "ready") + "\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId.ToString() + "\"}");
        }
    }
}

