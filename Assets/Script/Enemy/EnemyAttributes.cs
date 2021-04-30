using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttributes : CharacterBattleAttributes
{
    public float enemySpeed;
    public float attackArea;
    private int theChosenSkillIndex=0;    
    public float expGainedByPlayer;

    [Tooltip("The minimum distance keep enemy and player away")]
    public float distanceAwayFromPlayer;
    public float randWanderingPeriod;

//? ref to EnemyChaser Script
// minimax and choose the skill -> PBR

    void ShowAttackInfo(){
        for(int i=0 ; i <skills.Length;i++){
            Debug.Log(
            "Element :" + skills[i].GetAttackElementType() + "\n" +
            "Damage :" + skills[i].GetSkillPower() + "\n" +
            "Energy Cost :" + skills[i].GetAttackEnergyCost());
        }
    }

    void SetEnemyCurrentAttributes(){

        currentHp = maxHp;
        currentEnergy = maxEnergy;
        currentIdleTime = 0;

        inBattle = false;
        isIdle = true;
        isAccel = false;
        canMove = true;
        canRecover = true;

        healthBar.SetMaxHealth(maxHp);
        energyBar.SetMaxEnergy(maxEnergy);

        enemySpeed = 5f;

    }

    
    public override void Start()
    {       
        base.Start();
        UpdateCharacterCurrentAttributes();
        SetEnemyCurrentAttributes();
        
    }

    public override void Update()
    { 
        base.Update();
       // ReportIfCharacterIsDefeated();
    }


    public override void PrintAllAvailableSkills(){
        for(int i=1; i<skills.Length;i++){
            if(skills[i].IsAvailabe()){
                Debug.Log("skill ["+i+"] is available" + "\n" 
                +"Deal Damage ="+ battleManager.GetSkillDamageToTarget(skills[i],TargetIs.PLAYER));
            }
        }
    }
    

    // should be the one which is always called by the statemachine  
    public void UpdateAttackSkillIndex(){
        int depth = 3;
        hasChoseSkill=false;
        ChooseAttackSkillByMiniMax(battleManager.player.currentHp , battleManager.player.currentEnergy,
                                    this.currentHp , this.currentEnergy,
                                    depth , Mathf.NegativeInfinity , Mathf.Infinity , true);
        if(!hasChoseSkill){
            Debug.Log("EnemyAttributes::UpdateAttackSkillIndex Skill isn't chosen by enemy");
            theChosenSkillIndex=0;
        }
    }

    public int GetSkillIndex(){
        return theChosenSkillIndex;
    }

// note: Enemy is defined as the Global Enemy
// Player is really the player who is playing the game

    bool hasChoseSkill = false;
    private float ChooseAttackSkillByMiniMax(float currentPlayerHp, float currentPlayerEnergy , 
                                            float currentEnemyHp, float currentEnemyEnergy,
                                            int depth, float alpha, float beta, bool IsEnemyTurn){

        if(depth==0){   //base case
            return HeuristicEvaluation_1(currentPlayerHp, currentPlayerEnergy, currentEnemyHp, currentEnemyEnergy);
        }

        if(IsEnemyTurn){
            float maxEval=Mathf.NegativeInfinity;
            for(int i =1 ; i<skills.Length ; i++){      //note that skills[0] is always the dummy skill (the unavailable state)
                if(skills[i].IsAvailabe() && HaveEnoughEnergyToCastSkill(i,currentEnemyEnergy) ){     //for all available skills
                    float nextPlayerHp = currentPlayerHp - battleManager.GetSkillDamageToTarget(skills[i],TargetIs.PLAYER);
                    float nextEnemyEnergy = currentEnemyEnergy-skills[i].GetAttackEnergyCost();
                    float eval = ChooseAttackSkillByMiniMax(nextPlayerHp, currentPlayerEnergy, currentEnemyHp, nextEnemyEnergy, depth-1,alpha,beta,false );
                    if(eval>maxEval){
                        theChosenSkillIndex=i;
                        hasChoseSkill=true;
                        maxEval=eval;
                    }
                    alpha = Mathf.Max(alpha,eval);
                    if(beta <= alpha)
                        break;
                }
            }
            return maxEval;
        }
        
        else{   //player turn
            float minEval=Mathf.Infinity;
            //always assume player can cast the skills
            for(int i =1 ; i<battleManager.player.skills.Length; i++){   //note that skills[0] is always the dummy skill (the unavailable state)
                float nextEnemyHp = currentEnemyHp - battleManager.GetSkillDamageToTarget(battleManager.player.skills[i],TargetIs.ENEMY);
                float nextPlayerEnergy = currentPlayerEnergy - battleManager.player.skills[i].GetAttackEnergyCost();
                float eval = ChooseAttackSkillByMiniMax(currentPlayerHp, nextPlayerEnergy, nextEnemyHp, currentEnemyEnergy, depth-1,alpha,beta,true);
                if(eval<minEval){
                    minEval=eval;
                }
                beta = Mathf.Min(beta,eval);
                if(beta <= alpha)
                    break;
            }
            return minEval;
        }
    }

    float AggresiveFactor =0.4f;
    float Strategic_EnergyFactor=0.3f;

    //an evaluation to difference between the "Sources of enemy" to "Sources of player"
    private float HeuristicEvaluation_1(float estimatedPlayerHp, float estimatedPlayerEnergy , 
        float estimatedEnemyHp, float estimatedEnemyEnergy){

            return RandomnessEvaluation() *(
                AggresiveFactor * (estimatedEnemyHp-estimatedPlayerHp)
                +Strategic_EnergyFactor * (estimatedEnemyEnergy-estimatedPlayerEnergy) );

    }

    private float RandomnessEvaluation(){
       // return Random.Range(0.75f,1f);
        return Random.Range(1f,1f);

    }

//Helper Functions

    private bool HaveEnoughEnergyToCastSkill(int index, float currentEnergy){
        return skills[index].GetAttackEnergyCost()<currentEnergy;
    }

    /*
    public void ReportIfCharacterIsDefeated() {
        if(isDefeated()){
            battleManager.Operation_CharacterDefeated(this);
        }
    }
    */
}
 