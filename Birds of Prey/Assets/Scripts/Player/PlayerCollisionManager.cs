using UnityEngine;

[DisallowMultipleComponent]
public class PlayerCollisionManager : MonoBehaviour {

    private const float groundDamageCooldown = 0.5f;
    private float currentGroundDamageCooldown = 0f;

    private void Update() {
        if(currentGroundDamageCooldown > 0) {
            currentGroundDamageCooldown -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log("Player collided with an object with tag: " + other.gameObject.tag);
        if(other.gameObject.tag == "Projectile" && !NetworkedVariables.connectedClients[NetworkedVariables.playerId].isDead) {
            Projectile projectile = other.gameObject.GetComponent<Projectile>();
            if(projectile.shooter != NetworkedVariables.playerId && NetworkedVariables.connectedClients[NetworkedVariables.playerId].playerHealth - projectile.damage > 0) {
                TCPClient.callStack.Insert(0, "{\"type\":\"playerHit\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"playerId\":\"" + NetworkedVariables.playerId + "\", \"shooterId\":\"" + projectile.shooter + "\", \"damage\":\"" + projectile.damage + "\"}");
            }
            if(projectile.shooter != NetworkedVariables.playerId && NetworkedVariables.connectedClients[NetworkedVariables.playerId].playerHealth - projectile.damage <= 0) {
                TCPClient.callStack.Insert(0, "{\"type\":\"playerDied\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"playerId\":\"" + NetworkedVariables.playerId + "\", \"shooterId\":\"" + projectile.shooter + "\", \"isSuicide\":\"false\"}");
            }
        }

        if(other.gameObject.tag == "Ground" && !NetworkedVariables.connectedClients[NetworkedVariables.playerId].isDead) {
            int damage = Mathf.CeilToInt(PrefabOrganizer.Planes[NetworkedVariables.connectedClients[NetworkedVariables.playerId].getCurrentType()].startHealth / 2);
            if(currentGroundDamageCooldown <= 0 && NetworkedVariables.connectedClients[NetworkedVariables.playerId].playerHealth - damage > 0) {
                TCPClient.callStack.Insert(0, "{\"type\":\"playerHit\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"playerId\":\"" + NetworkedVariables.playerId + "\", \"shooterId\":\"" + NetworkedVariables.playerId + "\", \"damage\":\"" + damage + "\"}");
                currentGroundDamageCooldown = groundDamageCooldown;
            } else if(currentGroundDamageCooldown <= 0 && NetworkedVariables.connectedClients[NetworkedVariables.playerId].playerHealth - damage <= 0) {
                TCPClient.callStack.Insert(0, "{\"type\":\"playerDied\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"playerId\":\"" + NetworkedVariables.playerId + "\", \"shooterId\":\"" + NetworkedVariables.playerId + "\", \"isSuicide\":\"true\"}");
                currentGroundDamageCooldown = groundDamageCooldown;
            }
        }
    }

    private void OnCollisionStay(Collision other) {
        if(other.gameObject.tag == "Ground" && !NetworkedVariables.connectedClients[NetworkedVariables.playerId].isDead) {
            int damage = Mathf.CeilToInt(PrefabOrganizer.Planes[NetworkedVariables.connectedClients[NetworkedVariables.playerId].getCurrentType()].startHealth / 2);
            if(currentGroundDamageCooldown <= 0 && NetworkedVariables.connectedClients[NetworkedVariables.playerId].playerHealth - damage > 0) {
                TCPClient.callStack.Insert(0, "{\"type\":\"playerHit\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"playerId\":\"" + NetworkedVariables.playerId + "\", \"shooterId\":\"" + NetworkedVariables.playerId + "\", \"damage\":\"" + damage + "\"}");
                currentGroundDamageCooldown = groundDamageCooldown;
            } else if(currentGroundDamageCooldown <= 0 && NetworkedVariables.connectedClients[NetworkedVariables.playerId].playerHealth - damage <= 0) {
                TCPClient.callStack.Insert(0, "{\"type\":\"playerDied\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"playerId\":\"" + NetworkedVariables.playerId + "\", \"shooterId\":\"" + NetworkedVariables.playerId + "\", \"isSuicide\":\"true\"}");
                currentGroundDamageCooldown = groundDamageCooldown;
            }
        }
    }
}
