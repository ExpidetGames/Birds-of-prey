using UnityEngine;
using TMPro;
using System;

//Just a simple Player Controller for testing

public class PlayerControllerScript : MonoBehaviour
{
    public bool isOwned;

    private Vector2 lookInput;
    private Vector3 moveInput;
    private Vector2 defaultPositionRange = new Vector2(-4, 4);
    private Controls controls; 
    private float xRot = 0;
    private float currentBulletDelay;
    //private float showHitTime = 0;
    private bool crouch;
    private bool shooting;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private Canvas nameTagCanvas;
    [SerializeField] private GameObject pointBlank;
    [SerializeField] private float bulletDelay;

    [Space]
    [SerializeField] private float speed;
    [SerializeField] private float sensitivity;

    
    private void Start() {
        controls = new Controls();

        controls.PlayerControls.MoveKeys.performed += ctx => {
            moveInput = new Vector3(ctx.ReadValue<Vector2>().x, moveInput.y, ctx.ReadValue<Vector2>().y);
        };
        controls.PlayerControls.MoveKeys.canceled += ctx => {
            moveInput = new Vector3(ctx.ReadValue<Vector2>().x, moveInput.y, ctx.ReadValue<Vector2>().y);
        };

        controls.PlayerControls.MouseDelta.performed += ctx => {
            lookInput = ctx.ReadValue<Vector2>();
        };
        controls.PlayerControls.MouseDelta.canceled += ctx => {
            lookInput = ctx.ReadValue<Vector2>();
        };

        controls.PlayerControls.Jump.performed += ctx => {
            jump();
        };

        controls.PlayerControls.Sneak.performed += ctx => {
            crouch = !crouch;
            sneak(crouch);
        };

        controls.PlayerControls.Shoot.performed += ctx => {
            shooting = true;
        };

        controls.PlayerControls.Shoot.canceled += ctx => {
            shooting = false;
        };

        if (isOwned){
            controls.Enable();
            currentBulletDelay = bulletDelay;
            //transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 10, Random.Range(defaultPositionRange.x, defaultPositionRange.y));
            NetworkedVariables.ownedPlayerTransform = transform;
        }else{
            this.gameObject.GetComponent<PlayerControllerScript>().enabled = false;
            this.gameObject.GetComponentInChildren<Camera>().enabled = false;
        }
    }

    private void Update() {
        updatePlayer();

        if(shooting && currentBulletDelay <= 0){
            shoot();
            currentBulletDelay = bulletDelay;
        }
        currentBulletDelay -= Time.deltaTime;

        if(NetworkedVariables.inGame){
            if(NetworkedVariables.ownedPlayerTransform != null){
                UDPClient.udpCallStack.Insert(0, "{\"type\":\"transformUpdate\",\"roomId\":\""+ NetworkedVariables.roomId +"\",\"playerId\":\""+ NetworkedVariables.playerId +"\", \"newTransform\":\""+ JsonParser.transformToJson(NetworkedVariables.ownedPlayerTransform)+"\"}");
            }
        }
    }

    private void updatePlayer(){
        movePlayer(moveInput);
        movePlayerCamera(lookInput);
    }

    private void movePlayer(Vector3 moveInput){
        Vector3 moveVector = transform.TransformDirection(moveInput) * speed;
        playerBody.velocity = new Vector3(moveVector.x, playerBody.velocity.y, moveVector.z);
    }

    private void movePlayerCamera(Vector2 lookInput) {
        lookInput *= .5f * .1f;
        xRot -= lookInput.y * sensitivity;
        xRot = Mathf.Clamp(xRot, -90, 90);
        transform.Rotate(0f, lookInput.x * sensitivity, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }

    private void jump(){
        if (Physics.Raycast(this.transform.position, Vector3.down, .5f)) {
            playerBody.AddForce(Vector3.up * 10, ForceMode.VelocityChange);
        }
        
    }

    private void shoot() {
        if(this.crouch){
            createBullet("crazyBullet");
        }else{
           createBullet("lazyBullet");
        }
    }

    private void createBullet(String type){
        int distance = 1000000;
        Vector3 cameraLookDirection = playerCamera.transform.TransformDirection(Vector3.forward);
        Vector3 bulletStartPoint = pointBlank.transform.position;
        Vector3 bulletEndPoint = playerCamera.transform.position + (cameraLookDirection * distance);
        TCPClient.callStack.Insert(0, "{\"type\":\"shootRequest\", \"roomId\":\""+ NetworkedVariables.roomId +"\",\"shooter\":\""+ NetworkedVariables.playerId +"\", \"bulletType\":\""+ type +"\", \"bulletStartPosition\":{\"x\":\""+bulletStartPoint.x.ToString().Replace(",",".") +"\",\"y\":\""+bulletStartPoint.y.ToString().Replace(",",".")+"\", \"z\":\""+bulletStartPoint.z.ToString().Replace(",",".")+"\"} ,\"bulletEndPosition\":{\"x\":\""+bulletEndPoint.x.ToString().Replace(",",".") +"\",\"y\":\""+bulletEndPoint.y.ToString().Replace(",",".")+"\", \"z\":\""+bulletEndPoint.z.ToString().Replace(",",".")+"\"}}");
    }

    private void sneak(bool sneak) {
        if (sneak) {
            this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y * .5f, this.transform.localScale.z);
        } else {
            this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y * 2, this.transform.localScale.z);
        }
    }

    public void setName(string name){
        nameDisplay.text = name;
    }  

    public void disableNameTag() {
        nameTagCanvas.enabled = false;
    }
    public void rotateNameTowards(GameObject objectToLookAt){
        nameTagCanvas.transform.LookAt(nameTagCanvas.transform.position + objectToLookAt.transform.rotation * Vector3.forward, objectToLookAt.transform.rotation * Vector3.up);
    } 

    private void OnDestroy() {
        controls.Disable();
    }

}
