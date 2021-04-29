using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;


//NOTE : Enemy is Defined as THE GLOBAL ENEMY. Enemy is never used to describe The Player 
//A GOD CLASS WITH CODE THAT IS HURTING MY EYE :P 

public class EnemyStateMachine : MonoBehaviour
{
    //getting vision
    public GameObject enemyVisionFrom;
    private EnemyVision enemyVision;

    //getting animator
    public Animator ani;

    //getting attack ability 
    private EnemyFire enemyFire;

    //Fetching EnemyAttributes
    [SerializeField] EnemyAttributes enemyAttributes;
    UnityEngine.AI.NavMeshAgent EnemyAgent;
    float attackArea;

    //Main Scene : Enemy States Variable
    [SerializeField]float MainSceneRandWanderRadius;
    bool isInHatredState; //Keep chasing player in main scene?

    //BattleScene : Enemy States Variable 
    bool isCastingSkill;    //battle scene state: is the player casting skill at that time?
    bool isAllowedToAttack; //battle scene state: can the enemy attack or not at that moment
    bool canRandomWander ;

    //for chasing player
    public Transform Player;      //the target that AI will keep chasing for
    [SerializeField] float headTurningSpeed; 
    Vector3 distanceToPlayer;

    //for time checking
    [SerializeField] float hatredTime=4f;   //keep chasing player for this time
    [SerializeField] float castSkillTimeBuffer=2f;   //wait for 2 sec before the next attk
    float outOfVisionTime;   //a timer used to keep track how long the player is away from enemy's vision

    //for init position attributes 
    Vector3 initPosition;
    Quaternion initLookRotation;

    //for function call Optimization
    bool haveSeenPlayerOnce;         
    IEnumerator ResetCoroutine;
    bool isResetingEnemyCoroutine;    
    
    //for random behaviour in main/battle scene
    float randAccumulator_MainScene;
    float randAccumulator_BattleScene;
    bool randWanderExited;
    IEnumerator RandomWanderingCo;

    //Generic SceneHandler
    private SceneHandler sceneHandler;
    private BattleHandler battleHandler;

    //debugging method for built game
    private float activeTriggerBuffer = 0.3f;  //after loading main scene , wait for this time to trigger battle
    private float triggerAccumulator;
//initialization
 
    void SetInitialData(){
        isInHatredState=false;
        isCastingSkill=false;
        isAllowedToAttack=true;
        outOfVisionTime=0f;   
        haveSeenPlayerOnce=false;
        isResetingEnemyCoroutine=false;
        randAccumulator_MainScene=0f;
        randAccumulator_BattleScene=0f;
        canRandomWander=false;
        randWanderExited=true;
        triggerAccumulator = 0f; 

        sceneHandler = GameObject.FindGameObjectWithTag("SceneHandler").GetComponent<SceneHandler>();
        battleHandler = GameObject.FindGameObjectWithTag("BattleHandler").GetComponent<BattleHandler>();
       
        enemyAttributes.inBattle=false;
        enemyAttributes.isIdle=true;
        enemyAttributes.canMove=true;
        
    }

    void ReAssignDependencies(){
        //for scene reloading management
        if(sceneHandler.IsInMainScene()){
            GetComponent<CapsuleCollider>().isTrigger = true;
            Player = GameObject.FindGameObjectWithTag("Player").transform;    //reassign dependencies
        }
    }

    void Start()
    {
        SetInitialData();
        ReAssignDependencies();
        
        //fetching enemyAttribute
        attackArea = enemyAttributes.attackArea;

        enemyVision=enemyVisionFrom.GetComponent<EnemyVision>();

        //ani = GetComponentInChildren<Animator>();

        EnemyAgent=GetComponent<UnityEngine.AI.NavMeshAgent>();
        EnemyAgent.speed = enemyAttributes.enemySpeed;

        enemyFire=GetComponent<EnemyFire>();

        SaveOriginalGeometry(); 
    }

    void SaveOriginalGeometry(){
        initPosition = EnemyAgent.transform.position;
        initLookRotation=transform.rotation;
    }
    
//Runtime Function
    void Update(){   
        triggerAccumulator+=Time.deltaTime;
        UpdatingEnemyState();
        if(haveSeenPlayerOnce || sceneHandler.IsInBattleScene()){  //for optimization
            UpdateDistanceToPlayer();
            EnemyReactWithState();
            ani.SetBool("isAttacking", isCastingSkill); //update attack animation
        }
    }

    void UpdateDistanceToPlayer(){
        distanceToPlayer=  Player.position-transform.position ;   
    }
   
