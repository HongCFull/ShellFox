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

    bool BattleEndProcessed;
    private float loadSceneDelay = 3f;  //Note: this variable should only be set inside the script

    void Start(){
        sceneHandler = GameObject.FindGameObjectWithTag("SceneHandler").GetComponent<SceneHandler>();
        DefeatedEnemyNameList = new List<string>();
        BattleEndProcessed = false;
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
/*
    public void DealDamageToTargetBySkill(BattleSkill attackSkill,TargetIs target){
        if(target==TargetIs.ENEMY){ //deal damage to global enemy
            if(enemy==null) return;
            enemy.LostHpBy( GetSkillDamageToTarget(attackSkill,target) );
        }
        else{   //deal dmg to player
            if(player==null) return;
            player.LostHpBy( GetSkillDamageToTarget(attackSkill,target) );
        }
    }
*/
    public float GetSkillDamageToTarget(BattleSkill attackSkill, TargetIs target){   //target = the one who are being attacked 
       if(target==TargetIs.ENEMY){
           if(enemy==null)  return 0;
            return CalculateElementFactor(attackSkill,target)*(
                (attackSkill.GetSkillPower() * player.attack / (enemy.defense*5))+ 5);
       }
       else{    //dealing dmg to player
            if(player==null)  return 0;
            return CalculateElementFactor(attackSkill,target)*(
                (attackSkill.GetSkillPower() * enemy.attack / (player.defense*5))+ 5);
       }
    }

    //called by scene handler
    public void SetUpBattleHandler(EnemyAttributes eny){
        enemy = eny;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttributes>();
        UpdatePlayerInBattleOrNot();
        BattleEndProcessed = false;
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
        if(player==null)    return;
        for(int i =1 ; i <player.skills.Length ; i++){
            if(player.skills[i]==null) Debug.Log("null player skill + " +i);
            player.skills[i].EffectToSpawn.GetComponent<ProjectileMoveScript>().damage = 
            (GetSkillDamageToTarget( player.skills[i],TargetIs.ENEMY));
        }
    }

    public void UpdateEnemyVFXDmg(Scene scene, LoadSceneMode mode){
        if(!sceneHandler.IsInBattleScene(scene.name)) return;
        if(enemy==null)    return;
        for(int i =1 ; i <enemy.skills.Length ; i++){
            enemy.skills[i].EffectToSpawn.GetComponent<ProjectileMoveScript>().damage = 
            (GetSkillDamageToTarget( enemy.skills[i],TargetIs.PLAYER));
        }
    }

    private bool AttackElementIs(BattleSkill attackSkill,BattleType.Element element){
        if(attackSkill == null) Debug.Log("asdajsd");
        return attackSkill.GetAttackElementType()==element;
    }

    public bool Operation_CharacterDefeated(CharacterBattleAttributes defeatedChar){
        if (BattleEndProcessed) return false;
        if(defeatedChar.GetType() == typeof(EnemyAttributes)){     //enemy is defeated
            BattleEndProcessed = true;
//DefeatedEnemyNameList.Add(defeatedChar.gameObject.name);    //record the name of this enemy for spawning function
            player.PlayerGainExp(enemy.expGainedByPlayer);
            player.GetComponent<PlayerAttributes>().GainHalfHpEnergy();  //refill players battle data
            player.UpGradePlayerLevelAndAttributes();
            player.HideBattleUI();  
            Destroy(defeatedChar.gameObject);   //destroy the enemy in dont destroy on load && the battle manager will reset
            StartCoroutine(sceneHandler.LoadPreviousMainSceneFromBattleAfter(loadSceneDelay));

        }
    //NOT IMPLEMENTED IF THE PLAYER IS DEFEATED!
        else if(defeatedChar.GetType() == typeof(PlayerAttributes)){   //player is defeated
            BattleEndProcessed = true;
          //  player.enabled = false;
            enemy.ResetBattleAttributes_and_UI();
            player.ResetBattleAttributes_and_UI();  //refill players battle data
            player.HideBattleUI();
            
            enemy = null;
            player = null;
            
            //defeatedChar.gameObject.SetActive(false);
            StartCoroutine(sceneHandler.RestorePlayerMainSceneFromLastSave(loadSceneDelay));
           // defeatedChar.gameObject.SetActive(true);
          //  player.enabled = true;
            Debug.Log("YOU LOSE!!!!");
        }
        return false;
    }

}
