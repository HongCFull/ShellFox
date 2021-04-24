using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    [SerializeField][Range(0,-90)] float minAngle =-45;
    [SerializeField][Range(0,90)]  float maxAngle=45;

    public float RotationSpeed = 100;
    public Transform Target, Player;
    float mouseX,mouseY;
    public bool isUsingMouseToControll =true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible=false;
        Cursor.lockState= CursorLockMode.Locked;    //lock mouse into the center of the screen

    }

    void OnEnable(){
        SceneManager.sceneLoaded += ResetMouseRotation;
    }

    void ResetMouseRotation(Scene scene, LoadSceneMode mode){
        mouseX=0f;
        mouseY=0f;
    }

    void OnDisable(){
        SceneManager.sceneLoaded -= ResetMouseRotation;
    }

    // Update is called once per frame
    void LateUpdate()       //camera should be update after the player movement is updated 
    {
        CameraControl();
    }

    void CameraControl(){
        if(!isUsingMouseToControll) return;
        
        mouseX +=Input.GetAxis("Mouse X") *RotationSpeed;
        mouseY +=Input.GetAxis("Mouse Y") *RotationSpeed;
        mouseY = Mathf.Clamp(mouseY,minAngle,maxAngle);         //restrict the mouseY (up/down dependent) to specified range 

        transform.LookAt(Target);
        Target.rotation= Quaternion.Euler(-mouseY,mouseX,0);    //allow the camera "look upward"
        Player.rotation = Quaternion.Euler(0,mouseX,0); //only allow the player to turns their 'head' on x-z plane

    }

    public void SetIsUsingMouseForCamera(){
        isUsingMouseToControll=true;
    }

    public void SetIsNotUsingMouseForCamera(){
        isUsingMouseToControll=false;
    }
}