    void UpdatingEnemyState(){
        if(sceneHandler.IsInMainScene()){
            randAccumulator_MainScene+=Time.deltaTime;
            if( isInHatredState || enemyVision.CanSee(Player.gameObject) ){    //monster was chasing player, or inside the vision of enemy
                isInHatredState=true;
                randAccumulator_MainScene = 0;    //dont need random wandering
                haveSeenPlayerOnce=true;
                Reset_IfNotInsideVisionForSomeTime();   //Maybe it is better to put in the fsm?
            }                 
            else if(randAccumulator_MainScene>= 3f){
                randAccumulator_MainScene=0;
                Vector3 newPos = GetRandomWanderPos_MainScene();
                EnemyAgent.SetDestination(newPos);
            }
            
        }
        else if(sceneHandler.IsInBattleScene()){   //BattleScene 
            enemyAttributes.inBattle=(true);
            enemyAttributes.UpdateCanmoveState();

            //rand wandering only happened when the enemy can attack the player And energy > half
            if(IsInsideAttackRegion() && enemyAttributes.currentEnergy>=enemyAttributes.maxEnergy/2){
                randAccumulator_BattleScene+=Time.deltaTime;
                if(randAccumulator_BattleScene>enemyAttributes.randWanderingPeriod){
                    randAccumulator_BattleScene=0;
                    canRandomWander = true;
                }
            }
        }
    }

    //Trigger the battle if the enemy is hitted by the player
    //in main scene , set enemy collider to trigger only 
    //in battle scene , set enemy collider to !(trigger only)  
    
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player" && sceneHandler.IsInMainScene()&& triggerAccumulator>=activeTriggerBuffer){
            triggerAccumulator = 0f;
            SetEnemyIsInBattleState(); 
            AssignEnemyAttributeToBattleManager();
            GetComponent<CapsuleCollider>().isTrigger = false;
            GetComponent<PreserveEnemy>().sceneHandler.SetPlayerMainScenePosHolder(other.gameObject.transform.position);
            GetComponent<PreserveEnemy>().EnemyTrigger_LoadBattleScene();
        }
    }
    
// The Main Finite State Reacting function
    void EnemyReactWithState(){
        if(sceneHandler.IsInMainScene()){  //MainScene StateMachine
            if(isInHatredState){    
                if(isResetingEnemyCoroutine)  // if the enemy was reseting, cancel the reset and chase the player
                    StopCoroutine(ResetCoroutine);
                FaceTarget(Player.position);
                ChasePlayer();          
            }
        }
        else if(sceneHandler.IsInBattleScene()){   //BattleScene StateMachine
            //IF BOTH VARIABLES ARE FALSE WILL RESULT IN DEADLOCK, BUT I HAVE NO IDEA WHAT CAUSES IT
            //TEMP SOLUTION : HARDCODE ONE OF THE VARIABLE TO BE TRUE!  
            if(!randWanderExited&&!canRandomWander) {
                randWanderExited=true;
            }

            if(!randWanderExited)    //The randwander must be finished first to proceed!
                return;

            if(canRandomWander){    
                canRandomWander=false;  //make sure only 1 time called
                randWanderExited=false;
                Vector3 randPos = GetRandomWanderPos_BattleScene();
                RandomWanderingCo= RandomWandering_BattleScene(randPos);
                StartCoroutine(RandomWanderingCo);
            }
            else{
                
                if(CanAttack()){
                    StopAndAttackPlayer();  //pass skill as var
                }
                else if(InAttackCoolDown()){    //skills are cooling down
                    FaceTarget(Player.position);
                    SetEnemyToIdle();
                }
                else if(!IsInsideAttackRegion()||!enemyVision.CanSee(Player.gameObject)){   //outside attack region
                    if (enemyAttributes.canMove ) { //chasing player and have energy
                        FaceTarget(Player.position);
                        ChasePlayer();
                    } 
                    else {  //chasing player but dont have energy
                        FaceTarget(Player.position);
                        SetEnemyToIdle();
                    }
                }
            }
            
        }
    }


//Enemy Movement Operation (chase, lookat)
    void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, headTurningSpeed*Time.deltaTime);  //2 = turn head's angular velocity 

    }

    void ChasePlayer(){
       // Debug.Log("Statemachine:: Chase Player");
        EnemyAgent.speed = enemyAttributes.enemySpeed;
        EnemyAgent.destination=Player.position; 
        enemyAttributes.isIdle=false;
    }

    Vector3 GetRandomWanderPos_MainScene(){
        Vector3 randomDir = Random.insideUnitSphere * MainSceneRandWanderRadius;
        randomDir+=initPosition;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDir, out hit,MainSceneRandWanderRadius,1);
        return hit.position;
    }

    Vector3 GetRandomWanderPos_BattleScene(){
        //a concentric circle which centered at the Player!
        float wanderRadius = (enemyAttributes.attackArea-enemyAttributes.distanceAwayFromPlayer);
        wanderRadius = Mathf.Max(0,wanderRadius);  //clamp it to positive

        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        Vector3 randPos = (randomDir+Player.position);
        NavMeshHit hit;
        NavMesh.SamplePosition(randPos, out hit,enemyAttributes.attackArea,1);
        return hit.position;
    }

    IEnumerator RandomWandering_BattleScene(Vector3 targetPos){

        while(!HaveReachedPos(targetPos) && enemyAttributes.canMove ){ //get out of the loop if reached pos or enemy have no energy
            EnemyAgent.SetDestination(targetPos);
            randAccumulator_BattleScene=0;  //dont contribute to randAccumulator during walking
            yield return null;
        }
        randWanderExited=true;
    }

