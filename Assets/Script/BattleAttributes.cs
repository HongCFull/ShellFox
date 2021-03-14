using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleAttributes : MonoBehaviour
{
    // Attributes for characters
    public BattleType.Element element;
    public float maxHp;
    public float maxEnergy;
    public float attack;
    public float restTime;

    public float currentHp;
    public float currentEnergy;
    public float currentIdleTime;

    // States
    public bool inBattle;
    public bool isIdle;
    public bool isAccel;
    public bool canMove;
    public bool canRecover;

    // UI elements
    public HealthBar healthBar;
    public EnergyBar energyBar;

    // Update is called once per frame
    void Update()
    {
        if (inBattle) {
            healthBar.gameObject.SetActive(true);
            energyBar.gameObject.SetActive(true);
            if (currentEnergy > 0) {
                canMove = true;
            }
            if (isIdle) {
                currentIdleTime += Time.deltaTime;
            } else {
                if (canMove) {
                    currentIdleTime = 0f;
                    canRecover = false;
                    currentEnergy -= 1f * (isAccel? 2f: 1f) * (currentEnergy <= 0? 0: 1);
                    if (currentEnergy <= 0) {
                        canMove = false;
                        currentEnergy = 0;
                    } else {
                        canMove = true;
                    }
                    energyBar.SetEnergy(currentEnergy);
                } else {
                    isIdle = true;
                }
            }
            if (currentIdleTime >= restTime) {
                canRecover = true;
            }
            if (canRecover) {
                currentEnergy += (currentEnergy >= maxEnergy? 0: maxEnergy * 0.01f);
                if (currentEnergy > maxEnergy) {
                    currentEnergy = maxEnergy;
                }
                energyBar.SetEnergy(currentEnergy);
            }
        } else {
            currentHp = maxHp;
            currentEnergy = maxEnergy;
            currentIdleTime = 0;

            canMove = true;

            healthBar.SetMaxHealth(maxHp);
            energyBar.SetMaxEnergy(maxEnergy);
        }
    }
}
