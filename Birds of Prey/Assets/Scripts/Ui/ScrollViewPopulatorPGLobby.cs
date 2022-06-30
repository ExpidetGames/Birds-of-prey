using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewPopulatorPGLobby : MonoBehaviour {

    private Dictionary<string, GameObject> spawnedNames = new Dictionary<string, GameObject>();
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void addToList(string playerId, GameObject playerNameTag) {
        GameObject newPlayerTag = Instantiate(playerNameTag);
        newPlayerTag.transform.parent = this.gameObject.transform;
        spawnedNames.Add(playerId, newPlayerTag);
    }

    public void removeFromList(string playerId) {
        GameObject objectToDestroy = spawnedNames[playerId];
        Destroy(objectToDestroy);
        spawnedNames.Remove(playerId);
    }

    public void updateClient(string clientId, bool newReadyState) {
        Image namePlateReadyIndicator = spawnedNames[clientId].GetComponentInChildren<Image>();
        namePlateReadyIndicator.color = (newReadyState) ? Color.green : Color.red;
    }

}
