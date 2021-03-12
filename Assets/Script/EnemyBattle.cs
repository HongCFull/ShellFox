using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattle : MonoBehaviour
{
    public float enemyHp;
    public float enemyAttack;
    public float enemyEnergy;
    
    public BattleType.Type enemyType;

    void InitializeAttribute(){
        enemyHp =100;
        enemyAttack=10; 
        enemyEnergy=50;

        enemyType=BattleType.Type.FIRE;
    }

    void Start()
    {
        InitializeAttribute();
        if(enemyType == BattleType.Type.FIRE ){
            Debug.Log("enemy Is fire type");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
