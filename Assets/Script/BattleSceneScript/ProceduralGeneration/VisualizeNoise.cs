using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VisualizeNoise : MonoBehaviour
{
    [SerializeField] RandomGeneration proceduralGen;
    [HideInInspector]public int height_Z;
    [HideInInspector]public int width_X;
    
    // Start is called before the first frame update
    void Start()
    {
        width_X = proceduralGen.gridNumberX;
        height_Z = proceduralGen.gridNumberZ;

        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
    }

    Texture2D GenerateTexture(){
        Texture2D texture = new Texture2D(width_X,height_Z);

        for(int z=0 ; z<height_Z ; z++){
            for(int x=0; x<width_X ; x++){
                
                float result = proceduralGen.GetNoiseResult(x,z);
                Color color = new Color(result,result,result);    
                texture.SetPixel(x,z,color);      
            }
        }
        texture.Apply();
        return texture;
    }

    // Update is called once per frame
    void Update()
    {        
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();

    }
}
