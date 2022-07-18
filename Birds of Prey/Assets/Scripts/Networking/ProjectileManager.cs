using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour {
    public static List<Dictionary<string, dynamic>> bulletInformations = new List<Dictionary<string, dynamic>>();
    public static List<Dictionary<string, dynamic>> rocketInformations = new List<Dictionary<string, dynamic>>();

    private PlayerManager playerManager;

    private void Start() {
        playerManager = this.gameObject.GetComponent<PlayerManager>();
    }

    private void Update() {
        if(bulletInformations.Count > 0) {
            //A bullet needs to be spawned
            lock(bulletInformations) {
                foreach(Dictionary<string, dynamic> bulletInformation in bulletInformations) {
                    BulletTypes bulletType = (BulletTypes)Enum.Parse(typeof(BulletTypes), bulletInformation["bulletType"]);
                    Vector3 spawnPosition = calculateBulletSpawnPosition(bulletInformation["shooter"], bulletInformation["gunIndex"]);
                    if(spawnPosition == Vector3.zero) {
                        continue;
                    }
                    GameObject spawnedBullet = Instantiate(PrefabOrganizer.Bullets[bulletType].bulletPrefab, spawnPosition, Quaternion.identity);
                    Bullet bullet = spawnedBullet.GetComponent<Bullet>();
                    bullet.angleToFireProjectile = bullet.listToVector3(bulletInformation["facingAngle"]);
                    bullet.originalVelocity = bullet.listToVector3(bulletInformation["velocity"]);
                    bullet.projectileType = bulletType;
                    bullet.shooter = bulletInformation["shooter"];
                }

                bulletInformations.Clear();
            }
        }

        if(rocketInformations.Count > 0) {
            lock(rocketInformations) {
                foreach(Dictionary<string, dynamic> rocketInformation in rocketInformations) {
                    RocketTypes rocketType = (RocketTypes)Enum.Parse(typeof(RocketTypes), rocketInformation["rocketType"]);
                    Vector3 spawnPosition = calculateRocketSpawnPosition(rocketInformation["shooter"], rocketInformation["gunIndex"]);
                    if(spawnPosition == Vector3.zero) {
                        continue;
                    }
                    GameObject spawnedRocket = Instantiate(PrefabOrganizer.Rockets[rocketType].rocketPrefab, spawnPosition, Quaternion.identity);
                    Rocket rocket = spawnedRocket.GetComponent<Rocket>();
                    rocket.playerManager = playerManager;
                    rocket.projectileType = rocketType;
                    rocket.angleToFireProjectile = rocket.listToVector3(rocketInformation["facingAngle"]);
                    rocket.originalVelocity = rocket.listToVector3(rocketInformation["velocity"]);
                    rocket.shooter = rocketInformation["shooter"];
                    rocket.targetId = rocketInformation["target"];
                }
                rocketInformations.Clear();
            }
        }
    }

    public static void shootBullet(string gunIndex, List<float> bulletAngle, List<float> velocity, string type, string shooter) {
        lock(bulletInformations) {
            Dictionary<string, dynamic> bulletInformation = new Dictionary<string, dynamic>();
            bulletInformation.Add("gunIndex", gunIndex);
            bulletInformation.Add("facingAngle", bulletAngle);
            bulletInformation.Add("velocity", velocity);
            bulletInformation.Add("bulletType", type);
            bulletInformation.Add("shooter", shooter);
            bulletInformations.Add(bulletInformation);
        }
    }

    public static void shootRocket(string gunIndex, List<float> facingAngle, List<float> velocity, string type, string shooter, string target) {
        lock(rocketInformations) {
            Dictionary<string, dynamic> rocketInformation = new Dictionary<string, dynamic>();
            rocketInformation.Add("gunIndex", gunIndex);
            rocketInformation.Add("facingAngle", facingAngle);
            rocketInformation.Add("velocity", velocity);
            rocketInformation.Add("rocketType", type);
            rocketInformation.Add("shooter", shooter);
            rocketInformation.Add("target", target);
            rocketInformations.Add(rocketInformation);
        }
    }

    private Vector3 calculateBulletSpawnPosition(string shooterId, string gunIndex) {
        if(!NetworkedVariables.connectedClients[shooterId].isDead) {
            GameObject par = playerManager.playerObjectFromId(shooterId);
            if(par != null) {
                Transform parent = playerManager.playerObjectFromId(shooterId).GetComponentsInChildren<Transform>()[(shooterId == NetworkedVariables.playerId) ? 1 : 0];
                Vector3 gunOffset = PrefabOrganizer.Planes[NetworkedVariables.connectedClients[shooterId].getCurrentType()].realPlayer.GetComponentInChildren<Shooter>().bulletSpawnPoints[int.Parse(gunIndex)].transform.localPosition;
                Vector3 spawnPosition = parent.transform.TransformPoint(gunOffset);
                return spawnPosition;
            }
            return Vector3.zero;
        } else {
            return Vector3.zero;
        }
    }

    private Vector3 calculateRocketSpawnPosition(string shooterId, string gunIndex) {
        if(!NetworkedVariables.connectedClients[shooterId].isDead) {
            GameObject par = playerManager.playerObjectFromId(shooterId);
            if(par != null) {
                Transform parent = playerManager.playerObjectFromId(shooterId).GetComponentsInChildren<Transform>()[(shooterId == NetworkedVariables.playerId) ? 1 : 0];
                Vector3 gunOffset = PrefabOrganizer.Planes[NetworkedVariables.connectedClients[shooterId].getCurrentType()].realPlayer.GetComponentInChildren<Shooter>().rocketSpawnPoints[int.Parse(gunIndex)].transform.localPosition;
                Vector3 spawnPosition = parent.transform.TransformPoint(gunOffset);
                return spawnPosition;
            }
            return Vector3.zero;
        } else {
            return Vector3.zero;
        }
    }
}
