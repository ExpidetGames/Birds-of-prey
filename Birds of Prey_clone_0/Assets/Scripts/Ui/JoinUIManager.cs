using UnityEngine;
using System.Collections;
using TMPro;
//This class is responsible for updating UI and holds functions the buttons execute
public class JoinUIManager : MonoBehaviour {
    [SerializeField] private TMP_InputField serverIpInput;
    [SerializeField] private TMP_InputField localUdpPortInput;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Animator animator;
    [Space]
    [SerializeField] private string ip;
    [SerializeField] private int tcpServerPort;
    [SerializeField] private int udpServerPort;
    [SerializeField] private int localUdpPort;
    [Space]
    [HideInInspector] public PlaneTypes planeType;

    private float timeUntilAnimStarts;

    private void Start() {
        timeUntilAnimStarts = Random.Range(5f, 10f);
        planeType = PlaneTypes.STANDARD_AIRCRAFT;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void Update() {



        if(timeUntilAnimStarts <= 0) {
            animator.SetBool("startAnimation", true);
            timeUntilAnimStarts = Random.Range(10f, 30f);
        } else {
            timeUntilAnimStarts -= Time.deltaTime;
        }

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("MainMenuBackground")) {
            animator.SetBool("startAnimation", false);
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

