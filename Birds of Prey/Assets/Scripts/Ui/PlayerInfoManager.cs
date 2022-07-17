using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfoManager : MonoBehaviour {

    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private TMP_Text killsDisplay;
    [SerializeField] private TMP_Text deathDisplay;

    private Client shownClient;

    private void Start() {
    }

    // Update is called once per frame
    void Update() {
        if(shownClient != null) {
            nameDisplay.text = shownClient.name;
            killsDisplay.text = shownClient.kills.ToString();
            deathDisplay.text = shownClient.deaths.ToString();
        }
    }

    public void init(string clientId) {
        shownClient = NetworkedVariables.connectedClients[clientId];
    }

}
