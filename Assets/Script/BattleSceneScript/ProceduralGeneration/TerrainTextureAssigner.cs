 using UnityEngine;
 using System.Collections;
 
public class TerrainTextureAssigner : MonoBehaviour{
    [System.Serializable]
    public class SplatHeights{
        public int textureIndex;
        public float startingHeight;
    }

    public Transform waterSurface;
    public float poolArea;
    
    [Tooltip("Make sure the ground mask for water is placed at index 0")]
    public SplatHeights[] splatHeights;

    void Start(){
        //assign height just above the water level to the next layer, more natural :)
        splatHeights[1].startingHeight = waterSurface.position.y+poolArea;
        ApplyProceduralGeneratedTexture();
    }

    //according to the definition in randomGeneration script:
    //widith = x , length = z
    void ApplyProceduralGeneratedTexture(){
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        float [, ,] splatmapData = new float [terrainData.alphamapWidth,
                                                terrainData.alphamapHeight,
                                                terrainData.alphamapLayers];
        
        for(int y =0 ; y<terrainData.alphamapHeight ; y++){
            for(int x=0; x<terrainData.alphamapWidth ; x++){    //for each "grid" in the terrain

                float terrainHeight = terrainData.GetHeight(y,x);
                float[] splat = new float [splatHeights.Length];    // the opactity of the texture layers
                //for each texture layer , assign their opacity on grid x,y
                for(int i =0 ; i< splatHeights.Length ; i++){   
                    float currentStartHeight = splatHeights[i].startingHeight;
                    float nextStartHeight = 0;
                    if(i+1 != splatHeights.Length){ //not available for the last texture layer
                        nextStartHeight = splatHeights[i+1].startingHeight;
                    }

                    if(i== splatHeights.Length-1 && terrainHeight >= currentStartHeight){   //Paint last layer
                        splat[i] =1;
                    }
                    else if(terrainHeight >=currentStartHeight && terrainHeight <= nextStartHeight) //paint transition texture layers
                        splat[i] = 1;
                }
                
                for(int j=0 ; j<splatHeights.Length; j++)
                    splatmapData[x,y,j]=splat[j];
            }
        }
        terrainData.SetAlphamaps(0,0,splatmapData);
    }

   
}