using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollViewPopulatorPGLobby : MonoBehaviour {

    [SerializeField] private GameObject namePlate;

    private Dictionary<string, GameObject> spawnedNames = new Dictionary<string, GameObject>();


    public void addToList(Client newClient) {
        if(!spawnedNames.ContainsKey(newClient.id)) {
            GameObject newPlayerTag = Instantiate(namePlate);
            newPlayerTag.GetComponent<NamePlateController>().setName(newClient.name);
            spawnedNames[newClient.id].GetComponent<NamePlateController>().updateReadyState(newClient.isReady);
            newPlayerTag.transform.SetParent(this.gameObject.transform);
            spawnedNames.Add(newClient.id, newPlayerTag);
        }
    }

    public void removeFromList(Client clientToRemove) {
        GameObject objectToDestroy = spawnedNames[clientToRemove.id];
        Destroy(objectToDestroy);
        spawnedNames.Remove(clientToRemove.id);
    }

    public void updateClient(Client client) {
        spawnedNames[client.id].GetComponent<NamePlateController>().updateReadyState(client.isReady);
    }

}
