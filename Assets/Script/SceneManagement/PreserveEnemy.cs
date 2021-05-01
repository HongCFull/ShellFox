using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//it should be called when the enemy is hitted to/by player
public class PreserveEnemy : MonoBehaviour
{
    [HideInInspector] public SceneHandler sceneHandler;
    [HideInInspector] BattleHandler battleManager;   //to get the defeated enemy name for not spawing those  
    [HideInInspector] public static GameObject obj;
    [SerializeField] float spawnTime;
    private GameObject player;  //only inside area of interst will be kept 
    private EnemyAOISpawner enemyAOISpawner;

    void Awake(){
        player= GameObject.FindGameObjectWithTag("Player");
        enemyAOISpawner = GameObject.FindGameObjectWithTag("EnemySpawnHandler").GetComponent<EnemyAOISpawner>();
        battleManager= GameObject.FindGameObjectWithTag("BattleHandler").GetComponent<BattleHandler>();
        sceneHandler = GameObject.FindGameObjectWithTag("SceneHandler").GetComponent<SceneHandler>();
        ReAssignDependencies();
    }

    void Update() {
        if(sceneHandler.IsInBattleScene())  return;     //dont destroy this enemy if it is battling with the player
        float distanceToPlayer = (player.transform.position - gameObject.transform.position).magnitude;
        if(distanceToPlayer >enemyAOISpawner.maxRadius){
            if(gameObject.tag=="EnemyClone")    return; //dont destroy the copy template of AOI spawner!
            else    
            //else if(gameObject.layer == LayerMask.NameToLayer("EnemyClone")){
            //    Debug.Log("Destroyed");
                Destroy(gameObject);    //destroy enemy if it is cloned/from original 

           // }
            //else
           //  gameObject.SetActive(false);    //dont delete original enemy "prefab"
            
        }
    }

    private void ReAssignDependencies(){
        if(sceneHandler.IsInMainScene()){
            sceneHandler = GameObject.FindGameObjectWithTag("SceneHandler").GetComponent<SceneHandler>();
            battleManager= GameObject.FindGameObjectWithTag("BattleHandler").GetComponent<BattleHandler>();
            //Debug.Log("reasiggned");
            
            foreach (string name in battleManager.DefeatedEnemyNameList){
                if(name == gameObject.name){
                    //shouldnt spawn this enemy now 
                    EnemySpawnHandler spawner = GameObject.FindGameObjectWithTag("EnemySpawnHandler").GetComponent<EnemySpawnHandler>();
                    spawner.RegisterToActiveAfterTime(gameObject,spawnTime);    //as we cant directly call a coroutine with inactive obj
                }
            }
        }
    }

    public void DontDestroyThisEnemy(){
        if(obj!=null){  //if there is already one gameobject
            Destroy(gameObject);
            return ;
        }

        obj = gameObject;
        DontDestroyOnLoad(gameObject);
        sceneHandler.preservedObjList.Add(gameObject);
    }

    public void DontDestroyEnemyClone(){
          if(obj!=null){  //if there is already one gameobject
            Destroy(gameObject);
            return ;
        }

        obj = gameObject;
        DontDestroyOnLoad(gameObject);
    }



    //called by the enemy StateMachine to load the battle scene 
    public void EnemyTrigger_LoadBattleScene(){
        DontDestroyThisEnemy();
        sceneHandler.LoadBattleSceneFromMainScene(SceneManager.GetActiveScene().name);    //load the battle scene immediately just after the enemy touched the player
    }

    public void RecordEnemyWorldScenePos(){
       // sceneHandler.enemyMainScenePos = gameObject.transform.position;
    }
}