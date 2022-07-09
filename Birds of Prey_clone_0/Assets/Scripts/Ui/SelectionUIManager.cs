using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SelectionUIManager : MonoBehaviour {

    [HideInInspector] public int currentlySelectedIndexToChange = 0;

    [SerializeField] private GameObject contentHolder;
    [SerializeField] private GameObject planePreviewImagePrefab;
    [SerializeField] private GameObject selectedPlanesHolder;

    [SerializeField] private GameObject enterGameCode;

    private List<PlaneImageController> allPreviewControllers;
    private selectedPlanePreviewManager[] allSelectionImageControllers;


    private void Start() {
        enterGameCode.SetActive(!NetworkedVariables.isRoomCreator);
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
        }
    }

    private class CreateRoomMessage {
        string id;
        string name;
        string[] planeTypes;
        string startHealth;
        string gameModeInfo;
        int worldIndex;

        public CreateRoomMessage(string id, string name, string[] planeTypes, string startHealth, GameModeTypes selectedGameMode, int worldIndex) {
            this.id = id;
            this.name = name;
            this.startHealth = startHealth;
            this.planeTypes = (string[])planeTypes.Clone();
            this.gameModeInfo = GameModeManager.gameModes[selectedGameMode].getGameModeInfo();
            this.worldIndex = worldIndex;
        }

        public string getCreationInfo() {
            return $"\"Id\":\"{this.id}\",\"name\":\"{this.name}\",\"planeTypes\":{JsonConvert.SerializeObject(this.planeTypes)},\"startHealth\":\"{this.startHealth}\",\"worldIndex\":\"{this.worldIndex}\",\"gameModeInfo\":{this.gameModeInfo}";
        }
    }
}
