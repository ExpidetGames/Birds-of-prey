using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
//This class is responsible for updating UI and holds functions the buttons execute
public class JoinUIManager : MonoBehaviour {
    [SerializeField] private TMP_InputField roomIDInput;
    [SerializeField] private TMP_InputField serverIpInput;
    [SerializeField] private TMP_InputField localUdpPortInput;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text errorDisplay;
    [SerializeField] private TMP_Dropdown gameModes;
    [SerializeField] private Canvas JoinCanvas;
    [SerializeField] private Canvas selectionCanvas;
    [Space]
    [SerializeField] private string ip;
    [SerializeField] private int tcpServerPort;
    [SerializeField] private int udpServerPort;
    [SerializeField] private int localUdpPort;
    [Space]
    [HideInInspector] public PlaneTypes planeType;


    private void Start() {
        populateDropdown(gameModes);
        gameModes.SetValueWithoutNotify((int)NetworkedVariables.currentGameMode);
        planeType = PlaneTypes.STANDARD_AIRCRAFT;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void Update() {

        if(!string.IsNullOrEmpty(NetworkedVariables.errorMessage) && errorDisplay != null) {
            errorDisplay.text = NetworkedVariables.errorMessage;
        }
    }



    IEnumerator createRoomTask() {
        connectToServer();
        yield return new WaitWhile(() => string.IsNullOrEmpty(NetworkedVariables.playerId));
        string message = "{\"type\":\"createRoom\", \"Id\":\"" + NetworkedVariables.playerId + "\", \"name\":\"" + nameInput.text + "\", \"startHealth\":\"" + PrefabOrganizer.Planes[planeType].startHealth + "\", \"planeType\":\"" + planeType.ToString() + "\", \"worldIndex\":\"" + PrefabOrganizer.Worlds[UnityEngine.Random.Range(0, PrefabOrganizer.Worlds.Count)].buildIndex + "\", \"gameModeInfo\":" + GameModeManager.gameModes[NetworkedVariables.currentGameMode].getGameModeInfo() + "}";
        TCPClient.callStack.Insert(0, message);
    }

    IEnumerator joinRoomTask() {
        connectToServer();
        yield return new WaitWhile(() => string.IsNullOrEmpty(NetworkedVariables.playerId));
        roomIDInput.SetTextWithoutNotify("AAAAAA");
        if(roomIDInput.text.Length == 6 && !roomIDInput.text.Any(char.IsDigit)) {
            string message = "{\"type\":\"joinRoom\", \"Id\":\"" + NetworkedVariables.playerId + "\",\"roomId\":\"" + roomIDInput.text.ToUpper() + "\", \"name\":\"" + nameInput.text + "\", \"startHealth\":\"" + PrefabOrganizer.Planes[planeType].startHealth + "\", \"planeType\":\"" + planeType.ToString() + "\"}";
            TCPClient.callStack.Insert(0, message);
        } else {
            TMPro.TextMeshProUGUI[] allTexts = roomIDInput.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            for(int i = 0; i < allTexts.Length; i++) {
                allTexts[i].color = Color.red;
            }
            NetworkedVariables.errorMessage = "Invalid Room Id. The Room Id has six letters with zero digits! There is no Case sensitivity!";
        }
    }

    public void createRoom() {
        StartCoroutine(createRoomTask());
    }

    public void joinRoom() {
        StartCoroutine(joinRoomTask());
    }

    public void selectPlane() {
        JoinCanvas.GetComponent<Canvas>().enabled = false;
        selectionCanvas.GetComponent<Canvas>().enabled = true;
    }

    private void connectToServer() {
        if(UDPClient.updClient == null) {
            UDPClient udpClient = new UDPClient(serverIpInput.text, 9535, int.Parse(localUdpPortInput.text));
            udpClient.connect();
        }
        if(TCPClient.ws == null) {
            TCPClient tcpClient = new TCPClient(serverIpInput.text, 9536);
            tcpClient.connect();
        }
    }

    public void onGameModeChanged() {
        Debug.Log(gameModes.value);
        NetworkedVariables.currentGameMode = (GameModeTypes)gameModes.value;
        Debug.Log($"The new Mode is: {NetworkedVariables.currentGameMode}");
    }

    public void populateDropdown(TMP_Dropdown dropdown) {
        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

        for(int i = 0; i < Enum.GetNames(typeof(GameModeTypes)).Length; i++)//Populate new Options
        {
            newOptions.Add(new TMP_Dropdown.OptionData(Enum.GetName(typeof(GameModeTypes), i)));
        }

        dropdown.ClearOptions();//Clear old options
        dropdown.AddOptions(newOptions);//
    }
}
