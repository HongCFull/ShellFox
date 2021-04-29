using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//: MonoBehaviour
public abstract class BattleSkill : MonoBehaviour
{
    protected BattleType.Element skillElement;
    protected float power; 
    protected float energyCost;
    protected float coolDown;
    protected float castTime;
    protected bool currentlyAvailable;

    public GameObject EffectToSpawn;


    public BattleSkill(){
        //Debug.Log("Created battle skill class");
        currentlyAvailable=true;
    }

    private void Start() {
        float EffectSize= 0.75f;
        EffectToSpawn.gameObject.transform.localScale=new Vector3(EffectSize,EffectSize,EffectSize);    
    }

    public abstract BattleType.Element GetAttackElementType();
    public abstract float GetSkillPower();
    public abstract float GetAttackEnergyCost();
    public abstract float GetAttackCoolDown();
    public abstract float GetAttackCastTime();

    public void SetSkillToAvailable(){
        currentlyAvailable=true;
    }

    public void SetSkillToUnavailable(){
        currentlyAvailable=false;
    }

    public bool IsAvailabe(){
        return currentlyAvailable;
    }

    //should use startCoroutine to call this !
    public IEnumerator SetParticularSkilltoAvailable_AfterTime(float time){
        yield return new WaitForSeconds(time);
      //  Debug.Log("set to available after "+time+" s");
        SetSkillToAvailable();
    }

}
