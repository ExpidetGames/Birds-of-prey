using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Threading;

//Does everything having to do with the PLayer: Updating Positions, disconnecting Players, updating Name Tags, updating Health.
public class PlayerManager : MonoBehaviour {
    private List<string> spawnedPlayerIds;
    private Dictionary<string, GameObject> allPlayers;
    private GameObject ownPlayer;
    private static GameObject instance;
    //{"type":"otherPlayerData", "names":{"69":"Buster Cherry","70":"Amanda Hump"}, "healthValues":{"69":"200","70":"100"}, "planeTypes":"{"69":"STANDARD_AIRCRAFT","70":"CRAPPY_AIRCRAFT"}"}
    void Awake() {
        DontDestroyOnLoad(this.gameObject);
        if(instance == null) {
            instance = this.gameObject;
        } else {
            Destroy(gameObject);
        }
        while(string.IsNullOrEmpty(NetworkedVariables.playerId)) {
            Thread.Sleep(10);
        }
        allPlayers = new Dictionary<string, GameObject>();
        spawnedPlayerIds = new List<string>();
    }

    private void Update() {
        //Spawning the own Player if it is null and the Player is back in the world
        if(ownPlayer == null && SceneManager.GetActiveScene().buildIndex == NetworkedVariables.worldIndex) {
            ownPlayer = Instantiate(PrefabOrganizer.Planes[NetworkedVariables.connectedClients[NetworkedVariables.playerId].planeType].realPlayer, new Vector3(transform.position.x, transform.position.y + 10, transform.position.z), Quaternion.identity);
            ownPlayer.GetComponent<PlayerHealth>().setHealth(NetworkedVariables.connectedClients[NetworkedVariables.playerId].playerHealth);
            ownPlayer.GetComponent<PlayerHealth>().myId = NetworkedVariables.playerId;
            ownPlayer.GetComponentInChildren<Plane>().thrust = PrefabOrganizer.Planes[NetworkedVariables.connectedClients[NetworkedVariables.playerId].planeType].thrust;
            ownPlayer.GetComponentInChildren<Plane>().turnTorque = PrefabOrganizer.Planes[NetworkedVariables.connectedClients[NetworkedVariables.playerId].planeType].turnTorque;
            spawnedPlayerIds.Add(NetworkedVariables.playerId);
        }
        updatePlayerTransforms();
    }

