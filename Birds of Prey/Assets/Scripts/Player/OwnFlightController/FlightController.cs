using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightController : MonoBehaviour
{
  public float speed = .05f;
  float zRotation;
  public float rotationResetTinmer;
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

    if(Input.GetAxis("Mouse X") == 0){
      rotationResetTinmer = rotationResetTinmer + 1 * Time.deltaTime;
    }
    else{
       zRotation = (Input.GetAxis("Mouse X") + transform.rotation.x) * 40;
       rotationResetTinmer = 0;
    }
    // if(rotationResetTinmer >3){
    //   transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity,.2f);
    // }
   

    transform.Rotate(Input.GetAxis("Vertical") , Input.GetAxis("Horizontal"), -zRotation * Time.deltaTime);
    
    transform.position += transform.forward * Time.deltaTime * speed;
    //rb.AddForce(this.gameObject.transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime);
    
  }
}