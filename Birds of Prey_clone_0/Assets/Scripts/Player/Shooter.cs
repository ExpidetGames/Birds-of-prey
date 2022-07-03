using System;
using UnityEngine;

public class Shooter : MonoBehaviour {
    private float timeBetweenBullets = 1f;
    private float timeBetweenRockets = 5f;
    [SerializeField] private float radarDistance = 10f;
    [SerializeField] private float timeToLockTarget = 5f;
    [SerializeField] private float lockDuration = 5f;
    [SerializeField] private GameObject[] bulletSpawnPoints;
    [SerializeField] private GameObject[] rocketSpawnPoints;
    [SerializeField] private GameObject plane;
    [SerializeField] private GameObject radarPoint;

    private GameObject currentLock;
    private GameObject lastLock;
    private GameObject currentTarget;
    private Rigidbody planeRb;
    private PlayerManager playerManager;
    private float timePastSinceLastBullet = 0f;
    private float timePastSinceLastRocket = 0f;
    private float timeUntilLock;
    private float timeUntilUnlock;

    private void Start() {
        timeBetweenBullets = PrefabOrganizer.Planes[NetworkedVariables.connectedClients[NetworkedVariables.playerId].planeType].timeBetweenBullets;
        timeBetweenRockets = PrefabOrganizer.Planes[NetworkedVariables.connectedClients[NetworkedVariables.playerId].planeType].timeBetweenRockets;
        planeRb = plane.GetComponentInChildren<Rigidbody>();
        timeUntilLock = timeToLockTarget;
        playerManager = GameObject.Find("NetworkComponents").GetComponent<PlayerManager>();
    }

    void Update() {
        //BULLETS
        if(Input.GetMouseButton(0) && timePastSinceLastBullet >= timeBetweenBullets) {
            foreach(GameObject gun in bulletSpawnPoints) {
                Vector3 planeFacingDirection = plane.transform.TransformDirection(Vector3.forward);
                playerManager.shootBullet(gun.transform.position, planeFacingDirection, planeRb.velocity, PrefabOrganizer.Planes[NetworkedVariables.connectedClients[NetworkedVariables.playerId].planeType].bulletAmuniton);
            }
            timePastSinceLastBullet = 0f;
        }
        if(timePastSinceLastBullet < timeBetweenBullets) {
            timePastSinceLastBullet += Time.deltaTime;
        }

        //ROCKETS
        if(Input.GetKeyDown(KeyCode.Q)) {
            accquireTarget();
        }

        if(timePastSinceLastRocket < timeBetweenRockets) {
            timePastSinceLastRocket += Time.deltaTime;
        }

        if(timeUntilUnlock > 0) {
            //The time until unlock is decreased with every frame, so no endless locks can appear
            timeUntilUnlock -= Time.deltaTime;
            if(timeUntilUnlock <= 0) {
                //If its time to unlock, the Target and time Until Lock are being reset
                currentTarget = null;
                timeUntilLock = 0;
            }
        }

        if(currentTarget != null && currentTarget.activeSelf) {
            //If a lock is accquired and the right mouse button is clicket rockets are ready to fire and the current Target is not null
            if(timePastSinceLastRocket >= timeBetweenRockets) {
                if(Input.GetMouseButton(1) && timeUntilUnlock > 0) {
                    foreach(GameObject rocketSpawner in rocketSpawnPoints) {
                        Vector3 planeFacingDirection = plane.transform.TransformDirection(Vector3.forward);
                        playerManager.shootRocket(currentTarget, rocketSpawner.transform.position, planeFacingDirection, planeRb.velocity, PrefabOrganizer.Planes[NetworkedVariables.connectedClients[NetworkedVariables.playerId].planeType].rocketAmunition);
                    }
                    timePastSinceLastRocket = 0f;
                }
            }
        } else {
            currentTarget = null;
        }

    }
    private void accquireTarget() {
        //Debug.DrawRay(radarPoint.transform.position, plane.transform.TransformDirection(Vector3.forward) * radarDistance, Color.blue);
        RaycastHit hit;
        Physics.SphereCast(radarPoint.transform.position, 5, plane.transform.TransformDirection(Vector3.forward), out hit, radarDistance);
        if(hit.distance > 0) {
            //A target is hit
            currentLock = hit.collider.gameObject;
            //Checking if the hit target is the same as the frame before
            if(currentLock == lastLock && timeUntilLock <= timeToLockTarget) {
                //If it is the Time Until the Lock is accquired gets increased
                timeUntilLock += Time.deltaTime;
            } else {
                //If its not the time gets reset
                timeUntilLock = 0;
            }

            // The player was good enough to consistently hold the lock on the target so the lock is accquired (Poor guy)
            if(timeUntilLock >= timeToLockTarget) {
                String targetId = playerManager.idFromGameObject(currentLock);
                //and the player is a real player (not the ground or sth else)
                if(targetId != null) {
                    currentTarget = currentLock;
                    timeUntilUnlock = lockDuration;
                    //Informing the other Players about the lock
                    TCPClient.callStack.Insert(0, "{\"type\": \"targetLocked\", \"roomId\":\"" + NetworkedVariables.roomId + "\", \"shooter\":\"" + NetworkedVariables.playerId + "\", \"target\":\"" + targetId + "\"}");
                }
            }
            lastLock = currentLock;
        } else {
            //If nothing is hit, the time until the lock is accquired get reset
            timeUntilLock = 0;
        }
    }
}
