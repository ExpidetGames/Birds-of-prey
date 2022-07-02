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
            newPlayerTag.GetComponentInChildren<TMP_Text>().SetText(newClient.name);
            newPlayerTag.GetComponentsInChildren<Image>()[1].color = (newClient.isReady) ? Color.green : Color.red;
            newPlayerTag.transform.SetParent(this.gameObject.transform);
            spawnedNames.Add(newClient.id, newPlayerTag);
        }
    }

    public void removeFromList(string playerId) {
        GameObject objectToDestroy = spawnedNames[playerId];
        Destroy(objectToDestroy);
        spawnedNames.Remove(playerId);
    }

    public void updateClient(Client client) {
        spawnedNames[client.id].GetComponentsInChildren<Image>()[1].color = (client.isReady) ? Color.green : Color.red;
    }

}
