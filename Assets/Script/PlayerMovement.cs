using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 200f;   
    [SerializeField] float gravity =-9.81f;
    [SerializeField] float jumpHeight = 10;

    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    Camera mainCam;
    private Vector3 jumpVelocity ;
    private Vector3 moveDirection;
    private bool isGrounded = true;
    private float distanceToGround;
    private float turnspeed = 15;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCam=Camera.main;
        Cursor.visible=false;
        Cursor.lockState=CursorLockMode.Locked;
    }

    void Update()
    {
        JumpHandle();
        Move();

    }

    void JumpHandle(){
        distanceToGround= controller.height/2 +0.1f;
        isGrounded=Physics.CheckSphere(transform.position,distanceToGround,groundMask);

        if(isGrounded && jumpVelocity.y <0){  
            jumpVelocity.y = -2f;
        }

        if(isGrounded && Input.GetKeyDown(KeyCode.Space)){
            jumpVelocity.y = Mathf.Sqrt(jumpHeight*-2*gravity);
        }

        jumpVelocity.y+=gravity*Time.deltaTime;
        controller.Move(jumpVelocity*Time.deltaTime);
    }

    void Move(){
    //change player's head
        float yawCamera=mainCam.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.Euler(0,yawCamera,0)
        ,turnspeed*Time.deltaTime);
        
    //Player wasd movement 
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
       // moveDirection= new Vector3(x,0,z);
        //moveDirection*=speed;
        controller.Move(transform.forward*speed*vertical*Time.deltaTime);
        controller.Move(transform.right*speed*horizontal*Time.deltaTime);

    }
}
