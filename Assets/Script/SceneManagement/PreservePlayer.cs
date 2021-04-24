using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreservePlayer : MonoBehaviour
{
    public SceneHandler sceneHandler;
    [HideInInspector] public static GameObject obj;

    void Start(){
        if(obj!=null){  //if there is already one gameobject
            Destroy(gameObject);
            return ;
        }

        obj = gameObject;
        DontDestroyOnLoad(gameObject);
        sceneHandler.preservedObjList.Add(gameObject);
    }


    public void RecordPlayerWorldScenePos(){
        SceneHandler sceneHandler = GameObject.FindGameObjectWithTag("SceneHandler").GetComponent<SceneHandler>();
        sceneHandler.SetPlayerMainScenePosHolder(gameObject.transform.position);
    }
}
