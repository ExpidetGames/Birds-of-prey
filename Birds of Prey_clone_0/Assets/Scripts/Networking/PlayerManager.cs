using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Threading;

//Does everything having to do with the PLayer: Updating Positions, disconnecting Players, updating Name Tags, updating Health.
public class PlayerManager : MonoBehaviour
{
    private List<string> spawnedPlayerIds;
    private Dictionary<string, GameObject> allPlayers;
    private GameObject ownPlayer;
    private static GameObject instance;
    //{"type":"otherPlayerData", "names":{"69":"Buster Cherry","70":"Amanda Hump"}, "healthValues":{"69":"200","70":"100"}, "planeTypes":"{"69":"STANDARD_AIRCRAFT","70":"CRAPPY_AIRCRAFT"}"}
    void Awake(){
        DontDestroyOnLoad(this.gameObject);
        if(instance == null){
            instance = this.gameObject;
        }else{
            Destroy(gameObject);
        }
        while(string.IsNullOrEmpty(NetworkedVariables.playerId)){
            Thread.Sleep(10);
        }
        allPlayers = new Dictionary<string, GameObject>();
        spawnedPlayerIds = new List<string>();
        //Spawning Own Player
        ownPlayer = Instantiate(PrefabOrganizer.Planes[NetworkedVariables.planeTypes[NetworkedVariables.playerId]].realPlayer, new Vector3(transform.position.x, transform.position.y + 10, transform.position.z), Quaternion.identity);
        ownPlayer.GetComponent<PlayerHealth>().setHealth(NetworkedVariables.playerHealths[NetworkedVariables.playerId]);
        ownPlayer.GetComponent<PlayerHealth>().myId = NetworkedVariables.playerId;
        spawnedPlayerIds.Add(NetworkedVariables.playerId);
            
    }

    private void Update() {
        updatePlayerTransforms();
    }

