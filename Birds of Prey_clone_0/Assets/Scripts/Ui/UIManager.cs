using UnityEngine;
using System.Linq;
using System.Collections;
using TMPro;

//This class is responsible for updating UI and holds functions the buttons execute
public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField roomIDInput;
    [SerializeField] private TMP_InputField serverIpInput;
    [SerializeField] private TMP_InputField localUdpPortInput;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text roomIdDisplay;
    [SerializeField] private TMP_Text errorDisplay;
    [SerializeField] private TMP_Text nameDisplay;
    [Space]
    [SerializeField] private string ip;
    [SerializeField] private int tcpServerPort;
    [SerializeField] private int udpServerPort;
    [SerializeField] private int localUdpPort;
    [Space]
    [SerializeField] private PlaneTypes planeType;


    private void Start() {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void Update() {

        if(roomIdDisplay != null){
            roomIdDisplay.text = NetworkedVariables.roomId;
        }
        if(!string.IsNullOrEmpty(NetworkedVariables.joinErrorMessage) && errorDisplay != null){
            errorDisplay.text = NetworkedVariables.joinErrorMessage;
        }
        if(NetworkedVariables.inGame && NetworkedVariables.playerNames.ContainsKey(NetworkedVariables.playerId) && nameDisplay != null){
            nameDisplay.text = NetworkedVariables.playerNames[NetworkedVariables.playerId];
        }
    }

    

    IEnumerator createRoomTask(){
        connectToServer();
        yield return new WaitWhile(() => string.IsNullOrEmpty(NetworkedVariables.playerId));
        string message = "{\"type\":\"createRoom\", \"Id\":\""+ NetworkedVariables.playerId +"\", \"name\":\""+ nameInput.text +"\", \"startHealth\":\""+PrefabOrganizer.Planes[planeType].startHealth+"\", \"planeType\":\""+planeType.ToString()+"\"}";
        TCPClient.callStack.Insert(0, message);
    }

    IEnumerator joinRoomTask(){
        connectToServer();
        yield return new WaitWhile(() => string.IsNullOrEmpty(NetworkedVariables.playerId));
        roomIDInput.SetTextWithoutNotify("AAAAAA");
        if(roomIDInput.text.Length == 6 && !roomIDInput.text.Any(char.IsDigit)){
            string message = "{\"type\":\"joinRoom\", \"Id\":\""+  NetworkedVariables.playerId +"\",\"roomId\":\""+ roomIDInput.text.ToUpper() +"\", \"name\":\""+ nameInput.text +"\", \"startHealth\":\""+PrefabOrganizer.Planes[planeType].startHealth+"\", \"planeType\":\""+planeType.ToString()+"\"}";
            TCPClient.callStack.Insert(0, message);;
        }else{
            TMPro.TextMeshProUGUI[] allTexts = roomIDInput.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            for(int i = 0; i < allTexts.Length; i++){
                allTexts[i].color = Color.red;
            }
            NetworkedVariables.joinErrorMessage = "Invalid Room Id. The Room Id has six letters with zero digits! There is no Case sensitivity!";
        }
    }

    public void createRoom(){
        StartCoroutine(createRoomTask());
    }

    public void joinRoom(){
        StartCoroutine(joinRoomTask());
    }
    
    public void disconnect(){
        if(NetworkedVariables.inGame){
            TCPClient.callStack.Add("{\"type\":\"clientDisconnected\", \"roomId\":\""+ NetworkedVariables.roomId +"\",\"Id\":\""+ NetworkedVariables.playerId.ToString() +"\"}");
            NetworkedVariables.inGame = false;
        }   
    }

    private void connectToServer(){
        UDPClient udpClient = new UDPClient(serverIpInput.text, 9535, int.Parse(localUdpPortInput.text));
        udpClient.connect();
        TCPClient tcpClient = new TCPClient(serverIpInput.text, 9536);
        tcpClient.connect();
    }
}
