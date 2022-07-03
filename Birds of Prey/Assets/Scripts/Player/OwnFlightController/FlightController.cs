using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightController : MonoBehaviour
{
  public float speed = .05f;
  float zRotation;
  public float zRotationLerpSpeed;
  public Rigidbody rb;

  // Use this for initialization
  void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  // Update is called once per frame
  void Update()
  {
    Debug.Log(Input.GetAxis("Mouse X"));
    if(Input.GetAxis("Mouse X") == 0){
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y,zRotationLerpSpeed),2f * Time.deltaTime);
    }
    else{
       zRotation = (Input.GetAxis("Mouse X") + transform.rotation.x) * 40;
    }
     float rotation;
     if(transform.rotation.eulerAngles.z > 180){
      rotation =360 - transform.rotation.eulerAngles.z;
     }
     else{
      rotation = -transform.rotation.eulerAngles.z;
     }
    Debug.Log(transform.rotation.eulerAngles.z);
    transform.Rotate(Input.GetAxis("Vertical") , Input.GetAxis("Horizontal"), -zRotation * Time.deltaTime);
    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + rotation / 40, transform.rotation.eulerAngles.z);
    transform.position += transform.forward * Time.deltaTime * speed;
    //rb.AddForce(this.gameObject.transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime);
    
  }
}