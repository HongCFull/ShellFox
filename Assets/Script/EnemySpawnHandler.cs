using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Each obj that should be delayed to spawn, should register in here
public class EnemySpawnHandler : MonoBehaviour
{
    [HideInInspector] public BattleHandler battleHandler;
    [HideInInspector] static GameObject obj;

    [SerializeField] float executePeriod;
    float accumulator = 0f; 

    [HideInInspector]  public GameObject receivedObj;

    void Start()
    {
        battleHandler= GameObject.FindGameObjectWithTag("BattleHandler").GetComponent<BattleHandler>();
        DontDestroyThis();
    }

    public void DontDestroyThis(){
        if(obj!=null){  
            Destroy(gameObject);
            return ;
        }
        obj = gameObject;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterToActiveAfterTime(GameObject obj,float time){
        StartCoroutine(SpawnObjectAfterTime(obj,time));
    }

    IEnumerator SpawnObjectAfterTime(GameObject obj,float time){
        yield return new WaitForFixedUpdate();
        if(obj!=null){
            obj.SetActive(false);
            receivedObj = obj;
            Debug.Log("called coroutine and waiting for " +time+" sec");
        }
        yield return new WaitForSeconds(time);
        if(obj!=null){
            Debug.Log("set the enemy with name "+obj.name+" to active");
            obj.SetActive(true);
            //delete the defeated enemy name from the battle handler list
            battleHandler.DefeatedEnemyNameList.Remove(obj.name);
        }
    }
/*
    void Update(){
        accumulator+=Time.deltaTime;
        if(accumulator >= executePeriod){   //only execute every period passed
            accumulator =0f;   

        }
    }
*/
}
