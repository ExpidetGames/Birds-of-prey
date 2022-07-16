using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NamePlateController : MonoBehaviour {

    [SerializeField] private Image readyImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject otherOptionsButton;
    [SerializeField] private GameObject otherOptions;
    [SerializeField] private GameObject moveToNextTeamButton;

    [HideInInspector] public string clientId;

    private List<string> allTeams;
    private string currentTeam;
    private bool currentlyShowsOtherOptions;

    public void initValues(List<string> allTeams, string currentTeam) {
        this.allTeams = allTeams;
        this.currentTeam = currentTeam;
        if(allTeams.Count > 0) {
            moveToNextTeamButton.GetComponentInChildren<TMP_Text>().text = $"Move to next Team {allTeams[(allTeams.IndexOf(currentTeam) + 1) % allTeams.Count]}";
        }
    }

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
        nameText.text = name + ((NetworkedVariables.playerId == clientId) ? " (You)" : "");
    }

    public void onPressed() {
        currentlyShowsOtherOptions = !currentlyShowsOtherOptions;
        otherOptions.SetActive(currentlyShowsOtherOptions);
    }

    public void transferOwnership() {
        string newOwnerId = this.GetComponentInParent<NamePlateController>().clientId;
        TCPClient.callStack.Insert(0, "{\"type\":\"transferOwnership\", \"roomId\":\"" + NetworkedVariables.roomId + "\", \"newOwner\":\"" + newOwnerId + "\"}");
    }

    public void moveToNextTeam() {
        string newTeam = allTeams[(allTeams.IndexOf(currentTeam) + 1) % allTeams.Count];
        string playerToMove = this.GetComponentInParent<NamePlateController>().clientId;
        TCPClient.callStack.Insert(0, "{\"type\":\"changeTeam\", \"roomId\":\"" + NetworkedVariables.roomId + "\", \"playerId\":\"" + playerToMove + "\", \"newTeam\":\"" + newTeam + "\"}");
    }
}
