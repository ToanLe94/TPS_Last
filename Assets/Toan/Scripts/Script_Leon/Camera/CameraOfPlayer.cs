using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOfPlayer : MonoBehaviour {

    public Transform playerCam, character, centerpoint;
    public float Offet_Y_centerpoint;

    public float mouseX, mouseY;
    public float mouseSensitivity=10f;

    float moveFB, moveLB;
    public float  moveSpeed=2f;

    public float zoom;
    public float zoomSpeed=2f;
    public float zoomMin = -1f;
    public float zoomMax = -0.5f;

    public float rotationSpeed = 5f;
	// Use this for initialization
	void Start () {
        zoom = -3;
        mouseX = 180f;
    }

    // Update is called once per frame
    void Update()
    {

        zoom += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        if (zoom < zoomMin)
        {
            zoom = zoomMin;
        }
        if (zoom > zoomMax)
        {
            zoom = zoomMax;
        }
        playerCam.transform.localPosition = new Vector3(0, 0, zoom);
        //if (Input.GetKey(KeyCode.F) == true)
        //{

        //    mouseX += Input.GetAxis("Mouse X");
        //    mouseY += Input.GetAxis("Mouse Y");

        //}
        mouseX += Input.GetAxis("Mouse X") ;
        mouseY += Input.GetAxis("Mouse Y");
        mouseY = Mathf.Clamp(mouseY, -60f, 60f);


        centerpoint.localRotation = Quaternion.Euler(mouseY, mouseX, 0);
        playerCam.rotation = Quaternion.LookRotation(centerpoint.position - playerCam.position);
        //Debug.Log("centerpoint: " + centerpoint.eulerAngles);
        centerpoint.position = new Vector3(character.position.x, character.position.y + Offet_Y_centerpoint, character.position.z);



        if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("Vertical") < 0)
        {
            //Quaternion turnAngle = Quaternion.Euler(0, centerpoint.eulerAngles.y, 0);
            //character.rotation = Quaternion.Slerp(character.rotation, turnAngle, Time.deltaTime * rotationSpeed);
           
            character.rotation = Quaternion.Euler(0, character.eulerAngles.y + Input.GetAxis("Mouse X") * 1.5f, 0);
        }

        

    }
}
