using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holds Variables that need to be Networked and those, who have to be accessible in the other scripts
public static class NetworkedVariables {
    //This class holds a bunch of Variables which should be networked

    //The transform of the owned Player
    public static Transform ownedPlayerTransform;
    //I dont even wanna talk about how long it took me to figure that you have to initialize an object with new befor you can add something
    public static Dictionary<string, List<List<float>>> allConnectedPlayerTransforms = new Dictionary<string, List<List<float>>>();
    //Every scene Index pushed in here is loaded
    public static List<int> scenceToLoad = new List<int>();
    //Set by the JSONParser when a client disconnects and checked by the Scene Updater to delete disconnected Clients
    public static List<string> disconnectedPlayerIds = new List<string>();
    //Contains all the clients and soon their information like id, name, team, health, planeType, transform, isDead and so on
    public static Dictionary<string, Client> connectedClients = new Dictionary<string, Client>();
    //Contains the information of a lock if a target is locked
    public static List<Dictionary<string, string>> targetLockInfo = new List<Dictionary<string, string>>();
    //Holds the players who are currently dead/in limbo (The death scene) 
    public static List<Dictionary<string, string>> deadPlayers = new List<Dictionary<string, string>>();
    //Holds the players who want to rejoin the game
    public static List<Dictionary<string, string>> playersToRejoin = new List<Dictionary<string, string>>();
    //Holds the information about the current game Mode
    public static GameModeTypes currentGameMode = GameModeTypes.OneVsOne;
    //Indicates if the client is the creator of the room or not
    public static bool isRoomCreator;
    //True when Player is in world false if he is in Main Menu
    public static bool inGame;
    //To avoid getting hit twice when the player is already dead
    public static bool isDead;
    //Stores the BuildIndex of the world the players are playing
    public static int worldIndex;
    //The playerId which identifies the player
    public static string playerId;
    //The room Id the player is currently in
    public static string roomId;
    //Set when the entered GameCode doesnt exist
    public static string errorMessage;

}
