using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class ScrollViewPopulatorPGLobby : MonoBehaviour {

    [Serializable]
    public class ListHeaderInfo {
        public string teamName;
        public Sprite headerSprite;
    }

    [SerializeField] private GameObject namePlate;
    [SerializeField] private Image listHeaderImage;
    [SerializeField] private TMP_Text listHeaderText;
    [SerializeField] private List<ListHeaderInfo> _teamHeaders = new List<ListHeaderInfo>();


    [HideInInspector] public string teamColor;

    private Dictionary<string, GameObject> spawnedNames = new Dictionary<string, GameObject>();
    private Dictionary<string, ListHeaderInfo> teamHeaders = new Dictionary<string, ListHeaderInfo>();

    private void Start() {
        foreach(ListHeaderInfo header in _teamHeaders) {
            teamHeaders.Add(header.teamName, header);
        }
        listHeaderImage.sprite = teamHeaders[teamColor].headerSprite;
        listHeaderText.text = $"Team {teamColor}";
    }

    public void addToList(Client newClient) {
        if(!spawnedNames.ContainsKey(newClient.id)) {
            GameObject newPlayerTag = Instantiate(namePlate);
            newPlayerTag.GetComponent<NamePlateController>().setName(newClient.name);
            newPlayerTag.GetComponent<NamePlateController>().updateReadyState(newClient.isReady);
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
