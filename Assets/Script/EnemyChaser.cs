using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyChaser : MonoBehaviour
{

    //for chasing player
    [SerializeField] Transform Player;      //the target that AI will keep chasing for
    [SerializeField]float alertArea=10f;
    [SerializeField]float attackArea =3f;

    UnityEngine.AI.NavMeshAgent navMesh;
    Vector3 distanceToPlayer;
    bool isChasingPlayer=false;
    float nonHatredTime=0; //how much time the player is away from monster's hatred 

    //for time checking
    [SerializeField]float resetTime =5f;    //record how much time is used for dispatching the hatred of player
    float timePassed;

    //for position attribute short naming
    Vector3 initPosition;

//initialization
    void Start()
    {
        navMesh=GetComponent<UnityEngine.AI.NavMeshAgent>();
        SaveOriginalPosition(); 
        
    }
    
    void SaveOriginalPosition(){
        initPosition= transform.position;
    }
    
//Runtime Function
    void Update()
    {
        UpdateDistanceToPlayer();
        UpdatingEnemyState();
        ChasePlayer();

    }

    bool IsFacingTowardsPlayer(){
        Vector3 EnemyFoward = transform.forward;
        return Vector3.Dot(EnemyFoward,distanceToPlayer)>0; //return true if facing towards player
    }

    bool IsInsideAlertRegion(){
        return distanceToPlayer.magnitude <= alertArea;
    }

    void UpdateDistanceToPlayer(){
        distanceToPlayer=  Player.position-transform.position ;   
    }

    void UpdatingEnemyState(){

        if(  isChasingPlayer||     //monster was chasing player
            IsInsideAlertRegion()  &&  IsFacingTowardsPlayer() ){   //player is in the alert region & enemy is facing towards the player 

            isChasingPlayer=true;
           // nonHatredTime=0f;       //reset time counter
        }
    
        if( !IsInsideAlertRegion() ){
            //Debug.Log("wanna go back now");    
            if(isChasingPlayer)
                nonHatredTime+=Time.deltaTime;
            
            if(nonHatredTime > resetTime){  //the enemy is no longer interested in chasing the player
                isChasingPlayer=false;
                nonHatredTime=0f;   //reset time counter
            }
        }

    }

    // enemy chasing function
    void ChasePlayer(){
        if(isChasingPlayer){
            navMesh.destination=Player.position;
            if(distanceToPlayer.magnitude <=attackArea){        //stop and attack
                navMesh.destination=transform.position;
            }
        }
        //go back to spawn point if it have chosen not to chase the player 
        else{
            navMesh.destination=initPosition;
        }
    }


    private void OnDrawGizmos() {
        Gizmos.color=Color.green;
        Gizmos.DrawWireSphere(transform.position,alertArea);
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(transform.position,attackArea);

    }

}
