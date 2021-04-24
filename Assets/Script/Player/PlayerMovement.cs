using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public PlayerAttributes playerAttribute;
    
    //for player wasd movement
    CharacterController controller;
    public float movementSpeed;
    public float jumpSpeed;
    public float runMultiplier;
    Vector3 velocity;   //for process usage

    //for jump handling
    [SerializeField] float gravity ;
    [SerializeField] private LayerMask groundMask;
    bool headHitSomething=false;

    //for original version
    float orignialSpeed;
    float orignialRunMultiplier;
    float orignialGravity;
    float originalJumpSpeed;

//for initialization

    void RecordOriginalAttribute(){
        orignialSpeed = movementSpeed;
        orignialRunMultiplier=runMultiplier;
        orignialGravity=gravity;
        originalJumpSpeed = jumpSpeed;
    }

    void Start(){
        controller = GetComponent<CharacterController>();
        RecordOriginalAttribute();
    }

//runtime function
    void Update(){
        MovementVer2();
    }

    void OnControllerColliderHit(ControllerColliderHit other) { //called back when the character controller's "collider" hit something 
        if(Physics.Raycast(transform.position,Vector3.up,GetComponent<CharacterController>().height/2+0.1f))    //note that we only checked the center of the head(instead of the whole head)
            headHitSomething=true;
    }

    void UpdateMovementAttributeBySituation(){
        if(playerAttribute.inBattle){
            ApplyBattleMovementAttribute();
        }
        else{
            ApplyOriginalAttribute();
        }
    }

    void ApplyBattleMovementAttribute(){
        if (playerAttribute.canMove) {
            movementSpeed = playerAttribute.playerSpeed_inBattle;
            runMultiplier=playerAttribute.playerAccel_inBattle;
            jumpSpeed = playerAttribute.playerJumpSpeed_inBattle;

        } else {
            movementSpeed = 0f;
            runMultiplier=0f;
            jumpSpeed=0f; 
        }
    }

    void ApplyOriginalAttribute(){
        movementSpeed=orignialSpeed;
        runMultiplier=orignialRunMultiplier;
        gravity=orignialGravity;
        jumpSpeed=originalJumpSpeed;
    }

    void MovementVer2(){
        //grounded reset
        if(controller.isGrounded && velocity.y < 0){
            velocity.y = -2f;
            headHitSomething=false; //reset 
        }

        //wasd movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * x + transform.forward * z;

        if(Input.GetKey(KeyCode.LeftShift)){   //allow player to accelerate by holding left shift when it is grounded
            //Debug.Log("accelerating");
            playerAttribute.isAccel = true;
            movement*= runMultiplier;
        } else {
            playerAttribute.isAccel = false;
        }
        controller.Move(movement * movementSpeed * Time.deltaTime);

        //apply gravity 
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        //Jump handler
        if(Input.GetButton("Jump") && controller.isGrounded){
            velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
        }

        Vector3 origin = new Vector3(
        transform.position.x,
        transform.position.y-GetComponent<CharacterController>().height/2+GetComponent<CharacterController>().radius,   // the y value of the center of bottom sphere
        transform.position.z);

        if(headHitSomething&& velocity.y>0){
            velocity.y=0;   
        }

        //accelerating
        if(Input.GetKey(KeyCode.LeftShift))
        {
            controller.Move(movement * Time.deltaTime * runMultiplier);
        }

    }
    
}
