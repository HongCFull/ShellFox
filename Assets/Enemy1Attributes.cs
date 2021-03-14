using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1Attributes : MonoBehaviour
{
    public EnemyAttributes enemy1Attributes;
    
    // Start is called before the first frame update
    void Start()
    {
        enemy1Attributes.enemyBA.element = BattleType.Element.FIRE;
        enemy1Attributes.enemyBA.maxHp = 100;
        enemy1Attributes.enemyBA.maxEnergy = 250;
        enemy1Attributes.enemyBA.attack = 10; 
        enemy1Attributes.enemyBA.restTime = 2f;
               
        enemy1Attributes.enemyBA.currentHp = enemy1Attributes.enemyBA.maxHp;
        enemy1Attributes.enemyBA.currentEnergy = enemy1Attributes.enemyBA.maxEnergy;
        enemy1Attributes.enemyBA.currentIdleTime = 0;

        enemy1Attributes.enemyBA.inBattle = false;
        enemy1Attributes.enemyBA.isIdle = true;
        enemy1Attributes.enemyBA.isAccel = false;
        enemy1Attributes.enemyBA.canMove = true;
        enemy1Attributes.enemyBA.canRecover = true;

        enemy1Attributes.enemyBA.healthBar.SetMaxHealth(enemy1Attributes.enemyBA.maxHp);
        enemy1Attributes.enemyBA.energyBar.SetMaxEnergy(enemy1Attributes.enemyBA.maxEnergy);

        enemy1Attributes.enemySpeed = 5f;
        enemy1Attributes.alertArea = 15f;
        enemy1Attributes.battleTriggerArea = 5f;
        enemy1Attributes.attackArea = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
