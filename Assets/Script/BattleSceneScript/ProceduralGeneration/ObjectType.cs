using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectType : MonoBehaviour
{
    public GameObject[] gameObjects;

//currently using
    public GameObject GenerateRandomObject_WorldCoordinate(Vector3 worldCoordinate ,float sizeOffset){
        int index = (int) Random.Range(0,gameObjects.Length);
        if(index <0 || index >gameObjects.Length){
                Debug.LogError("No Gameobject is set with the input Index : "+index);
                return null;
        }

        Quaternion Rotation = Quaternion.Euler(0,Random.Range(0f,360f),0);
        GameObject obj = Instantiate(gameObjects[index],worldCoordinate,Quaternion.identity );
        obj.transform.rotation = Rotation;
        float scale = sizeOffset;

        obj.transform.localScale += new Vector3(scale,scale,scale);
        /*if(!obj.GetComponent<SpawnDetection>().canSpawn ){
            Destroy(obj);
            return null;
        }*/
        obj.SetActive(true);
        return obj;
    }

    bool CanGenerateGameObjectOnCoordinate(GameObject obj, Vector3 worldCoordinate){
        return true;
    }
}
