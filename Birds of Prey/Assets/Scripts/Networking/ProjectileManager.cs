using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{ 
    public static List<Dictionary<string, dynamic>> bulletInformations = new List<Dictionary<string, dynamic>>();
    public static List<Dictionary<string, dynamic>> rocketInformations = new List<Dictionary<string, dynamic>>();

    private void Update() {
        if(bulletInformations.Count > 0){
            //A bullet needs to be spawned
            lock(bulletInformations){
                foreach(Dictionary<string, dynamic> bulletInformation in bulletInformations){
                    BulletTypes bulletType = (BulletTypes) Enum.Parse(typeof(BulletTypes), bulletInformation["bulletType"]);
                    GameObject spawnedBullet = Instantiate(PrefabOrganizer.Bullets[bulletType].bulletPrefab, new Vector3(bulletInformation["start"][0], bulletInformation["start"][1], bulletInformation["start"][2]), Quaternion.identity);
                    Bullet bullet = spawnedBullet.GetComponent<Bullet>();
                    bullet.angleToFireProjectile = bullet.listToVector3(bulletInformation["facingAngle"]);
                    bullet.originalVelocity = bullet.listToVector3(bulletInformation["velocity"]);
                    bullet.projectileType = bulletType;
                    bullet.shooter = bulletInformation["shooter"];
                }

                bulletInformations.Clear();
            }
        }

        if(rocketInformations.Count > 0){
            lock(rocketInformations){
                foreach(Dictionary<string, dynamic> rocketInformation in rocketInformations){
                    RocketTypes rocketType = (RocketTypes) Enum.Parse(typeof(RocketTypes), rocketInformation["rocketType"]);
                    GameObject spawnedRocket = Instantiate(PrefabOrganizer.Rockets[rocketType].rocketPrefab, new Vector3(rocketInformation["start"][0], rocketInformation["start"][1], rocketInformation["start"][2]), Quaternion.identity);
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

    public static void shootBullet(List<float> bulletStart, List<float> bulletAngle, List<float> velocity, string type, string shooter){
        lock(bulletInformations){
            Dictionary<string, dynamic> bulletInformation = new Dictionary<string, dynamic>();
            bulletInformation.Add("start", bulletStart);
            bulletInformation.Add("facingAngle", bulletAngle);
            bulletInformation.Add("velocity", velocity);
            bulletInformation.Add("bulletType", type);
            bulletInformation.Add("shooter", shooter);
            bulletInformations.Add(bulletInformation);
        }
    }

    public static void shootRocket(List<float> startPosition, List<float> facingAngle, List<float> velocity, string type, string shooter, string target){
        lock(rocketInformations){
            //Debug.Log($"Player {shooter} shot player {target} with a rocket of type {type} from the position {startPosition}");
            Dictionary<string, dynamic> rocketInformation = new Dictionary<string, dynamic>();
            rocketInformation.Add("start", startPosition);
            rocketInformation.Add("facingAngle", facingAngle);
            rocketInformation.Add("velocity", velocity);
            rocketInformation.Add("rocketType", type);
            rocketInformation.Add("shooter", shooter);
            rocketInformation.Add("target", target);
            rocketInformations.Add(rocketInformation);
        }
    }
}