    public void updatePlayerTransforms() {
        Dictionary<string, List<List<float>>> playerTransforms = NetworkedVariables.allConnectedPlayerTransforms;
        //There is new player that needs to be spawned
        if(playerTransforms.Count > spawnedPlayerIds.Count) {
            lock(playerTransforms) {
                foreach(string newPlayerId in playerTransforms.Keys) {
                    if(!spawnedPlayerIds.Contains(newPlayerId) && !allPlayers.ContainsKey(newPlayerId) && SceneManager.GetActiveScene().buildIndex == NetworkedVariables.worldIndex) {
                        GameObject spawnedPlayer;
                        //Spawning the new Player Dummy Object
                        if(NetworkedVariables.connectedClients.ContainsKey(newPlayerId)) {
                            spawnedPlayer = Instantiate(PrefabOrganizer.Planes[NetworkedVariables.connectedClients[newPlayerId].planeType].playerDummy, transform.position, Quaternion.identity);
                        } else {
                            continue;
                        }
                        PlayerDummyScript dummy = spawnedPlayer.GetComponent<PlayerDummyScript>();
                        PlayerHealth dummyHealth = spawnedPlayer.GetComponent<PlayerHealth>();
                        if(dummy != null) {
                            dummy.setName(NetworkedVariables.connectedClients[newPlayerId].name);
                        }
                        if(dummyHealth != null) {
                            dummyHealth.setHealth(NetworkedVariables.connectedClients[newPlayerId].playerHealth);
                            dummyHealth.myId = newPlayerId;
                        }
                        //Adding the Player Object to all the Players to work with it later
                        allPlayers.Add(newPlayerId, spawnedPlayer);
                        //Setting the Id of that PlayerObject to "Spawned"
                        spawnedPlayerIds.Add(newPlayerId);
                    }
                }
            }
        }
        //A client died so it needs to be cleaned up
        if(NetworkedVariables.deadPlayers.Count() > 0) {
            foreach(Dictionary<string, string> deadPlayerInfo in NetworkedVariables.deadPlayers) {
                if(deadPlayerInfo["deactivated"] == "0") {
                    string deadPlayerId = deadPlayerInfo["deadPlayerId"];
                    if(deadPlayerId == NetworkedVariables.playerId) {
                        //Dead player side
                        NetworkedVariables.scenceToLoad.Add(1);
                    } else if(allPlayers.ContainsKey(deadPlayerId)) {
                        //Killers side
                        spawnedPlayerIds.Remove(deadPlayerId);
                        Destroy(allPlayers[deadPlayerId]);
                        allPlayers.Remove(deadPlayerId);
                    }
                    NetworkedVariables.allConnectedPlayerTransforms.Remove(deadPlayerId);
                    deadPlayerInfo["deactivated"] = "1";
                }
            }
        }
        //A client wants to respawn 
        if(NetworkedVariables.playersToRejoin.Count > 0) {
            foreach(Dictionary<string, string> respawningPlayerInfo in NetworkedVariables.playersToRejoin) {
                //The owned client wants to rejoin
                if(respawningPlayerInfo["id"] == NetworkedVariables.playerId) {
                    NetworkedVariables.isDead = false;
                    NetworkedVariables.scenceToLoad.Add(NetworkedVariables.worldIndex);
                    spawnedPlayerIds.Clear();
                    allPlayers.Clear();
                    ownPlayer = Instantiate(PrefabOrganizer.Planes[NetworkedVariables.connectedClients[NetworkedVariables.playerId].planeType].realPlayer, new Vector3(transform.position.x, transform.position.y + 10, transform.position.z), Quaternion.identity);
                    ownPlayer.GetComponent<PlayerHealth>().setHealth(NetworkedVariables.connectedClients[NetworkedVariables.playerId].playerHealth);
                    ownPlayer.GetComponent<PlayerHealth>().myId = NetworkedVariables.playerId;
                    //Disabling all audioListeners because own Player has an Audio Listener and this stupid error message is really annoying..
                    AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();
                    foreach(AudioListener audioListener in audioListeners) {
                        if(!audioListener.GetComponentInParent<MouseFlightController>()) {
                            audioListener.enabled = false;
                        }
                    }
                }
            }
            NetworkedVariables.playersToRejoin.Clear();
            NetworkedVariables.deadPlayers.Clear();
        }
        //A client disconnected so it has to be destroyed
        if(NetworkedVariables.disconnectedPlayerIds.Count > 0) {
            foreach(string id in NetworkedVariables.disconnectedPlayerIds) {
                if(id == NetworkedVariables.playerId) {
                    spawnedPlayerIds.Clear();
                    allPlayers.Clear();
                    if(ownPlayer != null) {
                        ownPlayer.SetActive(false);
                    }
                }
                disconnectPlayer(id);
            }
            NetworkedVariables.disconnectedPlayerIds.Clear();
        }
        //Every Player is spawned so the position of them have to be updated
        if(playerTransforms.Count == spawnedPlayerIds.Count) {
            foreach(string playerId in allPlayers.Keys) {
                if(allPlayers[playerId] != null && playerTransforms.ContainsKey(playerId)) {
                    List<List<float>> currentPlayerInformation = playerTransforms[playerId];
                    //Updating Position
                    allPlayers[playerId].transform.position = Vector3.MoveTowards(allPlayers[playerId].transform.position, new Vector3(currentPlayerInformation[0][0], currentPlayerInformation[0][1], currentPlayerInformation[0][2]), Mathf.Infinity);
                    //Updating Rotation
                    allPlayers[playerId].transform.eulerAngles = new Vector3(currentPlayerInformation[1][0], currentPlayerInformation[1][1], currentPlayerInformation[1][2]);
                    //Updating Rotation of Name Tag so the name is always readable
                    PlayerDummyScript dummy = allPlayers[playerId].GetComponent<PlayerDummyScript>();
                    if(dummy != null && ownPlayer != null) {
                        dummy.rotateNameTowards();
                    }
                }
            }
        }
    }

