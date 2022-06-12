using UnityEngine;

public class DeathUIManager : MonoBehaviour {

    public void rejoinTheFight() {
        TCPClient.callStack.Insert(0, "{\"type\":\"rejoin\", \"roomId\":\"" + NetworkedVariables.roomId + "\", \"playerId\":\"" + NetworkedVariables.playerId + "\", \"newHealth\":\"" + PrefabOrganizer.Planes[NetworkedVariables.planeTypes[NetworkedVariables.playerId]].startHealth + "\"}");
    }

    public void surrender() {
        if(true) {

        } else {

        }
    }
}
