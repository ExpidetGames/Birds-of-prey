using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NamePlateController : MonoBehaviour {

    [SerializeField] private Image readyImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] GameObject otherOptionsButton;
    [SerializeField] GameObject otherOptions;

    [HideInInspector] public string clientId;

    private bool currentlyShowsOtherOptions;

    private void Start() {
        currentlyShowsOtherOptions = false;
        otherOptions.SetActive(currentlyShowsOtherOptions);
    }

    private void Update() {
        otherOptionsButton.SetActive(NetworkedVariables.isRoomCreator);
    }
    public void updateReadyState(bool newState) {
        readyImage.color = (newState) ? Color.green : Color.red;
    }

    public void setName(string name) {
        nameText.text = name;
    }

    public void onPressed() {
        currentlyShowsOtherOptions = !currentlyShowsOtherOptions;
        otherOptions.SetActive(currentlyShowsOtherOptions);
    }

    public void transferOwnership() {
        string newOwnerId = this.GetComponentInParent<NamePlateController>().clientId;
        TCPClient.callStack.Insert(0, "{\"type\":\"transferOwnership\", \"roomId\":\"" + NetworkedVariables.roomId + "\", \"newOwner\":\"" + newOwnerId + "\"}");
    }
}
