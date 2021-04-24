using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAiming : MonoBehaviour
{

    Camera mainCamera;
    [SerializeField] BattleHandler BattleManager;
    public Transform bulletStartPos;
    public Transform bulletTarget;

    //Used to determine where the bullet should go to
    Ray ray;        // a ray which is "shooted to" the crosshair  
    RaycastHit hitInfo;     //the gameobject that is hitted by the ray  

    // reference to the chosen skill for choosing the gameEffect object 
    private PlayerAttributes player;  //used to get the chosen battle skill
    private GameObject effectToSpawn;

    // Update is called once per frame

    void Start() {
        mainCamera=Camera.main;
        player = gameObject.GetComponent<PlayerAttributes>();

    }

    void Update()
    {   
        if(player.inBattle){  // only can attack in battle
            //Debug.Log("player in battle");
            CastSkill(); 
        }
        else{

        }
    }

    void CastSkill(){
        if(Input.GetKeyDown(KeyCode.Mouse0) //player clicked the mouse, the skill is not in cool down, having enough energy
            && player.skills[player.theChosenSkillIndex].IsAvailabe() && IfHaveEnoughEnergy() ){
            SetSkillToCoolDownState(player.theChosenSkillIndex);
            SpentPlayerEnergy();
            UpdateTheChosenSkillEffect();
            UpdateAimingRayAttribute();
            UpdateBulletTargetPosition();
            CreateVFXObject();
        }
    }

    void SetSkillToCoolDownState(int skillIndex){
        player.skills[skillIndex].SetSkillToUnavailable();
        float coolDownTime =  player.skills[skillIndex].GetAttackCoolDown();
        StartCoroutine(player.skills[skillIndex].SetParticularSkilltoAvailable_AfterTime(coolDownTime));
    }

    void UpdateTheChosenSkillEffect(){

        if(player.theChosenSkillIndex!=0){    // not the dummy skill
            effectToSpawn = player.skills[player.theChosenSkillIndex].EffectToSpawn;
        }
        else{   //dummy skill is assigned
            effectToSpawn=null;
            Debug.Log("Assigned effectToSpawn to null");
        }
    }

    bool IfHaveEnoughEnergy(){
        if( player.currentEnergy - player.skills[player.theChosenSkillIndex].GetAttackEnergyCost() >= Mathf.Epsilon)
            return true;
        else
            return false;
    }

    void SpentPlayerEnergy(){
        float cost = player.skills[player.theChosenSkillIndex].GetAttackEnergyCost();
        player.LostEnergyBy(cost);
    }

    void UpdateAimingRayAttribute(){
        ray.origin=mainCamera.transform.position;
        ray.direction=mainCamera.transform.forward;
    }

    void UpdateBulletTargetPosition(){
    
        if(Physics.Raycast(ray, out hitInfo)){  
            bulletTarget.position = hitInfo.point;
        }
        else{
            bulletTarget.position = ray.origin + ray.direction * 1000.0f;
        }
    }

    void CreateVFXObject(){
      //  Create the effect object and assign the proper rotation
        if(effectToSpawn!=null){
            GameObject vfx = Instantiate (effectToSpawn, bulletStartPos.position, Quaternion.identity);
            vfx.transform.localRotation=Quaternion.LookRotation((bulletTarget.position-bulletStartPos.position).normalized);
            vfx.GetComponent<ProjectileMoveScript>().damage = BattleManager.GetSkillDamageToTarget(player.skills[player.theChosenSkillIndex],TargetIs.ENEMY);

        }
        else{
            Debug.Log("The effect is null");
        }
    }
}



// The Debug Red Line        
// Debug.DrawLine(ray.origin,lookAtPosition.position, Color.red,1f);