    public GameObject playerObjectFromId(string playerId) {
        if(allPlayers != null && allPlayers.ContainsKey(playerId)) {
            return allPlayers[playerId];
        }
        return null;
    }

    public string idFromGameObject(GameObject playerObject) {
        for(int i = 0; i < allPlayers.Count; i++) {
            KeyValuePair<string, GameObject> currentCheck = allPlayers.ElementAt(i);
            if(currentCheck.Value == playerObject) {
                return currentCheck.Key;
            }
        }

        Debug.LogError($"The player Object was not found");
        return null;

    }

    public void shootRocket(GameObject target, Vector3 startPoint, Vector3 planeFacingDirection, Vector3 planeVelocity, RocketTypes rocketType) {
        string targetId = idFromGameObject(target);
        if(!string.IsNullOrEmpty(targetId)) {
            string facingDirection = "[\"" + planeFacingDirection.x.ToString().Replace(",", ".") + "\",\"" + planeFacingDirection.y.ToString().Replace(",", ".") + "\", \"" + planeFacingDirection.z.ToString().Replace(",", ".") + "\"]";
            string startPosition = "[\"" + startPoint.x.ToString().Replace(",", ".") + "\",\"" + startPoint.y.ToString().Replace(",", ".") + "\", \"" + startPoint.z.ToString().Replace(",", ".") + "\"]";
            string velocity = "[\"" + planeVelocity.x.ToString().Replace(",", ".") + "\",\"" + planeVelocity.y.ToString().Replace(",", ".") + "\", \"" + planeVelocity.z.ToString().Replace(",", ".") + "\"]";
            TCPClient.callStack.Insert(0, "{\"type\":\"shootRocketRequest\", \"roomId\":\"" + NetworkedVariables.roomId + "\", \"shooter\":\"" + NetworkedVariables.playerId + "\", \"rocketType\":\"" + rocketType + "\", \"target\":\"" + targetId + "\", \"bulletStartPosition\": " + startPosition + ", \"planeFacingDirection\": " + facingDirection + ", \"velocity\": " + velocity + "}");
        }
    }

    public void shootBullet(Vector3 startPoint, Vector3 planeFacingDirection, Vector3 planeVelocity, BulletTypes bulletType) {
        string facingDirection = "[\"" + planeFacingDirection.x.ToString().Replace(",", ".") + "\",\"" + planeFacingDirection.y.ToString().Replace(",", ".") + "\", \"" + planeFacingDirection.z.ToString().Replace(",", ".") + "\"]";
        string startPosition = "[\"" + startPoint.x.ToString().Replace(",", ".") + "\",\"" + startPoint.y.ToString().Replace(",", ".") + "\", \"" + startPoint.z.ToString().Replace(",", ".") + "\"]";
        string velocity = "[\"" + planeVelocity.x.ToString().Replace(",", ".") + "\",\"" + planeVelocity.y.ToString().Replace(",", ".") + "\", \"" + planeVelocity.z.ToString().Replace(",", ".") + "\"]";
        TCPClient.callStack.Insert(0, "{\"type\":\"shootBulletRequest\", \"roomId\":\"" + NetworkedVariables.roomId + "\",\"shooter\":\"" + NetworkedVariables.playerId + "\", \"bulletType\":\"" + bulletType + "\", \"bulletStartPosition\": " + startPosition + ",\"planeFacingDirection\": " + facingDirection + ", \"velocity\": " + velocity + "}");
    }

    public void disconnectPlayer(string playerId) {
        if(allPlayers.ContainsKey(playerId) && spawnedPlayerIds.Contains(playerId)) {
            //Deleting the GameObject out of the scene
            Destroy(allPlayers[playerId]);
            //Removing it from the allPlayer Dict
            allPlayers.Remove(playerId);
            //Removing the ID from the spawned Ids
            spawnedPlayerIds.Remove(playerId);
            //
            NetworkedVariables.allConnectedPlayerTransforms.Remove(playerId);
            NetworkedVariables.connectedClients.Remove(playerId);
        }
    }
}
