using UnityEngine;
using TMPro;

public class WorldUIManager : MonoBehaviour {

    [SerializeField] private TMP_Text roomIdDisplay;
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private GameObject pauseButton;
    private void Start() {
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
    }

    private void Update() {
        if(pauseMenu.activeInHierarchy) {
            if(roomIdDisplay != null) {
                roomIdDisplay.text = NetworkedVariables.roomId;
            }
            if(NetworkedVariables.inGame && NetworkedVariables.playerNames.ContainsKey(NetworkedVariables.playerId) && nameDisplay != null) {
                nameDisplay.text = NetworkedVariables.playerNames[NetworkedVariables.playerId];
            }
        }
    }

    public void disconnect() {
        if(NetworkedVariables.inGame) {
            TCPClient.callStack.Add("{\"type\":\"clientDisconnected\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId.ToString() + "\"}");
            NetworkedVariables.inGame = false;
        }
    }

    public void pauseGame() {
        pauseButton.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void continueGame() {
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
    }
}