// Enemy Attack Functions

    void StopAndAttackPlayer(){ //Suppose skill var is passed by ref

        FaceTarget(Player.position);
        SetEnemyToIdle();
        AttackPlayer();

    }

    void AttackPlayer(){
        //   enemyAttributes.PrintAllAvailableSkills();
        enemyAttributes.UpdateAttackSkillIndex();

        int skillIndex = enemyAttributes.GetSkillIndex();
        //   Debug.Log("Chose the skill index = "+skillIndex);

        enemyFire.ShootRay();

        //Spent Energy and Set skill to unavailable
        enemyAttributes.LostEnergyBy( enemyAttributes.skills[skillIndex].GetAttackEnergyCost());
        enemyAttributes.skills[skillIndex].SetSkillToUnavailable();
        
        //set the skills after some cool down time
        HandleSkillCoolDown(skillIndex);
    }

    void HandleSkillCoolDown(int skillIndex){
        StartCoroutine(enemyAttributes.skills[skillIndex].SetParticularSkilltoAvailable_AfterTime(
            enemyAttributes.skills[skillIndex].GetAttackCoolDown() ));
        StartCoroutine(SetEnemyIsCastingSkill(enemyAttributes.skills[skillIndex].GetAttackCastTime()));         
    }

    IEnumerator SetEnemyIsCastingSkill(float skillCastTime){
        isCastingSkill=true;
        isAllowedToAttack=false;

        yield return new WaitForSeconds(skillCastTime); //after skillCastTime sec
        isCastingSkill=false;
    // random walk should be done in here if current energy >= thresold
        yield return new WaitForSeconds(castSkillTimeBuffer); //after skillCastTime + castSkillTimeBuffer sec
        isAllowedToAttack=true;

    }

//Reset Functions

    void Reset_IfNotInsideVisionForSomeTime(){
        if(!enemyVision.CanSee(Player.gameObject)){ //start counting when the player isn't inside the enemy vision
            outOfVisionTime+=Time.deltaTime;
            if(outOfVisionTime >hatredTime) {  //the enemy is no longer interested in chasing the player
               
                SetInitialData();   //restore all the states in FSM
                enemyAttributes.ResetBattleAttributes_and_UI(); //reset the battle attributes

                ResetCoroutine=ResetEnemyPosition(); //record the reset coroutine <- we may need to stop it depends on the situation
                StartCoroutine(ResetCoroutine);   //geometrically reset enemy                
            } 
        }
        else{
            outOfVisionTime=0;   //reset timer
        }
    }

    IEnumerator ResetEnemyPosition(){   //set the enemy to original pos. if the enemy has reached the original pos, apply the original rotation
        isResetingEnemyCoroutine=true;
        enemyAttributes.isIdle=false;

        EnemyAgent.destination=initPosition;
        while(!HaveReachedPos(initPosition)){    
            yield return null;
        }

        while(Vector3.Angle(transform.rotation*transform.forward , initLookRotation*transform.forward)>5f){
            transform.rotation = Quaternion.Slerp(transform.rotation, initLookRotation, headTurningSpeed * Time.deltaTime);  // the last parameter is the head turning angular speed 
            yield return null;
        }

        SetEnemyToIdle();
        isResetingEnemyCoroutine=false;
    }

    void SetEnemyToIdle(){
        //set state and geometrically idle
        EnemyAgent.destination=transform.position;
        enemyAttributes.isIdle=true;
    }

// Set Enemy's battle state 
    void SetEnemyIsInBattleState(){
        haveSeenPlayerOnce=true;
        isInHatredState=true;
        enemyAttributes.inBattle=(true);
       // AssignEnemyAttributeToBattleManager();
    }

    void AssignEnemyAttributeToBattleManager(){
        //Link the enemyAttribute part to the battleManager
        battleHandler.enemy = gameObject.GetComponent<EnemyAttributes>();
        battleHandler.UpdatePlayerInBattleOrNot();
    }

// State Checking helper 

    bool IsInsideAttackRegion(){
       return distanceToPlayer.magnitude <=attackArea;
    }

    bool CanAttack(){   //suppose skill have been chosen
        if(isAllowedToAttack && IsInsideAttackRegion() && enemyVision.CanSee(Player.gameObject))        //stop and attack
            return true; 
        else
            return false;
    }

    bool InAttackCoolDown(){
        return IsInsideAttackRegion() && !isAllowedToAttack;
    }

    bool HaveReachedPos(Vector3 Pos){
        float Offset = 0.3f;
        return  (Mathf.Abs(transform.position.x-Pos.x)<Offset) &&
                (Mathf.Abs(transform.position.z-Pos.z)<Offset) &&
                ((transform.position.x - Pos.x)<Offset) ;     //Note that Y coor will differ a bit
    }

    bool IsNavMeshWalkable(Vector3 targetDestination){
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetDestination, out hit, 1f, NavMesh.AllAreas))
            return true;
        return false;
    }

//Visualize Debug Function
    private void OnDrawGizmos() {
    
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(transform.position,attackArea);
    }

}