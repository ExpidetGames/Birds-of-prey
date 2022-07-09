using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using System.Linq;

public class SelectionUIManager : MonoBehaviour {

    [HideInInspector] public int currentlySelectedIndexToChange = 0;

    [SerializeField] private GameObject contentHolder;
    [SerializeField] private GameObject planePreviewImagePrefab;
    [SerializeField] private GameObject selectedPlanesHolder;

    [SerializeField] private GameObject roomIdInputHolder;

    private TMP_InputField roomIdInput;
    private List<PlaneImageController> allPreviewControllers;
    private selectedPlanePreviewManager[] allSelectionImageControllers;


    private void Start() {
        roomIdInputHolder.SetActive(!NetworkedVariables.isRoomCreator);
        roomIdInput = roomIdInputHolder.GetComponent<TMP_InputField>();
        roomIdInput.SetTextWithoutNotify("AAAAAA");
        allPreviewControllers = new List<PlaneImageController>();
        allSelectionImageControllers = selectedPlanesHolder.GetComponentsInChildren<selectedPlanePreviewManager>();

        allSelectionImageControllers[currentlySelectedIndexToChange].select();

        for(int i = 0; i < allSelectionImageControllers.Length; i++) {
            allSelectionImageControllers[i].myIndex = i;
            allSelectionImageControllers[i].manager = this;
        }

        Populate();

    }

    void Populate() {
        int currentIndex = 0;
        foreach(PrefabOrganizer.Plane plane in PrefabOrganizer.Planes.Values) {
            GameObject newPlaneImage = Instantiate(planePreviewImagePrefab);
            PlaneImageController imageController = newPlaneImage.GetComponentInChildren<PlaneImageController>();

            imageController.myIndex = currentIndex;
            imageController.setImage(plane.playerType);
            imageController.manager = this;

            newPlaneImage.transform.SetParent(contentHolder.transform);
            allPreviewControllers.Add(imageController);
            currentIndex++;
        }
    }

    public void selectElement(int index) {
        allSelectionImageControllers[currentlySelectedIndexToChange].changeImage(allPreviewControllers[index].displayedPlaneType);
    }

    public void changeSelectedSelectionPreview(int newIndex) {
        allSelectionImageControllers[currentlySelectedIndexToChange].deselect();
        allSelectionImageControllers[newIndex].select();
        currentlySelectedIndexToChange = newIndex;
    }

    public void back() {
        NetworkedVariables.scenceToLoad.Add(6);
    }

    public void connectToRoom() {
        if(NetworkedVariables.isRoomCreator) {
            PlaneTypes firstPlane = allSelectionImageControllers[0].currentlySelectedPlaneType;
            string[] planeTypes = new string[allSelectionImageControllers.Length];
            for(int i = 0; i < planeTypes.Length; i++) {
                planeTypes[i] = allSelectionImageControllers[i].currentlySelectedPlaneType.ToString();
            }
            CreateRoomMessage crm = new CreateRoomMessage(NetworkedVariables.playerId, NetworkedVariables.name, planeTypes, PrefabOrganizer.Planes[firstPlane].startHealth.ToString(), NetworkedVariables.currentGameMode, NetworkedVariables.worldIndex);
            string message = "{\"type\":\"createRoom\", " + crm.getCreationInfo() + "}";
            TCPClient.callStack.Insert(0, message);
        } else {
            if(roomIdInput.text.Length == 6 && !roomIdInput.text.Any(char.IsDigit)) {
                PlaneTypes firstPlane = allSelectionImageControllers[0].currentlySelectedPlaneType;
                string[] planeTypes = new string[allSelectionImageControllers.Length];
                for(int i = 0; i < planeTypes.Length; i++) {
                    planeTypes[i] = allSelectionImageControllers[i].currentlySelectedPlaneType.ToString();
                }
                JoinRoomMessage jrm = new JoinRoomMessage(NetworkedVariables.playerId, roomIdInput.text, NetworkedVariables.name, planeTypes, PrefabOrganizer.Planes[firstPlane].startHealth.ToString());
                TCPClient.callStack.Insert(0, "{\"type\":\"joinRoom\", " + jrm.getJoiningInfo() + "}");
            } else {
                TMPro.TextMeshProUGUI[] allTexts = roomIdInput.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
                for(int i = 0; i < allTexts.Length; i++) {
                    allTexts[i].color = Color.red;
                }
                NetworkedVariables.errorMessage = "Invalid Room Id. The Room Id has six letters with zero digits! There is no Case sensitivity!";
            }

        }
    }

    private class JoinRoomMessage {
        string playerId;
        string roomId;
        string name;
        string[] planeTypes;
        string startHealth;

        public JoinRoomMessage(string playerId, string roomId, string name, string[] planeTypes, string startHealth) {
            this.playerId = playerId;
            this.roomId = roomId;
            this.name = name;
            this.planeTypes = (string[])planeTypes.Clone();
            this.startHealth = startHealth;
        }

        public string getJoiningInfo() {
            return $"\"playerId\":\"{playerId}\", \"roomId\":\"{roomId}\", \"name\":\"{name}\",\"planeTypes\":{JsonConvert.SerializeObject(this.planeTypes)},\"startHealth\":\"{startHealth}\"";
        }
    }

    private class CreateRoomMessage {
        string playerId;
        string name;
        string[] planeTypes;
        string startHealth;
        string gameModeInfo;
        int worldIndex;

        public CreateRoomMessage(string id, string name, string[] planeTypes, string startHealth, GameModeTypes selectedGameMode, int worldIndex) {
            this.playerId = id;
            this.name = name;
            this.startHealth = startHealth;
            this.planeTypes = (string[])planeTypes.Clone();
            this.gameModeInfo = GameModeManager.gameModes[selectedGameMode].getGameModeInfo();
            this.worldIndex = worldIndex;
        }

        public string getCreationInfo() {
            return $"\"playerId\":\"{this.playerId}\",\"name\":\"{this.name}\",\"planeTypes\":{JsonConvert.SerializeObject(this.planeTypes)},\"startHealth\":\"{this.startHealth}\",\"worldIndex\":\"{this.worldIndex}\",\"gameModeInfo\":{this.gameModeInfo}";
        }
    }
}
