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

    
    private Vector3 jumpVelocity ;
    private Vector3 moveDirection;
    private bool isGrounded = true;
    private float distanceToGround;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
    }

    private float dampAngle;    //for damping angle function
    void Move(){
        distanceToGround= controller.height/2 +0.1f;
        isGrounded=Physics.CheckSphere(transform.position,distanceToGround,groundMask);

        if(isGrounded && jumpVelocity.y <0){  
            jumpVelocity.y = -2f;
        }

        if(isGrounded && Input.GetKeyDown(KeyCode.Space)){
            jumpVelocity.y = Mathf.Sqrt(jumpHeight*-2*gravity);
        }

        float z = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");
        moveDirection= new Vector3(x,0,z);
        moveDirection*=speed;
        controller.Move(moveDirection*Time.deltaTime);

//change player's head
        float smoothTime =0.1f;
        if(moveDirection.magnitude>=0.1f){
            float targetAngle = Mathf.Atan2(moveDirection.x,moveDirection.z) *Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,targetAngle,ref dampAngle,smoothTime);
            transform.rotation = Quaternion.Euler(0,angle,0);
        }

        jumpVelocity.y+=gravity*Time.deltaTime;
        controller.Move(jumpVelocity*Time.deltaTime);
    }
}
