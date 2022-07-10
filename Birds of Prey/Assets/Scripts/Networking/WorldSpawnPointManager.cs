using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(PlayerManager))]
public class WorldSpawnPointManager : MonoBehaviour {



    [Serializable]
    public class Spawnpoint {
        public string team;
        public Transform colliderHolderTransform;
        public BoxCollider boxCollider;

        public Vector3 getRandomPointInsideBox() {
            float randXPos = UnityEngine.Random.Range(colliderHolderTransform.position.x - (boxCollider.size.x / 2), colliderHolderTransform.position.x + (boxCollider.size.x / 2));
            float randYPos = UnityEngine.Random.Range(colliderHolderTransform.position.y - (boxCollider.size.y / 2), colliderHolderTransform.position.y + (boxCollider.size.y / 2));
            float randZPos = UnityEngine.Random.Range(colliderHolderTransform.position.z - (boxCollider.size.z / 2), colliderHolderTransform.position.z + (boxCollider.size.z / 2));
            return new Vector3(randXPos, randYPos, randZPos);
        }
    }

    [SerializeField] private List<Spawnpoint> _spawnPoints = new List<Spawnpoint>();
    public static Dictionary<string, Spawnpoint> spawnPoints = new Dictionary<string, Spawnpoint>();

    private void Start() {
        foreach(Spawnpoint spawn in _spawnPoints) {
            spawnPoints.Add(spawn.team, spawn);
        }
    }
}
