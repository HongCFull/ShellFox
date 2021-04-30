using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TargetIs{PLAYER,ENEMY}

public class BattleHandler : MonoBehaviour
{
    [Tooltip("player should be always assigned in the inspector")]
    public PlayerAttributes player;
    public EnemyAttributes enemy;
    public List<string> DefeatedEnemyNameList; 
    [SerializeField] SceneHandler sceneHandler;

    private float loadSceneDelay = 3f;  //Note: this variable should only be set inside the script


    void Start(){
        sceneHandler = GameObject.FindGameObjectWithTag("SceneHandler").GetComponent<SceneHandler>();
        DefeatedEnemyNameList = new List<string>();
    }


    private void OnEnable() {
        SceneManager.sceneLoaded += UpdatePlayerVFXDmg;
        SceneManager.sceneLoaded += UpdateEnemyVFXDmg;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= UpdatePlayerVFXDmg;
        SceneManager.sceneLoaded -= UpdateEnemyVFXDmg;    
    }
   
    public void UpdatePlayerInBattleOrNot(){
        if (enemy.inBattle) {
            player.inBattle=(true);

        } else {
            player.inBattle=(false);
        }
    }

    //used in testing
    public void DealDamageToTargetBySkill(BattleSkill attackSkill,TargetIs target){
        if(target==TargetIs.ENEMY){ //deal damage to global enemy
            enemy.LostHpBy( GetSkillDamageToTarget(attackSkill,target) );
        }
        else{   //deal dmg to player
            player.LostHpBy( GetSkillDamageToTarget(attackSkill,target) );
        }
    }

    public float GetSkillDamageToTarget(BattleSkill attackSkill, TargetIs target){   //target = the one who are being attacked 
       if(target==TargetIs.ENEMY){
            return CalculateElementFactor(attackSkill,target)*(
                (attackSkill.GetSkillPower() * player.attack / enemy.defense)/10 
                + 1);
       }
       else{    //dealing dmg to player
            return CalculateElementFactor(attackSkill,target)*(
                (attackSkill.GetSkillPower() * enemy.attack / player.defense)/10 
                + 1);
       }
    }

    public void SetEnemyAttributesToBattleHandler(EnemyAttributes eny){
        enemy = eny;
        UpdatePlayerInBattleOrNot();
    }

// Helper function. Should only be called inside this class 

    private float CalculateElementFactor(BattleSkill attackSkill,TargetIs target){

        BattleType.Element targetElement;
        if(target==TargetIs.ENEMY){
            targetElement=enemy.element;
        }
        else{
            targetElement=player.element;
        }
        
        // attack = fire
        if( AttackElementIs(attackSkill, BattleType.Element.FIRE) ){
            if( (targetElement==(BattleType.Element.GRASS) || targetElement==(BattleType.Element.ICE) )){   //super effective
                return 1.25f;
            }
            else if(targetElement==(BattleType.Element.WATER)){      //not effective
                return 0.75f;
            }
            else{
                return 1f;
            }
        }

        // attack = water
        else if(AttackElementIs(attackSkill, BattleType.Element.WATER) ){
            if( targetElement==(BattleType.Element.FIRE) ) //super effective
                return 1.25f;
        
            else if (targetElement==(BattleType.Element.GRASS)) //not effective
                return 0.75f;
            
            else
                return 1f;
        }

        // attack = grass
        else if(AttackElementIs(attackSkill,BattleType.Element.GRASS) ){
            if(targetElement==(BattleType.Element.WATER) )
                return 1.25f;
            
            else if (targetElement==(BattleType.Element.FIRE))
                return 0.75f;

            else 
                return 1f;
        }

        // attack =ice
        else if(AttackElementIs(attackSkill,BattleType.Element.ICE) ){
            if(targetElement==(BattleType.Element.GRASS) )
                return 1.25f;

            else if (targetElement==(BattleType.Element.FIRE))
                return 0.75f;

            else 
                return 1f; 
        }
        // Not Supposed to reach this else statement
        else
            return 1f; 
    }

    public void UpdatePlayerVFXDmg(Scene scene, LoadSceneMode mode){
        if(!sceneHandler.IsInBattleScene(scene.name)) return;
        for(int i =1 ; i <player.skills.Length ; i++){
            if(player.skills[i]==null) Debug.Log("null player skill + " +i);
            player.skills[i].EffectToSpawn.GetComponent<ProjectileMoveScript>().damage = 
            (GetSkillDamageToTarget( player.skills[i],TargetIs.ENEMY));
        }
    }

    public void UpdateEnemyVFXDmg(Scene scene, LoadSceneMode mode){
        if(!sceneHandler.IsInBattleScene(scene.name)) {
            Debug.Log("Returned in update enemy vfx");
            return;
        }
        for(int i =1 ; i <enemy.skills.Length ; i++){
            enemy.skills[i].EffectToSpawn.GetComponent<ProjectileMoveScript>().damage = 
            (GetSkillDamageToTarget( enemy.skills[i],TargetIs.PLAYER));
        }
    }

    private bool AttackElementIs(BattleSkill attackSkill,BattleType.Element element){
        if(attackSkill == null) Debug.Log("FUCK");
        return attackSkill.GetAttackElementType()==element;
    }

    public bool Operation_CharacterDefeated(CharacterBattleAttributes defeatedChar){
        if(defeatedChar.GetType() == typeof(EnemyAttributes)){     //enemy is defeated

            DefeatedEnemyNameList.Add(defeatedChar.gameObject.name);    //record the name of this enemy for spawning function
            player.PlayerGainExp(enemy.expGainedByPlayer);
            player.UpGradePlayerLevelAndAttributes();
            player.ResetBattleAttributes_and_UI();  //refill players battle data
            player.HideBattleUI();  
            Destroy(defeatedChar.gameObject);   //destroy the enemy in dont destroy on load && the battle manager will reset
            StartCoroutine(sceneHandler.LoadPreviousMainSceneFromBattleAfter(loadSceneDelay));

        }
    //NOT IMPLEMENTED IF THE PLAYER IS DEFEATED!
        else if(defeatedChar.GetType() == typeof(PlayerAttributes)){   //player is defeated
            //StartCoroutine(sceneHandler.LoadNextSceneAfterTime(loadSceneDelay));
            Debug.Log("YOU LOSE!!!!");
        }
        return false;
    }

}
