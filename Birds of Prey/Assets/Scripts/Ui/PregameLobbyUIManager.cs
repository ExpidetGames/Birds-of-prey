using UnityEngine;
using System.Linq;
using TMPro;
using System.Collections.Generic;

public class PregameLobbyUIManager : MonoBehaviour {

    [SerializeField] private TMP_Text roomCode;
    [SerializeField] private TMP_Text mode;
    [SerializeField] private TMP_Text readyUpButtonText;
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject spawnPointsParent;

    //Contain the team Colors as keys and the actual ScrollViews as Values
    private Dictionary<string, GameObject> scrollViews = new Dictionary<string, GameObject>();
    private Dictionary<string, bool> clientReadyStates = new Dictionary<string, bool>();
    private Transform[] spawnPoints;

    void Start() {
        roomCode.text = NetworkedVariables.roomId;
        mode.text = NetworkedVariables.currentGameMode.ToString();
        readyUpButtonText.text = "Ready Up";
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

    private void Update() {
        if(clientReadyStates.ContainsKey(NetworkedVariables.playerId)) {
            readyUpButtonText.text = (clientReadyStates[NetworkedVariables.playerId]) ? "Unready" : "Ready";
        }
        //A new client joined the lobby => It needs to be shown
        if(NetworkedVariables.connectedClients.Count > clientReadyStates.Count) {
            foreach(Client newClient in NetworkedVariables.connectedClients.Values) {
                if(!clientReadyStates.ContainsKey(newClient.id)) {
                    scrollViews[newClient.teamColor].GetComponentInChildren<ScrollViewPopulatorPGLobby>().addToList(newClient);
                    clientReadyStates.Add(newClient.id, newClient.isReady);
                }
            }
        } else if(NetworkedVariables.connectedClients.Count == clientReadyStates.Count) {
            foreach(Client client in NetworkedVariables.connectedClients.Values) {
                if(client.isReady != clientReadyStates[client.id]) {
                    clientReadyStates[client.id] = client.isReady;
                    scrollViews[client.teamColor].GetComponentInChildren<ScrollViewPopulatorPGLobby>().updateClient(client);
                }
            }
        }
    }

    public void leaveRoom() {
        if(NetworkedVariables.inGame) {
            TCPClient.callStack.Add("{\"type\":\"clientDisconnected\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId.ToString() + "\"}");
            NetworkedVariables.inGame = false;
        }
    }

    public void readyUp() {
        TCPClient.callStack.Add("{\"type\":\"" + ((clientReadyStates[NetworkedVariables.playerId]) ? "unready" : "ready") + "\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId.ToString() + "\"}");
        // if(NetworkedVariables.isRoomCreator) {
        //     TCPClient.callStack.Add("{\"type\":\"startGame\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId.ToString() + "\"}");
        // } else {
        //     TCPClient.callStack.Add("{\"type\":\"" + ((clientReadyStates[NetworkedVariables.playerId]) ? "unready" : "ready") + "\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId.ToString() + "\"}");
        // }
    }
}

