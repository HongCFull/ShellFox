using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTarget : MonoBehaviour
{
    Camera mainCamera;
    Ray ray;
    RaycastHit hitinfo;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera=Camera.main;
    }

    // Update the target that player will shoot at
    void Update()
    {
        ray.origin=mainCamera.transform.position;
        ray.direction=mainCamera.transform.forward;

        if(Physics.Raycast(ray, out hitinfo)){  
            transform.position = hitinfo.point;
        }
        else{       //aiming towards the air
            transform.position = ray.origin + ray.direction * 1000.0f;
        }
    }
}
