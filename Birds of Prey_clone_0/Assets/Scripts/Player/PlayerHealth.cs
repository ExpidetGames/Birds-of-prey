using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector] public string myId;
    [SerializeField] private int currentHealth;

    private void Update() {
        if(NetworkedVariables.playerHealths.ContainsKey(myId)){
            currentHealth = NetworkedVariables.playerHealths[myId];
        }
    }

    public void takeDamage(int damage){
        currentHealth -= damage;
    }

    public void setHealth(int newHealth){
        currentHealth = newHealth;
    }

    public int getHealth(){
        return currentHealth;
    }
}
