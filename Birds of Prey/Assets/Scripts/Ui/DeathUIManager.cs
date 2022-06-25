using UnityEngine;

public class DeathUIManager : MonoBehaviour {

    public void rejoinTheFight() {
        TCPClient.callStack.Insert(0, "{\"type\":\"rejoin\", \"roomId\":\"" + NetworkedVariables.roomId + "\", \"playerId\":\"" + NetworkedVariables.playerId + "\", \"newHealth\":\"" + PrefabOrganizer.Planes[NetworkedVariables.planeTypes[NetworkedVariables.playerId]].startHealth + "\"}");
    }

    public void surrender() {
        //Disconnecting the Player out of the room and sending it to the join Screen
        TCPClient.callStack.Add("{\"type\":\"clientDisconnected\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId + "\"}");
        NetworkedVariables.inGame = false;
    }
}
