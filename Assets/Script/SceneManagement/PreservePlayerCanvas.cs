using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreservePlayerCanvas : MonoBehaviour
{
    [HideInInspector] public static GameObject obj;
    
    void Start(){
        if(obj!=null){  //if there is already one gameobject
            Destroy(gameObject);
            return ;
        }

        obj = gameObject;
        DontDestroyOnLoad(gameObject);
        //sceneHandler.preservedObjList.Add(gameObject);
    }

    
}
