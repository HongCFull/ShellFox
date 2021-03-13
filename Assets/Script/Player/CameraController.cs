using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField][Range(0,-90)] float minAngle;
    [SerializeField][Range(0,90)]  float maxAngle;

    public float RotationSpeed = 1;
    public Transform Target, Player;
    float mouseX,mouseY;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible=false;
        Cursor.lockState= CursorLockMode.Locked;    //lock mouse into the center of the screen


    }

    // Update is called once per frame
    void LateUpdate()
    {
        CameraControl();
    }

    void CameraControl(){
        mouseX +=Input.GetAxis("Mouse X") *RotationSpeed;
        mouseY +=Input.GetAxis("Mouse Y") *RotationSpeed;
        mouseY = Mathf.Clamp(mouseY,minAngle,maxAngle);         //restrict the mouseY (up/down dependent) to specified range 

        transform.LookAt(Target);
        //Target.transform.position.x +=2.5;
        Target.rotation= Quaternion.Euler(-mouseY,mouseX,0);    //allow the camera "look upward"
        Player.rotation = Quaternion.Euler(0,mouseX,0); //only allow the player to turns their 'head' on x-z plane

    }
}
