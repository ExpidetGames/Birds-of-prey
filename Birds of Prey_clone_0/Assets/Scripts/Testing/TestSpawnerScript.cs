using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawnerScript : MonoBehaviour {

    private BoxCollider boxCollider;
    private Transform colliderHolderTransform;
    [SerializeField] private GameObject objToSpawn;
    [SerializeField] private int amountToSpawn;

    // Start is called before the first frame update
    void Start() {
        boxCollider = this.GetComponent<BoxCollider>();
        colliderHolderTransform = this.transform;
        for(int i = 0; i < amountToSpawn; i++) {
            Instantiate(objToSpawn, getRandomPositionInsideCollider(), Quaternion.identity);
        }
    }

    private Vector3 getRandomPositionInsideCollider() {
        float randXPos = Random.Range(colliderHolderTransform.position.x - (boxCollider.size.x / 2), colliderHolderTransform.position.x + (boxCollider.size.x / 2));
        float randYPos = Random.Range(colliderHolderTransform.position.y - (boxCollider.size.y / 2), colliderHolderTransform.position.y + (boxCollider.size.y / 2));
        float randZPos = Random.Range(colliderHolderTransform.position.z - (boxCollider.size.z / 2), colliderHolderTransform.position.z + (boxCollider.size.z / 2));
        return new Vector3(randXPos, randYPos, randZPos);
    }
}
