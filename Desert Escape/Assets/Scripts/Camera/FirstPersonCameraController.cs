using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour
{
    public Transform PlayerOrientation;
    public float mouseSensitivity;
    float xRotation;
    float yRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float rotationCamX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;  
        float rotationCamY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= rotationCamY;

        yRotation += rotationCamX;

        xRotation = Mathf.Clamp(xRotation, -90f,90f);

        transform.rotation = Quaternion.Euler(xRotation,yRotation, 0);

        PlayerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
