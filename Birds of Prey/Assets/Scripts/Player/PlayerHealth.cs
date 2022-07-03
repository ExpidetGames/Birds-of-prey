using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    [HideInInspector] public string myId;
    [SerializeField] private int currentHealth;

    private void Update() {
        if(NetworkedVariables.connectedClients.ContainsKey(myId)) {
            currentHealth = NetworkedVariables.connectedClients[myId].playerHealth;
        }
    }

    public void takeDamage(int damage) {
        currentHealth -= damage;
    }

    public void setHealth(int newHealth) {
        currentHealth = newHealth;
    }

    public int getHealth() {
        return currentHealth;
    }
}
