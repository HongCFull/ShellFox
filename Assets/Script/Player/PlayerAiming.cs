using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAiming : MonoBehaviour
{
    public Transform rayCastOrigin;
    public Transform lookAtPosition;

    Ray ray;
    RaycastHit hitInfo;

    // Update is called once per frame
    void Update()
    {
        ShootRay();
    }

    void ShootRay(){
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            ray.origin=rayCastOrigin.position;
            ray.direction=lookAtPosition.position-rayCastOrigin.position;
            Debug.DrawLine(ray.origin,lookAtPosition.position, Color.red,1f);

        }
    }
}
        
