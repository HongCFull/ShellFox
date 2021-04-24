using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BillBoard : MonoBehaviour
{
    public Transform cam;

    void Start() {
        ReAssignDependencies();
    }

    void ReAssignDependencies(){
        if(SceneManager.GetActiveScene().name == "MainScene"){
            cam = GameObject.FindGameObjectWithTag("MainCamera").transform;    //reassign dependencies
        }
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
