using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewPopulatorEndScreen : MonoBehaviour {

    [SerializeField] private GameObject playerInfo;

    // Start is called before the first frame update
    void Start() {
        Debug.Log("Script started");
        foreach(string clientId in NetworkedVariables.connectedClients.Keys) {
            GameObject currentPlayerInfo = Instantiate(playerInfo);
            RectTransform currentPlayerInfoTransforms = currentPlayerInfo.GetComponent<RectTransform>();
            currentPlayerInfo.transform.SetParent(this.gameObject.transform);
            currentPlayerInfoTransforms.localScale = new Vector3(1, 1, 1);
            currentPlayerInfo.GetComponent<PlayerInfoManager>().init(clientId);
        }
    }
}
