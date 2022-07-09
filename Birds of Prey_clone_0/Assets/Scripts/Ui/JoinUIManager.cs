using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
//This class is responsible for updating UI and holds functions the buttons execute
public class JoinUIManager : MonoBehaviour {
    [SerializeField] private TMP_InputField roomIdInput;
    [SerializeField] private TMP_InputField serverIpInput;
    [SerializeField] private TMP_InputField localUdpPortInput;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text errorDisplay;
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
        NetworkedVariables.scenceToLoad.Add(6);
    }

    IEnumerator joinRoomTask() {
        connectToServer();
        yield return new WaitWhile(() => string.IsNullOrEmpty(NetworkedVariables.playerId));
        roomIdInput.SetTextWithoutNotify("AAAAAA");
        if(roomIdInput.text.Length == 6 && !roomIdInput.text.Any(char.IsDigit)) {
            string message = "{\"type\":\"joinRoom\", \"Id\":\"" + NetworkedVariables.playerId + "\",\"roomId\":\"" + roomIdInput.text.ToUpper() + "\", \"name\":\"" + nameInput.text + "\", \"startHealth\":\"" + PrefabOrganizer.Planes[planeType].startHealth + "\", \"planeType\":\"" + planeType.ToString() + "\"}";
            TCPClient.callStack.Insert(0, message);
        } else {
            TMPro.TextMeshProUGUI[] allTexts = roomIdInput.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            for(int i = 0; i < allTexts.Length; i++) {
                allTexts[i].color = Color.red;
            }
            NetworkedVariables.errorMessage = "Invalid Room Id. The Room Id has six letters with zero digits! There is no Case sensitivity!";
        }
    }

    public void createRoom() {
        NetworkedVariables.name = nameInput.text;
        NetworkedVariables.isRoomCreator = true;
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


}
