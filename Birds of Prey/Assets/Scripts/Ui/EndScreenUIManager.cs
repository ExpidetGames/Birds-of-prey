using UnityEngine;

public class EndScreenUIManager : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void backToMainMenu() {
        TCPClient.callStack.Add("{\"type\":\"clientDisconnected\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId + "\"}");
        NetworkedVariables.inGame = false;
    }
}
