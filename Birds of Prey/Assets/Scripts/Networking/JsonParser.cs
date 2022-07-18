using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//This class encodes to Json and decodes from it
public class JsonParser : MonoBehaviour {
    //Converts a given Transform into a String thats send to the Server
    public static string transformToJson(Transform input) {
        float[,] transformArr = new float[,] { { input.position.x, input.position.y, input.position.z }, { input.eulerAngles.x, input.eulerAngles.y, input.eulerAngles.z }, { input.localScale.x, input.localScale.y, input.localScale.z } };
        string jsonPosition = JsonConvert.SerializeObject(transformArr);
        return jsonPosition;
    }

    //Decodes a message and does something based on that
    public static void decodeJsonMessage(string message) {
        /*
        if(!message.Contains("updatePlayerTransform")) {
            Debug.Log(message);
        }
        */
        JObject decodedMessage = null;
        if(message != null) {
            decodedMessage = JObject.Parse(message);
        } else {
            return;
        }
        string messageType = (string)decodedMessage["type"];
        //Debug.Log(messageType);
        //The owned Player just joined and receives its Id
        if(messageType.Equals("setId")) {
            //Debug.Log($"Received Id: {(string) decodedMessage["newId"]}");
            NetworkedVariables.playerId = (string)decodedMessage["newId"];
        }
        if(messageType.Equals("clientConnected")) {
            PlaneTypes[] planeTypes = decodedMessage["PlaneTypes"].ToObject<PlaneTypes[]>();
            addClient((string)decodedMessage["Id"], (string)decodedMessage["Name"], (string)decodedMessage["Team"], (int)decodedMessage["PlayerHealth"], (bool)decodedMessage["IsReady"], planeTypes);
        }
        //The transform data of at least one client has changed so it has to be updated
        if(messageType.Equals("updatePlayerTransform")) {
            foreach(KeyValuePair<string, string> playerData in decodedMessage["allPlayerTransformDict"].ToObject<Dictionary<string, string>>()) {
                List<List<float>> transformData = stringListToFloatList(JsonConvert.DeserializeObject<List<List<string>>>(playerData.Value));
                lock(NetworkedVariables.allConnectedPlayerTransforms) {
                    //print("Udpating position of player: " + playerData.Key);
                    if(NetworkedVariables.allConnectedPlayerTransforms.ContainsKey(playerData.Key)) {
                        NetworkedVariables.allConnectedPlayerTransforms[playerData.Key] = transformData;
                    } else {
                        NetworkedVariables.allConnectedPlayerTransforms.Add(playerData.Key, transformData);
                    }
                }
            }
        }
        if(messageType.Equals("startGame")) {
            NetworkedVariables.scenceToLoad.Add(NetworkedVariables.worldIndex);
        }
        //A player or team has won
        if(messageType.Equals("GameOver")) {
            //Team if a whole Team has won. Single if one Person has won
            string winnerType = (string)decodedMessage["winnerType"];
            //For Example Blue (for teams) or 31 for singles
            string winner = (string)decodedMessage["winner"];
            //To ensure that the kill who won the game is accounted for
            string lastKill = (string)decodedMessage["lastKill"];

            NetworkedVariables.connectedClients[lastKill].deaths++;
            if(NetworkedVariables.connectedClients[NetworkedVariables.playerId].isDeadForever) {
                NetworkedVariables.scenceToLoad.Add(8);
            }
        }
        //A target got locket and the players get informed about this unfortunate event
        if(messageType.Equals("targetLocked")) {
            string lockedTargetId = (string)decodedMessage["target"];
            string shooter = (string)decodedMessage["shooter"];
            Debug.Log($"The player {lockedTargetId} was locked by the player {shooter}");
            NetworkedVariables.targetLockInfo.Add(new Dictionary<string, string> { ["shooter"] = shooter, ["target"] = lockedTargetId });
        }
        //A bullet was shot and the other clients are informed with this message
        if(messageType.Equals("bulletShot")) {
            string bulletType = (string)decodedMessage["bulletType"];
            string shooter = (string)decodedMessage["shooter"];
            string gunIndex = (string)decodedMessage["gunIndex"];
            List<float> angle = new List<float>();
            List<float> velocity = new List<float>();
            //Retrieving the velocity of the plane
            foreach(string position in decodedMessage["velocity"].ToObject<List<string>>()) {
                velocity.Add(float.Parse(position, CultureInfo.InvariantCulture.NumberFormat));
            }

            //Retrieving the facing Direction of the plane
            foreach(string position in decodedMessage["planeFacingDirection"].ToObject<List<string>>()) {
                angle.Add(float.Parse(position, CultureInfo.InvariantCulture.NumberFormat));
            }
            ProjectileManager.shootBullet(gunIndex, angle, velocity, bulletType, shooter);
        }
        if(messageType.Equals("rocketShot")) {
            string rocketType = (string)decodedMessage["rocketType"];
            string shooter = (string)decodedMessage["shooter"];
            string target = (string)decodedMessage["targetId"];
            string gunName = (string)decodedMessage["gunName"];
            List<float> facingAngle = new List<float>();
            List<float> velocity = new List<float>();
            //Retrieving the facing Direction of the shooter
            foreach(string position in decodedMessage["facingAngle"].ToObject<List<string>>()) {
                facingAngle.Add(float.Parse(position, CultureInfo.InvariantCulture.NumberFormat));
            }
            //Retrieving the velocity of the shooter
            foreach(string position in decodedMessage["velocity"].ToObject<List<string>>()) {
                velocity.Add(float.Parse(position, CultureInfo.InvariantCulture.NumberFormat));
            }
            ProjectileManager.shootRocket(gunName: gunName, facingAngle: facingAngle, velocity: velocity, type: rocketType, shooter: shooter, target: target);
        }
        if(messageType.Equals("playerHit")) {
            int newHealth = (int)decodedMessage["newHealth"];
            string hitPlayer = (string)decodedMessage["hitPlayerId"];
            if(NetworkedVariables.connectedClients[hitPlayer].playerHealth > 0) {
                NetworkedVariables.connectedClients[hitPlayer].playerHealth = newHealth;
            }
        }
        if(messageType.Equals("playerDied")) {
            string deadPlayer = (string)decodedMessage["deadPlayer"];
            string killer = (string)decodedMessage["killer"];
            if(!NetworkedVariables.connectedClients[deadPlayer].isDead && !NetworkedVariables.connectedClients[deadPlayer].isDeadForever) {
                if(deadPlayer != killer) {
                    //If Equal Somebody killed himself by flying against the ground. Thats not a kill is it
                    NetworkedVariables.connectedClients[killer].kills++;
                }
                NetworkedVariables.connectedClients[deadPlayer].deaths++;
                if(NetworkedVariables.connectedClients[deadPlayer].deaths == NetworkedVariables.connectedClients[deadPlayer].planeTypes.Length) {
                    NetworkedVariables.connectedClients[deadPlayer].isDeadForever = true;
                    if(deadPlayer == NetworkedVariables.playerId) {
                        NetworkedVariables.scenceToLoad.Add(8);
                        return;
                    }
                }
                Dictionary<string, string> deadPlayerInfo = new Dictionary<string, string>() { ["deadPlayerId"] = deadPlayer, ["killer"] = killer, ["deactivated"] = "0" };
                NetworkedVariables.connectedClients[deadPlayer].currentPlaneType += 1;
                NetworkedVariables.connectedClients[deadPlayer].isDead = true;
                NetworkedVariables.deadPlayers.Add(deadPlayerInfo);
            }


        }
        if(messageType.Equals("rejoin")) {
            string playerId = (string)decodedMessage["playerId"];
            int newHealth = (int)decodedMessage["newHealth"];
            NetworkedVariables.connectedClients[playerId].playerHealth = newHealth;
            Dictionary<string, string> respawningPlayerInfo = new Dictionary<string, string> { ["id"] = playerId };
            NetworkedVariables.connectedClients[playerId].isDead = false;
            NetworkedVariables.playersToRejoin.Add(respawningPlayerInfo);
        }
        if(messageType.Equals("transferOwnership")) {
            string newOwner = (string)decodedMessage["newOwner"];
            if(newOwner == NetworkedVariables.playerId) {
                NetworkedVariables.isRoomCreator = true;
            } else if(NetworkedVariables.isRoomCreator) {
                NetworkedVariables.isRoomCreator = false;
            }
        }
        //Another client disconnected
        if(messageType.Equals("clientDisconnected")) {
            string disconnectedId = (string)decodedMessage["Id"];
            if(disconnectedId == NetworkedVariables.playerId) {
                NetworkedVariables.inGame = false;
                NetworkedVariables.isRoomCreator = false;
                NetworkedVariables.connectedClients[disconnectedId].isReady = false;
                NetworkedVariables.scenceToLoad.Add(0);
                NetworkedVariables.roomId = "";
                NetworkedVariables.allConnectedPlayerTransforms.Clear();
                NetworkedVariables.connectedClients.Clear();
            }
            if(NetworkedVariables.allConnectedPlayerTransforms.ContainsKey(disconnectedId)) {
                NetworkedVariables.allConnectedPlayerTransforms.Remove(disconnectedId);
            }
            NetworkedVariables.disconnectedPlayerIds.Add(disconnectedId);
        }
        if(messageType.Equals("createdRoom")) {
            NetworkedVariables.roomId = ((string)decodedMessage["newRoomId"]);
            NetworkedVariables.worldIndex = (int)decodedMessage["sceneIndex"];
            NetworkedVariables.isRoomCreator = true;
            NetworkedVariables.inGame = true;
            NetworkedVariables.scenceToLoad.Add(2);
        }
        if(messageType.Equals("ready")) {
            string playerId = (string)decodedMessage["Id"];
            NetworkedVariables.connectedClients[playerId].isReady = true;
        }
        if(messageType.Equals("unready")) {
            string playerId = (string)decodedMessage["Id"];
            NetworkedVariables.connectedClients[playerId].isReady = false;
        }
        if(messageType.Equals("changeTeam")) {
            string playerToMove = (string)decodedMessage["playerId"];
            string newTeam = (string)decodedMessage["newTeam"];
            NetworkedVariables.connectedClients[playerToMove].teamColor = newTeam;
        }
        if(messageType.Equals("joinSuccess")) {
            //Debug.Log(message);
            NetworkedVariables.roomId = ((string)decodedMessage["newRoomId"]);
            NetworkedVariables.worldIndex = (int)decodedMessage["sceneIndex"];
            //Populating the clients List in Networked Variables with the other clients
            foreach(Dictionary<string, dynamic> client in decodedMessage["otherClients"].ToObject<List<Dictionary<string, dynamic>>>()) {
                //Debug.Log($"Found client with name: {client["Name"]} and id {client["Id"]} with the team {client["Team"]}");
                if(client["Id"] != NetworkedVariables.playerId) {
                    PlaneTypes[] planeTypes = client["PlaneTypes"].ToObject<PlaneTypes[]>();
                    addClient(client["Id"], client["Name"], client["Team"], (int)client["PlayerHealth"], client["IsReady"], planeTypes);
                }
            }
            NetworkedVariables.currentGameMode = (GameModeTypes)((int)decodedMessage["gameMode"]);
            NetworkedVariables.isRoomCreator = false;
            NetworkedVariables.inGame = true;
            NetworkedVariables.scenceToLoad.Add(2);
        }
        if(messageType.Equals("Error")) {
            NetworkedVariables.errorMessage = ((string)decodedMessage["value"]);
        }
    }

    private static void addClient(string id, string name, string team, int playerHealth, bool isReady, PlaneTypes[] planeTypes) {
        NetworkedVariables.connectedClients.Add(id, new Client(id: id, name: name, teamColor: team, playerHealth: playerHealth, isReady: isReady, isDead: false, planeTypes: planeTypes, startingPlaneType: 0));
    }

    public static List<List<float>> stringListToFloatList(List<List<string>> input) {
        List<List<float>> output = new List<List<float>>();
        for(int i = 0; i < input.Count; i += 1) {
            List<float> innerList = new List<float>();
            for(int j = 0; j < input[i].Count; j += 1) {
                innerList.Add(float.Parse(input[i][j].Trim(), CultureInfo.InvariantCulture.NumberFormat));
            }
            output.Add(innerList);
        }
        return output;
    }
}
