using UnityEngine;

public class Bullet : Projectile {

    private void Start() {
        base.OnStart();

        BulletTypes bulletType = (BulletTypes)projectileType;

        projectileSpeed = PrefabOrganizer.Bullets[bulletType].projectileSpeed;
        damage = PrefabOrganizer.Bullets[bulletType].damage;
        despawnTime = PrefabOrganizer.Bullets[bulletType].despawnTime;
        timeUntilProjectileGetsLethal = PrefabOrganizer.Bullets[bulletType].timeUntilBulletGetsLethal;

        projectileRigidbody.velocity = originalVelocity;
        projectileRigidbody.AddForce(angleToFireProjectile * projectileSpeed, ForceMode.Impulse);

    }

    public void Update() {
        base.OnUpdate();
    }
}
