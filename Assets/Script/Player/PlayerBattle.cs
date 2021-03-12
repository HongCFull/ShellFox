using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerBattle : MonoBehaviour
{

    public float playerHp;
    public float playerAttack;
    public float playerEnergy;
    
    public BattleType.Type playertype;

    void InitializeAttribute(){
        playerHp =100;
        playerAttack=10; 
        playerEnergy=50;

        playertype=BattleType.Type.WATER;
    }

    void Start()
    {
        InitializeAttribute();
        if(playertype == BattleType.Type.WATER ){
            Debug.Log("Is water type");
        }
    }

    void Update()
    {
        
    }
}
