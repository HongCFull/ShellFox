using UnityEngine;

public class WaterJetAttack : BattleSkill
{
    [SerializeField] float Damage ;
    [SerializeField] float EnergyCost ;
    [SerializeField] float CoolDown;
    [SerializeField] float CastTime;


    private void Awake() {
        skillElement=BattleType.Element.WATER;
        power=Damage;
        energyCost=EnergyCost;
        coolDown=CoolDown;
        castTime=CastTime; 
    }
    
    
    public override BattleType.Element GetAttackElementType(){
        return skillElement;
    }

    public override float GetSkillPower(){
        return power;
    }

    public override float GetAttackEnergyCost(){
        return energyCost;
    }

    public override float GetAttackCoolDown(){
        return coolDown;
    }   
  
    public override float GetAttackCastTime(){
        return castTime;
    }
        
}
