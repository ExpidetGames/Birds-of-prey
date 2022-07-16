using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotationTesting : MonoBehaviour {

    public List<GameObject> spawnPoints = new List<GameObject>();
    private List<GameObject> spawnedGunPoints = new List<GameObject>();

    public GameObject gunVisualizer;
    public Transform parent;
    private Vector3 pos, fw, up;

    void Start() {
        foreach(GameObject spawnPoint in spawnPoints) {
            fw = parent.transform.InverseTransformDirection(transform.forward);
            up = parent.transform.InverseTransformDirection(transform.up);
            var newpos = parent.transform.TransformPoint(spawnPoint.transform.localPosition);
            var newfw = parent.transform.TransformDirection(fw);
            var newup = parent.transform.TransformDirection(up);
            var newrot = Quaternion.LookRotation(newfw, newup);
            spawnedGunPoints.Add(Instantiate(gunVisualizer, newpos, newrot));
        }
    }
    void Update() {

    }
}
