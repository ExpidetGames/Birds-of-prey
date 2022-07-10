using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class PregameLobbyUIManager : MonoBehaviour {

    [SerializeField] private TMP_Text roomCode;
    [SerializeField] private TMP_Text mode;
    [SerializeField] private TMP_Text readyUpButtonText;
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject spawnPointsParent;
    [SerializeField] private GameObject startGameButton;

    //Contain the team Colors as keys and the actual ScrollViews as Values
    private Dictionary<string, GameObject> scrollViews = new Dictionary<string, GameObject>();
    private Dictionary<string, bool> clientReadyStates = new Dictionary<string, bool>();
    private Dictionary<string, string> teamColors = new Dictionary<string, string>();
    private Transform[] spawnPoints;

    void Start() {
        roomCode.text = $"RoomId: {NetworkedVariables.roomId}";
        mode.text = $"Mode: {NetworkedVariables.currentGameMode.ToString()}";
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
        startGameButton.SetActive(NetworkedVariables.isRoomCreator);
        if(clientReadyStates.ContainsKey(NetworkedVariables.playerId)) {
            readyUpButtonText.text = (clientReadyStates[NetworkedVariables.playerId]) ? "Unready" : "Ready";
        }

        if(startGameButton.activeInHierarchy) {
            if(clientReadyStates.Count > 0 && !clientReadyStates.ContainsValue(false)) {
                startGameButton.GetComponent<Button>().interactable = true;
            } else {
                startGameButton.GetComponent<Button>().interactable = false;
            }
        }
        //A new client joined the lobby => It needs to be shown
        if(NetworkedVariables.connectedClients.Count > clientReadyStates.Count) {
            foreach(Client newClient in NetworkedVariables.connectedClients.Values) {
                if(!clientReadyStates.ContainsKey(newClient.id)) {
                    scrollViews[newClient.teamColor].GetComponentInChildren<ScrollViewPopulatorPGLobby>().addToList(newClient);
                    clientReadyStates.Add(newClient.id, newClient.isReady);
                    teamColors.Add(newClient.id, newClient.teamColor);
                }
            }
        } else if(NetworkedVariables.connectedClients.Count == clientReadyStates.Count) {
            //There are as many clients spawned as there are in game so it needs to be check if any of them changed their ready state
            foreach(Client client in NetworkedVariables.connectedClients.Values) {
                if(client.isReady != clientReadyStates[client.id]) {
                    clientReadyStates[client.id] = client.isReady;
                    scrollViews[client.teamColor].GetComponentInChildren<ScrollViewPopulatorPGLobby>().updateClient(client);
                }

                if(client.teamColor != teamColors[client.id]) {
                    //Removing from old list
                    scrollViews[teamColors[client.id]].GetComponentInChildren<ScrollViewPopulatorPGLobby>().removeFromList(client);
                    //Updating value
                    teamColors[client.id] = client.teamColor;
                    //Adding to new List
                    scrollViews[teamColors[client.id]].GetComponentInChildren<ScrollViewPopulatorPGLobby>().addToList(client);
                }
            }
        }
        if(NetworkedVariables.disconnectedPlayerIds.Count > 0) {
            //Somebody disconnected so the client has to be removed
            foreach(string disconnectedId in NetworkedVariables.disconnectedPlayerIds) {
                if(NetworkedVariables.connectedClients.ContainsKey(disconnectedId)) {
                    Client disconnectedClient = NetworkedVariables.connectedClients[disconnectedId];
                    clientReadyStates.Remove(disconnectedClient.id);
                    scrollViews[disconnectedClient.teamColor].GetComponentInChildren<ScrollViewPopulatorPGLobby>().removeFromList(disconnectedClient);
                    if(disconnectedClient.id == NetworkedVariables.playerId) {
                        NetworkedVariables.connectedClients.Clear();
                    } else {
                        NetworkedVariables.connectedClients.Remove(disconnectedClient.id);
                    }
                }
            }
            NetworkedVariables.disconnectedPlayerIds.Clear();
        }
    }

    public void leaveRoom() {
        if(NetworkedVariables.inGame) {
            TCPClient.callStack.Add("{\"type\":\"clientDisconnected\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId.ToString() + "\", \"wasOwner\":\"" + NetworkedVariables.isRoomCreator + "\"}");
            NetworkedVariables.inGame = false;
        }
    }

    public void readyUp() {
        TCPClient.callStack.Add("{\"type\":\"" + ((clientReadyStates[NetworkedVariables.playerId]) ? "unready" : "ready") + "\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId.ToString() + "\"}");
    }

    public void startGame() {
        TCPClient.callStack.Insert(0, "{\"type\":\"startGame\", \"roomId\":\"" + NetworkedVariables.roomId + "\"}");
    }
}

