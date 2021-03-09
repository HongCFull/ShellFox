using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //for player wasd movement
    [SerializeField] float speed = 20f;   
    private CharacterController controller;

    //for camera perspective control
    Camera mainCam;
    private Vector3 moveDirection;
    private float turnspeed = 15;

    //for jump handling
    [SerializeField] float gravity =-9.81f;
    [SerializeField] float jumpHeight = 10;
    [SerializeField] private LayerMask groundMask;
    private Vector3 jumpVelocity ;
    private bool isGrounded = true;
    private float distanceToGround;

    void Start(){
        controller = GetComponent<CharacterController>();
       // SetCamera();
    }

    void Update(){
       // JumpHandle();
        Move();
    }

    void SetCamera(){
        mainCam=Camera.main;
        Cursor.visible=false;
        Cursor.lockState=CursorLockMode.Locked;
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
    //Player wasd movement 
    
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        controller.Move(transform.forward*speed*vertical*Time.deltaTime);
        controller.Move(transform.right*speed*horizontal*Time.deltaTime);

        JumpHandle();
    }
}
