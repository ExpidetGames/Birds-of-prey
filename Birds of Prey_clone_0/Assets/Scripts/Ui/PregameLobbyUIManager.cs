using UnityEngine;
using TMPro;

public class PregameLobbyUIManager : MonoBehaviour {

    [SerializeField] private TMP_Text roomCode;
    [SerializeField] private TMP_Text mode;
    [SerializeField] private TMP_Text readyUpButtonText;
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private GameObject scrollView;

    private bool isClientReady;

    void Start() {
        roomCode.text = NetworkedVariables.roomId;
        mode.text = NetworkedVariables.currentGameMode.ToString();
        readyUpButtonText.text = (NetworkedVariables.isRoomCreator) ? "Start the game" : "Ready up";
        if(GameModeManager.gameModes[NetworkedVariables.currentGameMode].hasTeams) {
            for(int i = 0; i < GameModeManager.gameModes[NetworkedVariables.currentGameMode].teamCount;i++) {
                Instantiate(scrollView);
            }
        }
    }

    void Update() {

    }

    public void leaveRoom() {
        if(NetworkedVariables.inGame) {
            TCPClient.callStack.Add("{\"type\":\"clientDisconnected\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId.ToString() + "\"}");
            NetworkedVariables.inGame = false;
        }
    }

    public void readyUp() {
        isClientReady = !isClientReady;
        if(NetworkedVariables.isRoomCreator) {
            TCPClient.callStack.Add("{\"type\":\"startGame\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId.ToString() + "\"}");
        } else {
            TCPClient.callStack.Add("{\"type\":\"" + ((isClientReady) ? "ready" : "unready") + "\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId.ToString() + "\"}");
        }
    }
}