    public void updatePlayerTransforms() {
        Dictionary<string, List<List<float>>> playerTransforms = NetworkedVariables.allConnectedPlayerTransforms;  
        
        //There is new player that needs to be spawned
        if(playerTransforms.Count > spawnedPlayerIds.Count){
            lock(playerTransforms){
                foreach(string newPlayerId in playerTransforms.Keys){
                    if(!spawnedPlayerIds.Contains(newPlayerId) && !allPlayers.ContainsKey(newPlayerId) && SceneManager.GetActiveScene().buildIndex == 1){
                        //Spawning the new Player Dummy Object
                        GameObject spawnedPlayer = Instantiate(PrefabOrganizer.Planes[NetworkedVariables.planeTypes[newPlayerId]].playerDummy, transform.position, Quaternion.identity);
                        PlayerDummyScript dummy = spawnedPlayer.GetComponent<PlayerDummyScript>();
                        PlayerHealth dummyHealth = spawnedPlayer.GetComponent<PlayerHealth>();
                        if(dummy != null){
                            dummy.setName(NetworkedVariables.playerNames[newPlayerId]);
                        }
                        if(dummyHealth != null){
                            dummyHealth.setHealth(NetworkedVariables.playerHealths[newPlayerId]);
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
        if(NetworkedVariables.deadPlayers.Count() > 0){
            foreach(Dictionary<string, string> deadPlayerInfo in NetworkedVariables.deadPlayers){
                if(deadPlayerInfo["deactivated"] == "0"){
                    string deadPlayerId = deadPlayerInfo["deadPlayerId"];
                    if(deadPlayerId == NetworkedVariables.playerId){
                        NetworkedVariables.scenceToLoad.Add(2);
                    }else{
                        spawnedPlayerIds.Remove(deadPlayerId);
                        allPlayers[deadPlayerId].SetActive(false);
                        allPlayers.Remove(deadPlayerId);
                    }
                    NetworkedVariables.allConnectedPlayerTransforms.Remove(deadPlayerId);
                    deadPlayerInfo["deactivated"] = "1";
                }
            }
        }
        //A client wants to respawn 
        if(NetworkedVariables.playersToRejoin.Count > 0){
            foreach(Dictionary<string, string> respawningPlayerInfo in NetworkedVariables.playersToRejoin){
                //The owned client wants to rejoin
                if(respawningPlayerInfo["id"] == NetworkedVariables.playerId){
                    NetworkedVariables.scenceToLoad.Add(1);
                    spawnedPlayerIds = new List<string>();
                    allPlayers = new Dictionary<string, GameObject>();
                    //Disabling all audioListeners because own Player has an Audio Listener and this stupid error message is really annoying..
                    AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();
                    foreach(AudioListener audioListener in audioListeners){
                        if(!audioListener.GetComponentInParent<MouseFlightController>()){
                            audioListener.enabled = false;
                        }
                    }
                    ownPlayer = Instantiate(PrefabOrganizer.Planes[NetworkedVariables.planeTypes[NetworkedVariables.playerId]].realPlayer, new Vector3(transform.position.x, transform.position.y + 10, transform.position.z), Quaternion.identity);
                    ownPlayer.GetComponent<PlayerHealth>().setHealth(NetworkedVariables.playerHealths[NetworkedVariables.playerId]);
                    ownPlayer.GetComponent<PlayerHealth>().myId = NetworkedVariables.playerId;
                    spawnedPlayerIds.Add(NetworkedVariables.playerId);
                }
            }
            NetworkedVariables.playersToRejoin.Clear();
            NetworkedVariables.deadPlayers.Clear();
        }
        //A client disconnected so it has to be destroyed
        if(NetworkedVariables.disconnectedPlayerIds.Count > 0){
            List<string> killedIds = new List<string>();
            foreach(string id in NetworkedVariables.disconnectedPlayerIds){
                disconnectPlayer(id);
                killedIds.Add(id);
            }
            foreach(string killedId in killedIds){
                NetworkedVariables.disconnectedPlayerIds.Remove(killedId);
            }
        }
        //Every Player is spawned so the position of them have to be updated
        if(playerTransforms.Count == spawnedPlayerIds.Count){
            //The players positions have to be updated
            foreach(string playerId in allPlayers.Keys){
                if(allPlayers[playerId] != null && playerTransforms.ContainsKey(playerId)){
                    List<List<float>> currentPlayerInformation = playerTransforms[playerId];
                    //Updating Position
                    allPlayers[playerId].transform.position = Vector3.MoveTowards(allPlayers[playerId].transform.position, new Vector3(currentPlayerInformation[0][0], currentPlayerInformation[0][1], currentPlayerInformation[0][2]), Mathf.Infinity);
                    //Updating Rotation
                    allPlayers[playerId].transform.eulerAngles = new Vector3(currentPlayerInformation[1][0], currentPlayerInformation[1][1], currentPlayerInformation[1][2]);
                    //Updating Size
                    allPlayers[playerId].transform.localScale = new Vector3(currentPlayerInformation[2][0], currentPlayerInformation[2][1], currentPlayerInformation[2][2]);
                    //Updating Rotation of Name Tag so the name is always readable TODO: Update so it rotates to the camera of the plane not to the plane itself
                    PlayerDummyScript dummy = allPlayers[playerId].GetComponent<PlayerDummyScript>();
                    if(dummy != null && ownPlayer != null){
                        dummy.rotateNameTowards(ownPlayer);
                    }
                }
            }
        }
    }

    private List<List<float>> clone2DList(List<List<float>> oldList){
        List<List<float>> newList = new List<List<float>>();
        for(int i = 0; i < oldList.Count; i++){
            List<float> inner = new List<float>();
            for(int j = 0; j < oldList[i].Count; j++){
                inner.Add(oldList[i][j]);
            }
            newList.Add(inner);
        }
        return newList;
    }

    public GameObject playerObjectFromId(string playerId){
        return allPlayers[playerId];
    }

    public string idFromGameObject(GameObject playerObject){
        for(int i = 0; i < allPlayers.Count; i++){
            KeyValuePair<string, GameObject> currentCheck = allPlayers.ElementAt(i);
            if(currentCheck.Value == playerObject){
                return currentCheck.Key;
            }
        }
        
        Debug.LogError($"The player Object was not found");
        return null;
        
    }

    public void shootRocket(GameObject target, Vector3 startPoint, Vector3 planeFacingDirection, Vector3 planeVelocity, RocketTypes rocketType){
        string targetId = idFromGameObject(target);
        if(!string.IsNullOrEmpty(targetId)){
            string facingDirection = "[\""+planeFacingDirection.x.ToString().Replace(",",".") +"\",\""+planeFacingDirection.y.ToString().Replace(",",".")+"\", \""+planeFacingDirection.z.ToString().Replace(",",".")+"\"]";
            string startPosition = "[\""+startPoint.x.ToString().Replace(",",".") +"\",\""+startPoint.y.ToString().Replace(",",".")+"\", \""+startPoint.z.ToString().Replace(",",".")+"\"]";
            string velocity = "[\""+planeVelocity.x.ToString().Replace(",",".") +"\",\""+planeVelocity.y.ToString().Replace(",",".")+"\", \""+planeVelocity.z.ToString().Replace(",",".")+"\"]";
            TCPClient.callStack.Insert(0, "{\"type\":\"shootRocketRequest\", \"roomId\":\""+NetworkedVariables.roomId+"\", \"shooter\":\""+NetworkedVariables.playerId+"\", \"rocketType\":\""+rocketType+"\", \"target\":\""+targetId+"\", \"bulletStartPosition\": "+startPosition+", \"planeFacingDirection\": "+facingDirection+", \"velocity\": "+velocity+"}");
        }
    }

    public void shootBullet(Vector3 startPoint, Vector3 planeFacingDirection, Vector3 planeVelocity, BulletTypes bulletType){
        string facingDirection = "[\""+planeFacingDirection.x.ToString().Replace(",",".") +"\",\""+planeFacingDirection.y.ToString().Replace(",",".")+"\", \""+planeFacingDirection.z.ToString().Replace(",",".")+"\"]";
        string startPosition = "[\""+startPoint.x.ToString().Replace(",",".") +"\",\""+startPoint.y.ToString().Replace(",",".")+"\", \""+startPoint.z.ToString().Replace(",",".")+"\"]";
        string velocity = "[\""+planeVelocity.x.ToString().Replace(",",".") +"\",\""+planeVelocity.y.ToString().Replace(",",".")+"\", \""+planeVelocity.z.ToString().Replace(",",".")+"\"]";
        TCPClient.callStack.Insert(0, "{\"type\":\"shootBulletRequest\", \"roomId\":\""+ NetworkedVariables.roomId +"\",\"shooter\":\""+ NetworkedVariables.playerId +"\", \"bulletType\":\""+ bulletType +"\", \"bulletStartPosition\": "+startPosition+",\"planeFacingDirection\": "+facingDirection+", \"velocity\": "+velocity+"}");
    }

    public void disconnectPlayer(string playerId){
        if(allPlayers.ContainsKey(playerId) && spawnedPlayerIds.Contains(playerId)){
            //Deleting the GameObject out of the scene
            Destroy(allPlayers[playerId]);
            //Removing it from the allPlayer Dict
            allPlayers.Remove(playerId);
            //Removing the ID from the spawned Ids
            spawnedPlayerIds.Remove(playerId);
        }
    }

    private string getStringFromList(List<List<float>> input){
       string outputString = "";
       string[] parameters = {"Position: ", " Rotation: ", " Scale: "};
       for(int i = 0; i < input.Count; i++){
           outputString += parameters[i];
           for(int j = 0; j < input[i].Count; j++){
               outputString += input[i][j].ToString() + ", ";
           }
       }
       return outputString;
   }
}