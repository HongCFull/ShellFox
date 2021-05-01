using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    [SerializeField] BattleHandler BattleManager;
    public Transform bulletStartPos;
    [Tooltip("This indicate the height of the position that hit the player")]
    public float aimHeightOffset;   
    // reference to the chosen skill for choosing the gameEffect object 
    private EnemyAttributes enemy;  //used to get the chosen battle skill

    private GameObject effectToSpawn;
    

    // Start is called before the first frame update
    void Start()
    {
        ReAssignDependencies();
        enemy = gameObject.GetComponent<EnemyAttributes>();
    }

    void ReAssignDependencies(){
        BattleManager = GameObject.FindGameObjectWithTag("BattleHandler").GetComponent<BattleHandler>();
    }

    // Update is called once per frame


    public void ShootVFX(){
        SpentEnemyEnergy();
        UpdateTheChosenSkillEffect();
        CreateVFXObject();  
    }

    void UpdateTheChosenSkillEffect(){

        if(enemy.GetSkillIndex()!=0){    // not the dummy skill
            effectToSpawn = enemy.skills[enemy.GetSkillIndex()].EffectToSpawn;
        }
        else{   //dummy skill is assigned
            effectToSpawn=null;
            Debug.Log("Assigned effectToSpawn to null");
        }
    }

    void SpentEnemyEnergy(){
        float cost = enemy.skills[enemy.GetSkillIndex()].GetAttackEnergyCost();
        enemy.LostEnergyBy(cost);
    }


    void CreateVFXObject(){
      //  Create the effect object and assign the proper rotation
        if(effectToSpawn!=null){
            GameObject vfx = Instantiate (effectToSpawn, bulletStartPos.position, Quaternion.identity);
            Vector3 playerPos = enemy.battleManager.player.transform.position;
            Vector3 direction = playerPos - bulletStartPos.position;
          
            vfx.transform.localRotation=Quaternion.LookRotation(direction.normalized);
            vfx.GetComponent<ProjectileMoveScript>().damage = BattleManager.GetSkillDamageToTarget(enemy.skills[enemy.GetSkillIndex()],TargetIs.PLAYER);
        }
        else{
            Debug.Log("The effect is not assigned");
        }
    }
}
