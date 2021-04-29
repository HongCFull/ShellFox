using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

//Handle HP and Energy

public class CharacterBattleAttributes : MonoBehaviour
{
    // Use this to get the information of the opponent
    // Refer to the BattleHandler game object <- we should update that gameobject directly 
    public BattleHandler battleManager;

    // UI elements
    public HealthBar healthBar;
    public EnergyBar energyBar;

    // Attributes for characters Currently
    public BattleType.Element element;
    public int Lv;
    public float maxHp;
    public float maxEnergy;
    public float attack;
    public float defense;
    public float energyRecoverPeriod;


    [SerializeField] BattleSkill[] inspector_skills;    //It is Just for copying into skills[] for real usage, dont access it after instantiating!
   // [HideInInspector]
    public BattleSkill[] skills;

    [HideInInspector]public float currentHp;
    [HideInInspector]public float currentEnergy;
    [HideInInspector]public float currentIdleTime;

    // States
    [HideInInspector]public bool inBattle;
    [HideInInspector]public bool isIdle;
    [HideInInspector]public bool isAccel;
    [HideInInspector]public bool canMove;
    [HideInInspector]public bool canRecover;

    // for optimization
    protected bool hasTriggeredBattle=false;      //for reducing unnecessary reset every frame
    protected float timer;     //universal timer in update , please reset it after usage 


// assign the inspector_skills into skills
    virtual public void Start() {
        InstantiateSkills();
        ReAssignDependencies();
    }

    public void ReAssignDependencies(){
        battleManager = GameObject.FindGameObjectWithTag("BattleHandler").GetComponent<BattleHandler>();
    }

    //optimization for enemy ? only instantiate skills when trigger battle
    public void InstantiateSkills(){
        for(int i=0;i<inspector_skills.Length; i++){
            skills[i]= Instantiate(inspector_skills[i]);
            skills[i].transform.parent = gameObject.transform;
        }
    }

//only useful in update for most of the time
    bool TimeHasPassedFor(float t){

        timer+=Time.deltaTime;
        if(timer>=t){
            timer=0;
            return true;
        }
        return false;
    }

// Update is called once per frame
    virtual public void Update()
    {
        BattleUIHandle();
    }

    protected void BattleUIHandle(){
        if (inBattle) {
        //update current avaliable skill list
            hasTriggeredBattle=true;            //for function call optimization
            timer+=Time.deltaTime;      //recording battle time, responsible for recover energy

            ShowBattleUI();
            UpdateCanmoveState();

            if (canMove && !isIdle) {
               // Debug.Log("Enemy is Spending energy to walk");
                SpendMovementEnergy();
            }
           
            if (CanRecoverEnergyInBattle()) { //assumed in battle -> can rest
                RecoverEnergy();
            }
            UpdateEnergyBar();
        } 
        else {  //not in battle
            if(hasTriggeredBattle){
                ResetBattleAttributes_and_UI();
                hasTriggeredBattle=false;
            }
        }
    }
    
//Functions For Energy

    public void UpdateCanmoveState(){
         if (currentEnergy <= Mathf.Epsilon) {
            canMove = false;
            currentEnergy = 0;
        } else {
            canMove = true;
        }
    }

    protected void UpdateEnergyBar(){
        energyBar.SetEnergy(currentEnergy);
    }

    protected void RecoverEnergy(){
        currentEnergy += (currentEnergy >= maxEnergy? 0: maxEnergy * 0.1f) ;   //recover 1/10 of MAX energy per recoverPeriod
        if (currentEnergy > maxEnergy) {
            currentEnergy = maxEnergy;
        }
    }

    protected bool CanRecoverEnergyInBattle(){
        if(TimeHasPassedFor(energyRecoverPeriod) && inBattle){
            timer=0f;
            canRecover = true;
            return true; 
        }
        else{
            canRecover = false;
            return false; 
        }
    }

    protected void SpendMovementEnergy(){
        currentEnergy -= (isAccel? 2f: 1f) * (currentEnergy <= 0? 0: 1) * Time.deltaTime *5;   
    }

    public void LostEnergyBy(float usage){
        if(currentEnergy-usage<0){
            currentEnergy=0;
            UpdateEnergyBar();
            
        }
        else{
            currentEnergy-=usage;
            UpdateEnergyBar();
        }
    }

//Functions for Health

    protected void UpdateHealthBar(){
        healthBar.SetHealth(currentHp);
    }

    public void LostHpBy(float damage){
        if(currentHp-damage<0){
            currentHp=0;
            UpdateHealthBar();
            //dead
        }
        else{
            currentHp-=damage;
            UpdateHealthBar();
        }
    }

//set functions

    public void ResetBattleAttributes_and_UI(){
        currentHp = maxHp;
        currentEnergy = maxEnergy;
        currentIdleTime = 0;
        canMove = true;
        inBattle=false;
        timer=0;
        
        healthBar.SetMaxHealth(maxHp);
        energyBar.SetMaxEnergy(maxEnergy);
    }


// Get function 

    virtual public bool isDefeated(){
        return currentHp<=Mathf.Epsilon;
    }

//General Operation
    public void ShowBattleUI(){
        healthBar.gameObject.SetActive(true);
        energyBar.gameObject.SetActive(true);
    }

    public void HideBattleUI(){
        healthBar.gameObject.SetActive(false);
        energyBar.gameObject.SetActive(false);
    }

    
    //should be called by player attributes and enemy attributes 
    public void UpdateCharacterCurrentAttributes(){
        //use the base attribute && current lv,exp to calculate the current hp atk def energy
        // formula : stat = (sqrt(x)+1) * lv
        float ratio = Mathf.Sqrt(Lv)+1;
        if(Lv<=1) ratio =1;  //assign baseStat to Lv 0,1 char
        SpeciesAttribute baseStat = GetComponent<SpeciesAttribute>();
        maxHp = baseStat.baseHp*ratio;
        maxEnergy = baseStat.baseEnergy*ratio;
        attack = baseStat.baseAttack*ratio;
        defense = baseStat.baseDefense*ratio;

    }

//FOR DEBUGGING
    public virtual void PrintAllAvailableSkills(){
        for(int i=1; i<skills.Length;i++){
            if(skills[i].IsAvailabe()){
                Debug.Log("skill ["+i+"] is available");
            }
        }
    }
}