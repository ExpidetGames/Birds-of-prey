using UnityEngine;

public class DeathUIManager : MonoBehaviour
{
    [SerializeField] private AudioListener audioListener;

    private void Update() {
        
    }
    public void rejoinTheFight(){
        //audioListener.enabled = false;
        TCPClient.callStack.Insert(0, "{\"type\":\"rejoin\", \"roomId\":\""+NetworkedVariables.roomId+"\", \"playerId\":\""+NetworkedVariables.playerId+"\", \"newHealth\":\""+PrefabOrganizer.Planes[NetworkedVariables.planeTypes[NetworkedVariables.playerId]].startHealth+"\"}");
    }

    public void surrender(){
        //audioListener.enabled = false;
    }
}
