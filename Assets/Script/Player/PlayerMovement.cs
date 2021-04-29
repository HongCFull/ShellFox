using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public PlayerAttributes playerAttribute;
    public PlayerAiming aim;
    private FootSteps footSteps;


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

    //for animation
    public bool enterJump;
    public Animator ani;

    // for footstep sound
    private float footStepsAccumulator=0;
    private bool inAir =false;

//for initialization

    void RecordOriginalAttribute(){
        orignialSpeed = movementSpeed;
        orignialRunMultiplier=runMultiplier;
        orignialGravity=gravity;
        originalJumpSpeed = jumpSpeed;
    }

    void Start(){
        controller = GetComponent<CharacterController>();
        ani = GetComponentInChildren<Animator>();
        aim = GetComponent<PlayerAiming>();
        footSteps = GetComponent<FootSteps>();
        enterJump = false;
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
            inAir = false;
        }

        //wasd movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * x + transform.forward * z;

        //update animation
        if (movement.x + movement.z == 0) {
            ani.SetFloat("Speed", 0.0f,0.15f,Time.deltaTime);
        } else {
            ani.SetFloat("Speed", 0.5f,0.15f,Time.deltaTime);
            footStepsAccumulator +=Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.LeftShift)){   //allow player to accelerate by holding left shift when it is grounded
            //Debug.Log("accelerating");
            playerAttribute.isAccel = true;
            movement*= runMultiplier;
            ani.SetFloat("Speed", 1.0f,0.15f,Time.deltaTime);
        } else {
            playerAttribute.isAccel = false;
            if (movement.x + movement.z == 0) {
                ani.SetFloat("Speed", 0.0f,0.15f,Time.deltaTime);
            } else {
                ani.SetFloat("Speed", 0.5f,0.15f,Time.deltaTime);
            }
        }
        if(!playerAttribute.canMove)    movement=new Vector3(0,0,0);

        controller.Move(movement * movementSpeed * Time.deltaTime);

        //play foot steps sound
        if(footStepsAccumulator>footSteps.playFootStepsPeriod && !inAir){
            footSteps.PlayFootStep();
            footStepsAccumulator=0;
        }

        //apply gravity 
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        //Jump handler
        if(Input.GetButton("Jump") && controller.isGrounded &&playerAttribute.canMove){
            enterJump = true;
            inAir = true;
            ani.SetBool("isJumping", enterJump);
            velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
        }

        //update jump animation
        ani.SetBool("isJumping", enterJump);

        if (enterJump && controller.isGrounded) {
            enterJump = false;
        }

        Vector3 origin = new Vector3(
        transform.position.x,
        transform.position.y-GetComponent<CharacterController>().height/2+GetComponent<CharacterController>().radius,   // the y value of the center of bottom sphere
        transform.position.z);

        if(headHitSomething&& velocity.y>0){
            velocity.y=0;   
        }

        //accelerating
        if(Input.GetKey(KeyCode.LeftShift)){
            if(movement.magnitude>0.0f)
                footStepsAccumulator +=Time.deltaTime;
            controller.Move(movement * Time.deltaTime * runMultiplier);
        }

        //update attack animation
        ani.SetBool("isAttacking", aim.isCastingSkill);

    }

}
