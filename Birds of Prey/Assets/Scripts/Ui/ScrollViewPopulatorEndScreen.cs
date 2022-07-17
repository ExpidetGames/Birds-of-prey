using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewPopulatorEndScreen : MonoBehaviour {

    [SerializeField] private GameObject playerInfo;

    // Start is called before the first frame update
    void Start() {
        foreach(string clientId in NetworkedVariables.connectedClients.Keys) {
            GameObject currentPlayerInfo = Instantiate(playerInfo);
            currentPlayerInfo.GetComponent<PlayerInfoManager>().init(clientId);
        }
    }
}
