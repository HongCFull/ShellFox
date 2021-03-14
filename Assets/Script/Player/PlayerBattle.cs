using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerBattle : MonoBehaviour
{
    public PlayerAttributes attributes;

    void Start()
    {
       
    }

    void Update()
    {


        if (Input.GetKeyDown(KeyCode.P)) {    //testing: player is taking dmg
            attributes.playerBA.currentHp-=20;
            attributes.playerBA.healthBar.SetHealth(attributes.playerBA.currentHp);
        }
    }
}
