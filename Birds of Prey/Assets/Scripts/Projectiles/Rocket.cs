using UnityEngine;

public class Rocket : Projectile {
    [HideInInspector] public string targetId;
    [HideInInspector] public PlayerManager playerManager;

    private float turnSpeed;
    private float timeUntilRocketEnginesFire;
    private GameObject target;
    private ParticleSystem[] particleSystems;


    void Start() {
        base.OnStart();
        RocketTypes rocketType = (RocketTypes)projectileType;
        particleSystems = this.GetComponentsInChildren<ParticleSystem>();
        projectileSpeed = PrefabOrganizer.Rockets[rocketType].projectileSpeed;
        damage = PrefabOrganizer.Rockets[rocketType].damage;
        despawnTime = PrefabOrganizer.Rockets[rocketType].despawnTime;
        turnSpeed = PrefabOrganizer.Rockets[rocketType].turnSpeed;
        timeUntilRocketEnginesFire = PrefabOrganizer.Rockets[rocketType].timeUntilRocketEnginesFire;
        timeUntilProjectileGetsLethal = PrefabOrganizer.Rockets[rocketType].timeUntilRocketGetsLethal;
        target = playerManager.playerObjectFromId(targetId);

        this.transform.rotation = Quaternion.LookRotation(angleToFireProjectile);
        projectileRigidbody.velocity = originalVelocity;
    }


    private void Update() {
        if(timeUntilRocketEnginesFire <= 0) {
            transform.position += (target.transform.position - transform.position).normalized * projectileSpeed * Time.deltaTime;
            transform.LookAt(target.transform.position);
        } else {
            timeUntilRocketEnginesFire -= Time.deltaTime;
        }
        base.OnUpdate();
    }
}
