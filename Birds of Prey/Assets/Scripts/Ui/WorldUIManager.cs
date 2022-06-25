using UnityEngine;
using TMPro;

public class WorldUIManager : MonoBehaviour {

    [SerializeField] private TMP_Text roomIdDisplay;
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private GameObject pauseMenu;

    private void Start() {
        pauseMenu.GetComponent<Canvas>().enabled = false;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            pauseMenu.GetComponent<Canvas>().enabled = !pauseMenu.GetComponent<Canvas>().enabled;
        }
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

    public void continueGame() {
        pauseMenu.GetComponent<Canvas>().enabled = false;
    }
}