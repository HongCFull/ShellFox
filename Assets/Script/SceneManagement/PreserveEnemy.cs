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
        
    void Awake(){
        battleManager= GameObject.FindGameObjectWithTag("BattleHandler").GetComponent<BattleHandler>();
        sceneHandler = GameObject.FindGameObjectWithTag("SceneHandler").GetComponent<SceneHandler>();
        ReAssignDependencies();
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

    //called by the enemy StateMachine to load the battle scene 
    public void EnemyTrigger_LoadBattleScene(){
        DontDestroyThisEnemy();
        sceneHandler.LoadBattleSceneFromMainScene(SceneManager.GetActiveScene().name);    //load the battle scene immediately just after the enemy touched the player
    }

    public void RecordEnemyWorldScenePos(){
       // sceneHandler.enemyMainScenePos = gameObject.transform.position;
    }
}