using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject viewPoint;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float smoothSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, new Vector3(viewPoint.transform.position.x, viewPoint.transform.position.y + 2, viewPoint.transform.position.z -10), smoothSpeed);
        playerCamera.transform.LookAt(viewPoint.transform.position);
    }
}
