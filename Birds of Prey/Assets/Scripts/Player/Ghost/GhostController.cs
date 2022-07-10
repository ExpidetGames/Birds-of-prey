using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour {

    [SerializeField] private Rigidbody movementRigidbody;
    [SerializeField] private Camera ghostCamera;
    [SerializeField] private float movementSpeed;

    InputMaster controls;

    private void Awake() {
        controls = new InputMaster();
        controls.Enable();
    }

    void Start() {

    }

    void Update() {
        Vector2 moveInput = controls.GhostControlls.Movement.ReadValue<Vector2>();
        Debug.Log(ghostCamera.transform.TransformDirection(Vector3.forward) * movementSpeed);
        movementRigidbody.AddForce(ghostCamera.transform.TransformDirection(Vector3.forward) * moveInput.y * movementSpeed * Time.deltaTime, ForceMode.VelocityChange);
    }

    private void OnDestroy() {
        controls.Disable();
    }
}
