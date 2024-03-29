using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxAndRestoreHealth(float health){
        slider.maxValue = health;
        slider.value =health;
    }

    public void SetMaxHealth(float maxHealth){
        slider.maxValue = maxHealth;
    }

    public void SetHealth(float health){
        slider.value =health;
    }
}
