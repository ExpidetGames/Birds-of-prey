using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour {
    public static List<Dictionary<string, dynamic>> bulletInformations = new List<Dictionary<string, dynamic>>();
    public static List<Dictionary<string, dynamic>> rocketInformations = new List<Dictionary<string, dynamic>>();

    public static Dictionary<string, GameObject> projectileSpawnPoints = new Dictionary<string, GameObject>();

    private void Update() {
        if(bulletInformations.Count > 0) {
            //A bullet needs to be spawned
            lock(bulletInformations) {
                foreach(Dictionary<string, dynamic> bulletInformation in bulletInformations) {
                    BulletTypes bulletType = (BulletTypes)Enum.Parse(typeof(BulletTypes), bulletInformation["bulletType"]);
                    GameObject spawnedBullet = Instantiate(PrefabOrganizer.Bullets[bulletType].bulletPrefab, projectileSpawnPoints[bulletInformation["gunName"]].transform.position, Quaternion.identity);
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
                    GameObject spawnedRocket = Instantiate(PrefabOrganizer.Rockets[rocketType].rocketPrefab, projectileSpawnPoints[rocketInformation["gunName"]].transform.position, Quaternion.identity);
                    Rocket rocket = spawnedRocket.GetComponent<Rocket>();
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

    public static void shootBullet(string gunName, List<float> bulletAngle, List<float> velocity, string type, string shooter) {
        lock(bulletInformations) {
            Dictionary<string, dynamic> bulletInformation = new Dictionary<string, dynamic>();
            bulletInformation.Add("gunName", gunName);
            bulletInformation.Add("facingAngle", bulletAngle);
            bulletInformation.Add("velocity", velocity);
            bulletInformation.Add("bulletType", type);
            bulletInformation.Add("shooter", shooter);
            bulletInformations.Add(bulletInformation);
        }
    }

    public static void shootRocket(string gunName, List<float> facingAngle, List<float> velocity, string type, string shooter, string target) {
        lock(rocketInformations) {
            Dictionary<string, dynamic> rocketInformation = new Dictionary<string, dynamic>();
            rocketInformation.Add("gunName", gunName);
            rocketInformation.Add("facingAngle", facingAngle);
            rocketInformation.Add("velocity", velocity);
            rocketInformation.Add("rocketType", type);
            rocketInformation.Add("shooter", shooter);
            rocketInformation.Add("target", target);
            rocketInformations.Add(rocketInformation);
        }
    }
}
