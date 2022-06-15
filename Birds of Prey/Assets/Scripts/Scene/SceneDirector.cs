using UnityEngine;
using UnityEngine.SceneManagement;

//This class is responsible for switching scenes

public class SceneDirector : MonoBehaviour {
    private void Start() {
        DontDestroyOnLoad(this.gameObject);
        Screen.SetResolution(1000, 1000, false);
    }

    void Update() {
        if(NetworkedVariables.scenceToLoad.Count > 0) {
            foreach(int sceneIndex in NetworkedVariables.scenceToLoad) {
                SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
                // Debug.Log($"Loaded scene {sceneIndex}");
            }
            NetworkedVariables.scenceToLoad.Clear();
        }
    }

    private void OnApplicationQuit() {
        if(TCPClient.ws == null) {
            return;
        }
        if(TCPClient.ws.ReadyState == WebSocketSharp.WebSocketState.Open) {
            TCPClient.ws.SendAsync("{\"type\":\"completeDelete\", \"roomId\":\"" + NetworkedVariables.roomId + "\", \"playerId\":\"" + NetworkedVariables.playerId + "\"}", null);
        }
        TCPClient.ws.Close();
    }
}
