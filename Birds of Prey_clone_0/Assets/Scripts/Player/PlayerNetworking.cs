using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworking : MonoBehaviour
{
    [SerializeField] private Transform aircraftTransform;

    private void Start() {

    }
    
    void Update(){
        if(NetworkedVariables.inGame){
            if(aircraftTransform != null){
                UDPClient.udpCallStack.Insert(0, "{\"type\":\"transformUpdate\",\"roomId\":\""+ NetworkedVariables.roomId +"\",\"playerId\":\""+ NetworkedVariables.playerId +"\", \"newTransform\":\""+ JsonParser.transformToJson(aircraftTransform)+"\"}");
            }
        }
    }
}
