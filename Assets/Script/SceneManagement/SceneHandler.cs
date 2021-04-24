using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;


//STUPID UNITY API. THIS SCRIPT IS THE TOUGHEST SCRIPT THAT I HAVE WRITTEN SO FAR
// How do i know CharacterController is gonna to overwrite transform.postion for every stupid frame T-T By Cfull
public class SceneHandler :MonoBehaviour
{
    public enum SceneType{
        MAIN,BATTLE
    }
    private string previousMainScene="";

   // public Transform playerMainScenePos;
  //  public Transform enemyMainScenePos;
    public Transform playerBattleScenePos;
    public Transform enemyBattleScenePos;
    
    //to hold those data across different scene , to remember which pos is used to trigger the battle
    public static Vector3 playerMainScene_TriggerPos = new Vector3(0,0,0);
    public static Vector3 enemyMainScenePos_TriggerPos = new Vector3(0,0,0);

    //Only contain player and enemy
    public List<GameObject> preservedObjList = new List<GameObject>();
    [HideInInspector] static GameObject obj;

    private GameObject player;

    void Start(){
        
        if(obj!=null){ 
            Destroy(gameObject);
            return ;
        }
        obj = gameObject;
        DontDestroyOnLoad(gameObject);
    }


    IEnumerator SpawnCharacters(string toScene){
        //add scene transition animation here if we have time :) 
        AsyncOperation isLoaded = SceneManager.LoadSceneAsync(toScene);
        while(!isLoaded.isDone){
            //animation
            Debug.Log("loading");
            yield return null;
        }
       // yield return SceneManager.LoadSceneAsync(GetNextSceneIndex());

        player =GameObject.FindGameObjectWithTag("Player");
        //i used almost 1week to find out my entire system is not working bcuz of this line :))))))))))))))
        player.GetComponent<CharacterController>().enabled=false;       
        if( IsInMainScene() ){
            //spawn players and enemy in main scene
            player.transform.position = playerMainScene_TriggerPos;
        }
        else if(IsInBattleScene()){
            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            enemy.GetComponent<NavMeshAgent>().Warp(enemyBattleScenePos.position);  //need to use warp to spawn a gameobj with navmeshagent
           
            player.transform.position = playerBattleScenePos.position;

            //cameraCtor.Target=enemyBattleScenePos ;
            CameraController cameraCtor= GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.transform.LookAt(enemyBattleScenePos);   

        }
        player.GetComponent<CharacterController>().enabled=true;

    }


//Get Functions

    string GetBattleSceneName(){
        return "BattleScene";
    }

    string GetPreviousMainScene(){
        return previousMainScene;
    }

    public void LoadBattleSceneFromMainScene(string currentSceneName){
        previousMainScene = currentSceneName;
        StartCoroutine(SpawnCharacters(GetBattleSceneName()));
    }
  
    public IEnumerator LoadPreviousMainSceneFromBattleAfter(float time){
        yield return new WaitForSeconds(time);
        StartCoroutine(SpawnCharacters(GetPreviousMainScene()));
    } 

    public bool IsInMainScene(){
        if( SceneManager.GetActiveScene().name == "MainScene"||
            SceneManager.GetActiveScene().name == "MainScene_Snow"){

            return true;
        }
        return false;
    }

    public bool IsInMainScene(string sceneName){
        if( sceneName == "MainScene"||
            sceneName == "MainScene_Snow"){

            return true;
        }
        return false;
    }

    public bool IsInBattleScene(){
        if( SceneManager.GetActiveScene().name == "BattleScene"){
            return true;
        }
        return false;
    }

    public bool IsInBattleScene(string sceneName){
         if( sceneName == "BattleScene"){
            return true;
        }
        return false;
    }
//set functions

    public void SetPlayerMainScenePosHolder(Vector3 pos){
        playerMainScene_TriggerPos = pos;
        //Debug.Log("recorded player main scene position in "+pos[0]+" "+pos[1]+" "+pos[2]+" ");

    }

    public void SetEnemyMainScenePosHolder(Vector3 pos){
        enemyMainScenePos_TriggerPos = pos;
    }

}
