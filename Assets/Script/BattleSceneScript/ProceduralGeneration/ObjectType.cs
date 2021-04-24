using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectType : MonoBehaviour
{
    public GameObject[] gameObjects;
    public float minHeightOffset;
    public float maxHeightOffset;
//currently using
    public GameObject GenerateRandomObject_WorldCoordinate(Vector3 worldCoordinate ,float sizeOffset){
        int index = (int) Random.Range(0,gameObjects.Length);
        if(index <0 || index >gameObjects.Length){
                Debug.LogError("No Gameobject is set with the input Index : "+index);
                return null;
        }
        
        if(minHeightOffset>maxHeightOffset){
            float temp = minHeightOffset;
            minHeightOffset = maxHeightOffset;
            maxHeightOffset = temp;
        }
        float heightOffset = Random.Range(minHeightOffset,maxHeightOffset);
        worldCoordinate.y += heightOffset;

        GameObject obj = Instantiate(gameObjects[index],worldCoordinate,Quaternion.identity );
        
        Quaternion Rotation = Quaternion.Euler(0,Random.Range(0f,360f),0);
        obj.transform.rotation = Rotation;
        
       
        float scale =  Mathf.Clamp(sizeOffset,0,2f);
        obj.transform.localScale += new Vector3(scale,scale,scale);
        
        obj.SetActive(true);
        return obj;
    }

    bool CanGenerateGameObjectOnCoordinate(GameObject obj, Vector3 worldCoordinate){
        return true;
    }
}
