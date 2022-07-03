using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class PlayerCollisionManager : MonoBehaviour {


    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Projectile" && !NetworkedVariables.isDead) {
            Projectile projectile = other.gameObject.GetComponent<Projectile>();
            if(projectile.shooter != NetworkedVariables.playerId && NetworkedVariables.connectedClients[NetworkedVariables.playerId].playerHealth - projectile.damage > 0) {
                TCPClient.callStack.Insert(0, "{\"type\":\"playerHit\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"playerId\":\"" + NetworkedVariables.playerId + "\", \"shooterId\":\"" + projectile.shooter + "\", \"damage\":\"" + projectile.damage + "\"}");
            } else if(projectile.shooter != NetworkedVariables.playerId && NetworkedVariables.connectedClients[NetworkedVariables.playerId].playerHealth - projectile.damage <= 0) {
                TCPClient.callStack.Insert(0, "{\"type\":\"playerDied\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"playerId\":\"" + NetworkedVariables.playerId + "\", \"shooterId\":\"" + projectile.shooter + "\"}");
                NetworkedVariables.isDead = true;
            }
        }
    }
}
