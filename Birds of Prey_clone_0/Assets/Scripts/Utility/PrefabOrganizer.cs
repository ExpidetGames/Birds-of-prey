using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabOrganizer : MonoBehaviour {

    [Serializable]
    public class Bullet {
        public BulletTypes bulletType;
        public GameObject bulletPrefab;
        public int damage;
        public float projectileSpeed;
        public float despawnTime;
        public float timeUntilBulletGetsLethal;
    }

    [Serializable]
    public class Rocket {
        public RocketTypes rocketType;
        public GameObject rocketPrefab;
        public int damage;
        public float turnSpeed;
        public float projectileSpeed;
        public float despawnTime;
        public float timeUntilRocketEnginesFire;
        public float timeUntilRocketGetsLethal;
    }

    [Serializable]
    public class Plane {
        public PlaneTypes playerType;
        public GameObject realPlayer;
        public GameObject playerDummy;
        public BulletTypes bulletAmuniton;
        public RocketTypes rocketAmunition;
        public int startHealth;
        public int thrust;
        public Vector3 turnTorque;
        public float timeBetweenRockets;
        public float timeBetweenBullets;
    }

    [SerializeField] private List<Bullet> _bullets = new List<Bullet>();
    public static Dictionary<BulletTypes, Bullet> Bullets = new Dictionary<BulletTypes, Bullet>();

    [SerializeField] private List<Rocket> _rockets = new List<Rocket>();
    public static Dictionary<RocketTypes, Rocket> Rockets = new Dictionary<RocketTypes, Rocket>();

    [SerializeField] private List<Plane> _planes = new List<Plane>();
    public static Dictionary<PlaneTypes, Plane> Planes = new Dictionary<PlaneTypes, Plane>();

    void Awake() {
        foreach(Bullet bullet in _bullets) {
            Bullets[bullet.bulletType] = bullet;
        }

        foreach(Rocket rocket in _rockets) {
            Rockets[rocket.rocketType] = rocket;
        }

        foreach(Plane player in _planes) {
            Planes[player.playerType] = player;
        }
    }

}

public enum BulletTypes {
    CRAZY_BULLET,
}

public enum RocketTypes {
    NORMAL_ROCKET,
    CRAZY_ROCKET,
    INSANE_ROCKET,
}

public enum PlaneTypes {
    STANDARD_AIRCRAFT,
    CRAPPY_AIRCRAFT
}