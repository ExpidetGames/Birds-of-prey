using UnityEngine;

public class Rocket : Projectile {
    [HideInInspector] public string targetId;

    private float turnSpeed;
    private float timeUntilRocketEnginesFire;
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

        this.transform.rotation = Quaternion.LookRotation(angleToFireProjectile);
        projectileRigidbody.velocity = originalVelocity;
    }


    private void Update() {
        if(Vector3.Distance(this.gameObject.transform.position, this.gameObject.transform.position) <= 10) {
            base.updateCollidersOfGameObject(base.ColliderObject, true);
        }
        if(timeUntilRocketEnginesFire <= 0) {
            Vector3 flightDirection = Vector3.forward;
            if(NetworkedVariables.allConnectedPlayerTransforms.ContainsKey(targetId)) {
                Vector3 targetRotation = (listToVector3(NetworkedVariables.allConnectedPlayerTransforms[targetId][0]) - this.transform.position).normalized;
                flightDirection = transform.TransformDirection(Vector3.forward);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(targetRotation), turnSpeed * Time.deltaTime);
            }
            projectileRigidbody.velocity = flightDirection * projectileSpeed;
            foreach(ParticleSystem particleSystem in particleSystems) {
                if(!particleSystem.isPlaying) {
                    particleSystem.Play();
                }
            }
        } else {
            timeUntilRocketEnginesFire -= Time.deltaTime;
        }
        base.OnUpdate();
    }
}
