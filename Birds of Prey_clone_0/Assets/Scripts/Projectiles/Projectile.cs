using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour {
    [HideInInspector] public int damage;
    [HideInInspector] public float projectileSpeed;
    [HideInInspector] public float despawnTime;
    [HideInInspector] public float timeUntilProjectileGetsLethal;
    [HideInInspector] public string shooter;
    [HideInInspector] public Vector3 angleToFireProjectile;
    [HideInInspector] public Vector3 originalVelocity;


    [HideInInspector] public Enum projectileType;

    [SerializeField] public Rigidbody projectileRigidbody;
    [SerializeField] public GameObject ColliderObject;
    [SerializeField] public GameObject particleSystemHolder;

    public void OnStart() {
        updateCollidersOfGameObject(ColliderObject, false);
    }

    public void OnUpdate() {
        if(despawnTime <= 0) {
            destroyProjectile();
        } else {
            despawnTime -= Time.deltaTime;
        }

        if(timeUntilProjectileGetsLethal <= 0) {
            updateCollidersOfGameObject(ColliderObject, true);
        } else {
            timeUntilProjectileGetsLethal -= Time.deltaTime;
        }

    }

    public void updateCollidersOfGameObject(GameObject colliderGameObject, bool newState) {
        Collider[] colliders = colliderGameObject.GetComponentsInChildren<Collider>(true);
        foreach(Collider collider in colliders) {
            collider.enabled = newState;
        }
    }

    public void updateParticleSystemsOfGameObject(GameObject particleSystemHolder, bool start) {
        if(particleSystemHolder != null) {
            ParticleSystem[] particleSystems = particleSystemHolder.GetComponentsInChildren<ParticleSystem>();
            foreach(ParticleSystem ps in particleSystems) {
                if(start) {
                    ps.Play();
                } else {
                    ps.Stop();
                }
            }
        }
    }
    public void updateColorsOfGameObject(GameObject rendererGameObject, Color newColor) {
        Renderer[] renderers = rendererGameObject.GetComponentsInChildren<Renderer>(true);
        foreach(Renderer renderComponent in renderers) {
            renderComponent.material.SetColor("_Color", newColor);
        }
    }

    public Vector3 listToVector3(List<float> input) {
        return new Vector3(input[0], input[1], input[2]);
    }

    public void OnCollisionEnter(Collision other) {
        Debug.Log("Rocket collided with " + other.gameObject.name);
        if(other.gameObject.tag != "Projectile") {
            destroyProjectile();
        }
    }

    public void destroyProjectile() {
        Destroy(this.gameObject);
    }

}
