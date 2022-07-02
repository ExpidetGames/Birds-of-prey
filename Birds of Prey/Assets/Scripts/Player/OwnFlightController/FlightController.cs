using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightController : MonoBehaviour
{
  public float speed = .05f;
  float xRotation;

  // Use this for initialization
  void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  // Update is called once per frame
  void Update()
  {

    xRotation = (Input.GetAxis("Mouse X") + transform.rotation.x) * 40;


    transform.Rotate(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), -xRotation * Time.deltaTime);
    
    transform.position += transform.forward * Time.deltaTime * speed;
  }
}