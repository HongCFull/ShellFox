using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerBattle : MonoBehaviour
{
    // For UI elements
    public HealthBar healthBar;

    // For Player Battle Attribute
    public float maxPlayerHp;
    public float currentPlayerHp;
    public float playerAttack;
    public float playerEnergy;
    public BattleType.Type playertype;

    void InitializeAttribute(){
        maxPlayerHp =100;
        currentPlayerHp=maxPlayerHp;
        playerAttack=10; 
        playerEnergy=50;
        playertype=BattleType.Type.WATER;
   
        //for UI
        healthBar.SetMaxHealth(maxPlayerHp);
    }

    void Start()
    {
        InitializeAttribute();
       
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)){    //testing: player is taking dmg
            currentPlayerHp-=20;
            healthBar.SetHealth(currentPlayerHp);
        }
        
    }
}
