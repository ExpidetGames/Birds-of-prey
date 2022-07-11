//
// Copyright (c) Brian Hernandez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
//

using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Hud : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private MouseFlightController mouseFlight = null;

    [Header("HUD Elements")]
    [SerializeField] private RectTransform boresight = null;
    [SerializeField] private RectTransform mousePos = null;
    [SerializeField] private TMP_Text healthDisplay = null;
    [SerializeField] private TMP_Text lockInfoDisplay = null;
    [SerializeField] private PlayerHealth playerHealth = null;

    private Camera playerCam = null;

    private void Awake() {
        if(lockInfoDisplay != null)
            lockInfoDisplay.text = "";

        if(mouseFlight == null)
            Debug.LogError(name + ": Hud - Mouse Flight Controller not assigned!");

        playerCam = mouseFlight.GetComponentInChildren<Camera>();

        if(playerCam == null)
            Debug.LogError(name + ": Hud - No camera found on assigned Mouse Flight Controller!");
    }

    private void Update() {
        if(mouseFlight == null || playerCam == null)
            return;

        UpdateGraphics(mouseFlight);
    }

    private void UpdateGraphics(MouseFlightController controller) {
        if(boresight != null) {
            boresight.position = playerCam.WorldToScreenPoint(controller.BoresightPos);
            boresight.gameObject.SetActive(boresight.position.z > 1f);
        }

        if(mousePos != null) {
            mousePos.position = playerCam.WorldToScreenPoint(controller.MouseAimPos);
            mousePos.gameObject.SetActive(mousePos.position.z > 1f);
        }

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
                lockInfoDisplay.SetText(stringToDisplay);
                NetworkedVariables.targetLockInfo.Clear();
            }
        }
    }

    public void SetReferenceMouseFlight(MouseFlightController controller) {
        mouseFlight = controller;
    }
}
