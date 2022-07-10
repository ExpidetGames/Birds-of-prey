using UnityEngine;
using System.Collections;
using TMPro;
//This class is responsible for updating UI and holds functions the buttons execute
public class JoinUIManager : MonoBehaviour {
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
        NetworkedVariables.scenceToLoad.Add(7);
    }

    public void createRoom() {
        NetworkedVariables.name = nameInput.text;
        NetworkedVariables.isRoomCreator = true;
        StartCoroutine(createRoomTask());
    }

    public void joinRoom() {
        NetworkedVariables.name = nameInput.text;
        NetworkedVariables.isRoomCreator = false;
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

