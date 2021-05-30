using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreserveBattleSystem : MonoBehaviour
{
    [HideInInspector] public static GameObject obj;

    void Start(){
        if(obj!=null){  //if there is already one gameobject
            Destroy(gameObject);
            return ;
        }

        obj = gameObject;
        DontDestroyOnLoad(gameObject);
    }
        
}

//try to push from macbook
