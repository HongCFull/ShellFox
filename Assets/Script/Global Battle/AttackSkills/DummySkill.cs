using UnityEngine;

public class DummySkill : BattleSkill
{
    private float Damage =0;
    private float EnergyCost=0;
    private float CoolDown=0;
    private float CastTime=0;

    public DummySkill(){
        EffectToSpawn=null;
        skillElement=BattleType.Element.ICE;
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




//DummySkill