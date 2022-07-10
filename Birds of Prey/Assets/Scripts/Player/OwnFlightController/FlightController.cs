using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlightController : MonoBehaviour {

    InputMaster controls;
    [SerializeField] private float speed = .05f, barrelRollSpeed = 20f, mouseSensitivity = 20f, zRotationClamp = 70f, zRotationLerpSpeed = 20f,wasdSpeed = 20f;
    private float zRotation;
    private bool isLerpingBack;
    //public Rigidbody rb;

    // Use this for initialization
    void Awake() {
        controls = new InputMaster();
        controls.Enable();
    }
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    // Update is called once per frame
    void Update() {
        //if mouse isnt moved
        //and barrelRoll isnt called
        //and zRotation isnt 0
        //calculate the lerping of the rotation from the plane back to 0 on the z axis
        if(controls.Player.Mouse.ReadValue<float>() == 0 && transform.rotation.eulerAngles.z != 0 && controls.Player.BarrelRowl.ReadValue<float>() == 0) {
            zRotation = 0;
            isLerpingBack = true;
            Quaternion lerpRotation;
            Quaternion qa = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            lerpRotation = Quaternion.Lerp(transform.rotation, qa, zRotationLerpSpeed * Time.deltaTime);
            transform.rotation = lerpRotation.normalized;
        }

        //calculate zRotation with mouse and barrelRowl
        else {
            isLerpingBack = false;
            zRotation = controls.Player.Mouse.ReadValue<float>() * mouseSensitivity;
            zRotation = controls.Player.BarrelRowl.ReadValue<float>() * barrelRollSpeed * 10 + zRotation;

            //mouse rotation is clambed 
            if(controls.Player.BarrelRowl.ReadValue<float>() == 0) {
                zRotation = Mathf.Clamp(zRotation, -zRotationClamp, zRotationClamp);
            }
            zRotation = zRotation * Time.deltaTime;

        }

        //calculate value wich will be addet to the yRotation relativ to the zRotation from the mouse
        float yRotation = 0;
        if(controls.Player.BarrelRowl.ReadValue<float>() == 0 && !isLerpingBack) {
            if(transform.rotation.eulerAngles.z > 180) {
                yRotation = 360 - transform.rotation.eulerAngles.z;
            } else {
                yRotation = -transform.rotation.eulerAngles.z;
            }
        }

        //calculate the wasd controls
        Vector2 rotation = controls.Player.Rotate.ReadValue<Vector2>() * Time.deltaTime * wasdSpeed;

        //perform the calculated movements and rotations 
        transform.Rotate(rotation.x, rotation.y, -zRotation);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + yRotation / 40, transform.rotation.eulerAngles.z);
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void OnDestroy() {
        controls.Disable();
    }
}