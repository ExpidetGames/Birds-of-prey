using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUIManager : MonoBehaviour {

    [SerializeField] private TMP_Text healthDisplay;
    [SerializeField] private TMP_Text lockDisplay;
    [SerializeField] private PlayerHealth playerHealth;

    private void Start() {
        if(lockDisplay != null)
            lockDisplay.text = "";
    }

    private void Update() {
        if(healthDisplay != null) {
            healthDisplay.SetText(playerHealth.getHealth().ToString());
        }

        if(NetworkedVariables.targetLockInfo.Count > 0) {
            string stringToDisplay = "";
            lock(NetworkedVariables.targetLockInfo) {
                foreach(Dictionary<string, string> lockInfo in NetworkedVariables.targetLockInfo) {
                    if(lockInfo["target"] == NetworkedVariables.playerId) {
                        stringToDisplay += $"You got locked by the Player {lockInfo["shooter"]}";
                    }
                    if(lockInfo["shooter"] == NetworkedVariables.playerId) {
                        stringToDisplay += $"You locked the Player {lockInfo["target"]}";
                    }
                    stringToDisplay += "\n";
                }
                lockDisplay.SetText(stringToDisplay);
                NetworkedVariables.targetLockInfo.Clear();
            }
        }
    }


}
