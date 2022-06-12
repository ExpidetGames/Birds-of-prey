using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holds Variables that need to be Networked and those, who have to be accessible in the other scripts
public static class NetworkedVariables
{
    //This class holds a bunch of Variables which should be networked

    //The transform of the owned Player
    public static Transform ownedPlayerTransform;
    //I dont even wanna talk about how long it took me to figure that you have to initialize an object with new befor you can add something
    public static Dictionary<string, List<List<float>>> allConnectedPlayerTransforms = new Dictionary<string, List<List<float>>>();
    //Holds the Player names that are currently in the lobby {id: PlayerName, id: PlayerName...}
    public static Dictionary<string,string> playerNames = new Dictionary<string, string>();
    //Holds the plane Types of every Player
    public static Dictionary<string, PlaneTypes> planeTypes = new Dictionary<string, PlaneTypes>();
    //Holds the health of all connected Players
    public static Dictionary<string, int> playerHealths = new Dictionary<string, int>();
    //Every scene Index pushed in here is loaded
    public static List<int> scenceToLoad = new List<int>();
    //Set by the JSONParser when a client disconnects and checked by the Scene Updater to delete disconnected Clients
    public static List<string> disconnectedPlayerIds = new List<string>();
    //Holds the players who are currently dead/in limbo (The death scene) 
    public static List<Dictionary<string, string>> deadPlayers = new List<Dictionary<string, string>>();
    //Holds the players who want to rejoin the game
    public static List<Dictionary<string, string>> playersToRejoin = new List<Dictionary<string, string>>();
    //True when Player is in world false if he is in Main Menu
    public static bool inGame;
    //The playerId which identifies the player
    public static string playerId;
    //The room Id the player is currently in
    public static string roomId;
    //Set when the entered GameCode doesnt exist
    public static string joinErrorMessage;

}