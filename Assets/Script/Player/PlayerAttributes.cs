using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttributes : CharacterBattleAttributes
{

    [HideInInspector] public static float playerExperience;
    [HideInInspector]public int theChosenSkillIndex;
    public float DisPlayExp;
    //accesed by the playerMovement script while battling 
    public float playerSpeed_inBattle;
    public float playerAccel_inBattle;
    public float playerJumpSpeed_inBattle;
    

    void SetPlayerCurrentAttributes(){
       // recoverPeriod = 2f;
       Debug.Log("SetPlayerCurrentAttributes called");
        currentHp = maxHp;
        currentEnergy = maxEnergy;
        currentIdleTime = 0;

        inBattle = false;
        isIdle = true;
        isAccel = false;
        canMove = true;
        canRecover = true;
        theChosenSkillIndex=0;

        healthBar.SetMaxAndRestoreHealth(maxHp);
        energyBar.SetMaxAndRestoreEnergy(maxEnergy);
        theChosenSkillIndex=0;

    }
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        UpGradePlayerLevelAndAttributes();
        SetPlayerCurrentAttributes();
    }

    // Update is called once per frame
    public override void Update()
    {

        base.Update();
        DisPlayExp = playerExperience;
        UpdatePlayerBattleUIandState();
        ProcessPlayerSkillChoice();
       // PrintAllAvailableSkillsIf_P_isPressed();
        PrintCurrentPlayerExpLVIf_E_isPressed();
    }


    void UpdatePlayerBattleUIandState(){
        if (inBattle) {
            //Debug.Log("current energy = " + currentEnergy);

            ShowBattleUI();
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
                if (canMove) {
                    isIdle = false;
                } else {
                    isIdle = true;
                }
            } else {
                isIdle = true;
            }
        } else {
            healthBar.gameObject.SetActive(false);
            energyBar.gameObject.SetActive(false);
        }
    }

    void ProcessPlayerSkillChoice(){
        if(Input.GetKeyDown(KeyCode.Alpha1)){
           // Debug.Log("Chose skill 1");
            theChosenSkillIndex=1;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2)){
           // Debug.Log("Chose skill 2");
            theChosenSkillIndex=2;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3)){
           // Debug.Log("Chose skill 3");
            theChosenSkillIndex=3;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4)){
           // Debug.Log("Chose skill 4");
            theChosenSkillIndex=4;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha0)){
           // Debug.Log("Chose the dummy skill");
            theChosenSkillIndex=0;
        }
    }

//for debugging 
    void PrintAllAvailableSkillsIf_P_isPressed(){
        if(Input.GetKeyDown(KeyCode.P)){
           PrintAllAvailableSkills(); 
        }
    }

    void PrintCurrentPlayerExpLVIf_E_isPressed(){
        if(Input.GetKeyDown(KeyCode.E)){
            Debug.Log(
                "\nPlayer exp = " + playerExperience
                +"\n Player Lv = "+Lv);
        }
    }

//called by battleHandler : 
    public void PlayerGainExp(float exp ){
        if(exp>0)   playerExperience+=exp;
    }

    public void UpGradePlayerLevelAndAttributes(){
        //formula : exp = 25*L*L - 25L , L = (-b + sqrt(delta))/2a
        float delta = 25*25 -4*25*(-playerExperience);
        if(delta<0) return;
        int previousLv = Lv;
        Lv = (int)((25 + Mathf.Sqrt(delta)) / (2*25));
    
        bool RestoreHpEnergy = Lv>previousLv;   //if lv upped ->restore health
        UpdateCharacterCurrentAttributesWithLv(RestoreHpEnergy);
    }

    public void GainHalfHpEnergy(){
        currentHp += maxHp/2f;
        currentHp = Mathf.Min(currentHp,maxHp);

        currentEnergy += maxEnergy/2f;
        currentEnergy = Mathf.Min(currentEnergy,maxEnergy);

        currentIdleTime = 0;
        canMove = true;
        inBattle=false;
        timer=0;
        
        healthBar.SetHealth(currentHp);
        energyBar.SetEnergy(currentEnergy);
    }

//for file saving 
    public float GetCurrentExp(){
        return playerExperience;
    }

    public void SetCurrentExp(float exp){
        playerExperience = exp;
    }
}
