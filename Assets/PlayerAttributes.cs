using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttributes : MonoBehaviour
{
    public BattleAttributes playerBA;
    public float playerSpeed;
    public float playerAccel;
    public float playerJumpHeight;

    void InitializePlayerAttribute(){
        playerBA.element = BattleType.Element.WATER;
        playerBA.maxHp = 100f;
        playerBA.maxEnergy = 500f;
        playerBA.attack = 10f; 
        playerBA.restTime = 2f;
               
        playerBA.currentHp = playerBA.maxHp;
        playerBA.currentEnergy = playerBA.maxEnergy;
        playerBA.currentIdleTime = 0;

        playerBA.inBattle = false;
        playerBA.isIdle = true;
        playerBA.isAccel = false;
        playerBA.canMove = true;
        playerBA.canRecover = true;

        playerBA.healthBar.SetMaxHealth(playerBA.maxHp);
        playerBA.energyBar.SetMaxEnergy(playerBA.maxEnergy);

        playerSpeed = 20f;
        playerAccel = 2f;
        playerJumpHeight = 3f;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        InitializePlayerAttribute();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerBA.inBattle) {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
                if (playerBA.canMove) {
                    playerBA.isIdle = false;
                } else {
                    playerBA.isIdle = true;
                }
            } else {
                playerBA.isIdle = true;
            }
        } else {
            playerBA.healthBar.gameObject.SetActive(false);
            playerBA.energyBar.gameObject.SetActive(false);
        }
    }
}
