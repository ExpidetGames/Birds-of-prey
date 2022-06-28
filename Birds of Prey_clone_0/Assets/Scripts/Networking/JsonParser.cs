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
        //Debug.Log(message);
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
        //A player or team has won
        if(messageType.Equals("GameOver")) {
            //Team if a whole Team has won. Single if one Person has won
            string winnerType = (string)decodedMessage["winnerType"];
            //For Example Blue (for teams) or 31 for singles
            string winner = (string)decodedMessage["winner"];
            //To ensure that the kill who won the game is accounted for
            string lastKill = (string)decodedMessage["lastKill"];

            Debug.Log($"The game was won by the player {winner}");
        }
        //A target got locket and the players get informed about this unfortunate event
        if(messageType.Equals("targetLocked")) {
            //TCPClient.callStack.Insert(0, "{\"type\": \"targetLocked\", \"roomId\":\"" + NetworkedVariables.roomId + "\", \"shooter\":\"" + NetworkedVariables.playerId + "\", \"target\":\"" + targetId + "\"}");
            string lockedTargetId = (string)decodedMessage["target"];
            string shooter = (string)decodedMessage["shooter"];
            NetworkedVariables.targetLockInfo.Add(new Dictionary<string, string> { ["shooter"] = shooter, ["target"] = lockedTargetId });
        }
        //A bullet was shot and the other clients are informed with this message
        if(messageType.Equals("bulletShot")) {
            string bulletType = (string)decodedMessage["bulletType"];
            string shooter = (string)decodedMessage["shooter"];
            List<float> startPosition = new List<float>();
            List<float> angle = new List<float>();
            List<float> velocity = new List<float>();
            //Retrieving the start Position of the bullet
            foreach(string position in decodedMessage["startPos"].ToObject<List<string>>()) {
                startPosition.Add(float.Parse(position, CultureInfo.InvariantCulture.NumberFormat));
            }
            //Retrieving the velocity of the plane
            foreach(string position in decodedMessage["velocity"].ToObject<List<string>>()) {
                velocity.Add(float.Parse(position, CultureInfo.InvariantCulture.NumberFormat));
            }

            //Retrieving the facing Direction of the plane
            foreach(string position in decodedMessage["planeFacingDirection"].ToObject<List<string>>()) {
                angle.Add(float.Parse(position, CultureInfo.InvariantCulture.NumberFormat));
            }
            ProjectileManager.shootBullet(startPosition, angle, velocity, bulletType, shooter);
        }
        if(messageType.Equals("rocketShot")) {
            string rocketType = (string)decodedMessage["rocketType"];
            string shooter = (string)decodedMessage["shooter"];
            string target = (string)decodedMessage["targetId"];
            List<float> startPosition = new List<float>();
            List<float> facingAngle = new List<float>();
            List<float> velocity = new List<float>();
            //Retrieving the start Position of the rocket
            foreach(string position in decodedMessage["startPos"].ToObject<List<string>>()) {
                startPosition.Add(float.Parse(position, CultureInfo.InvariantCulture.NumberFormat));
            }
            //Retrieving the facing Direction of the shooter
            foreach(string position in decodedMessage["facingAngle"].ToObject<List<string>>()) {
                facingAngle.Add(float.Parse(position, CultureInfo.InvariantCulture.NumberFormat));
            }
            //Retrieving the velocity of the shooter
            foreach(string position in decodedMessage["velocity"].ToObject<List<string>>()) {
                velocity.Add(float.Parse(position, CultureInfo.InvariantCulture.NumberFormat));
            }
            ProjectileManager.shootRocket(startPosition: startPosition, facingAngle: facingAngle, velocity: velocity, type: rocketType, shooter: shooter, target: target);
        }
        if(messageType.Equals("playerHit")) {
            int newHealth = (int)decodedMessage["newHealth"];
            string hitPlayer = (string)decodedMessage["hitPlayerId"];
            if(NetworkedVariables.playerHealths[hitPlayer] > 0) {
                NetworkedVariables.playerHealths[hitPlayer] = newHealth;
            }
        }
        if(messageType.Equals("playerDied")) {
            string deadPlayer = (string)decodedMessage["deadPlayer"];
            string killer = (string)decodedMessage["killer"];
            Dictionary<string, string> deadPlayerInfo = new Dictionary<string, string>() { ["deadPlayerId"] = deadPlayer, ["killer"] = killer, ["deactivated"] = "0" };
            NetworkedVariables.deadPlayers.Add(deadPlayerInfo);
        }
        if(messageType.Equals("rejoin")) {
            string playerId = (string)decodedMessage["playerId"];
            int newHealth = (int)decodedMessage["newHealth"];
            NetworkedVariables.playerHealths[playerId] = newHealth;
            //Debug.Log($"The new health of player {playerId} is {NetworkedVariables.playerHealths[playerId]}");
            Dictionary<string, string> respawningPlayerInfo = new Dictionary<string, string> { ["id"] = playerId };
            NetworkedVariables.playersToRejoin.Add(respawningPlayerInfo);
        }
        //Another client disconnected
        if(messageType.Equals("clientDisconnected")) {
            string disconnectedId = (string)decodedMessage["Id"];
            if(disconnectedId == NetworkedVariables.playerId) {
                NetworkedVariables.inGame = false;
                NetworkedVariables.isRoomCreator = false;
                NetworkedVariables.scenceToLoad.Add(0);
                NetworkedVariables.roomId = "";
                NetworkedVariables.allConnectedPlayerTransforms = new Dictionary<string, List<List<float>>>();
                NetworkedVariables.playerHealths = new Dictionary<string, int>();
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
            NetworkedVariables.scenceToLoad.Add(4);
        }
        if(messageType.Equals("readyUp")) {

        }
        if(messageType.Equals("unready")) {

        }
        if(messageType.Equals("joinSuccess")) {
            NetworkedVariables.roomId = ((string)decodedMessage["newRoomId"]);
            NetworkedVariables.worldIndex = (int)decodedMessage["sceneIndex"];
            NetworkedVariables.currentGameMode = (GameModeTypes)((int)decodedMessage["gameMode"]);
            NetworkedVariables.isRoomCreator = false;
            NetworkedVariables.inGame = true;
            NetworkedVariables.scenceToLoad.Add(4);
        }
        if(messageType.Equals("otherPlayerData")) {
            foreach(KeyValuePair<string, string> playerName in decodedMessage["names"].ToObject<Dictionary<string, string>>()) {
                NetworkedVariables.playerNames[playerName.Key] = playerName.Value;
            }

            foreach(KeyValuePair<string, string> playerHealth in decodedMessage["healthValues"].ToObject<Dictionary<string, string>>()) {
                NetworkedVariables.playerHealths[playerHealth.Key] = int.Parse(playerHealth.Value);
                //Debug.Log($"Player {playerHealth.Key} has {playerHealth.Value} Health Points");
            }

            foreach(KeyValuePair<string, string> planeTypes in decodedMessage["planeTypes"].ToObject<Dictionary<string, string>>()) {
                NetworkedVariables.planeTypes[planeTypes.Key] = (PlaneTypes)Enum.Parse(typeof(PlaneTypes), planeTypes.Value);
                //Debug.Log($"Player {planeTypes.Key} flies the plane {(PlaneTypes) Enum.Parse(typeof(PlaneTypes), planeTypes.Value)}");
            }
        }
        if(messageType.Equals("Error")) {
            NetworkedVariables.errorMessage = ((string)decodedMessage["value"]);
        }
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
