using UnityEngine;

public class DeathUIManager : MonoBehaviour {

    public void rejoinTheFight() {
        //This is called next Type because the plane Index gets Updated when the player dies not when it rejoins
        PlaneTypes nextType = NetworkedVariables.connectedClients[NetworkedVariables.playerId].getCurrentType();
        Debug.Log($"The current Index is: {NetworkedVariables.connectedClients[NetworkedVariables.playerId].currentPlaneType}");
        TCPClient.callStack.Insert(0, "{\"type\":\"rejoin\", \"roomId\":\"" + NetworkedVariables.roomId + "\", \"playerId\":\"" + NetworkedVariables.playerId + "\", \"newHealth\":\"" + PrefabOrganizer.Planes[nextType].startHealth + "\"}");
    }

    public void surrender() {
        //Disconnecting the Player out of the room and sending it to the join Screen
        TCPClient.callStack.Add("{\"type\":\"clientDisconnected\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"Id\":\"" + NetworkedVariables.playerId + "\"}");
        NetworkedVariables.inGame = false;
    }
}
