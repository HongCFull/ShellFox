using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomGeneration : MonoBehaviour
{
        
    [Range(0.0f , 100.0f)]
    public float  emptyAreaPercentage;

    [Tooltip("The ObjectSet e.g. trees")]
    public ObjectType[] objectType;

    public float[] objectTypePercentage;

    List<GameObject> objectList;

    // vertical (z) = z, horizontal (x) = x 
    [SerializeField] Terrain terrain;
    private NavMeshSurface surface;

    public Transform water;
    public int gridNumberX;   // the number of grid in x axis
    public int gridNumberZ;   // the number of grid in z axis

    private float girdSizeX;    // the size of a grid in x axis
    private float girdSizeZ;    // the size of a grid in z axis 
    private Grid[,] grids;
    
    // for noise operations
    [SerializeField] float noiseFrequencyX ;
    [SerializeField] float noiseFrequencyZ ;
    private float offsetX;
    private float offsetZ;
    
    // for distribution control
    private float noiseMax = Mathf.NegativeInfinity;
    private float noiseMin = Mathf.Infinity;
    private float noiseDelta ;
    private float[] noiseDistribution;
    private float[] noiseUpperBound_Type;
    private float emptyNoiseUpperBound;

    //for terrain height
    private int width ; //x
    [Tooltip("The Maximum Height of the Peak ")]
    public float depth ;  //y  
    private int length ; //z

    //for preventing characters to spawn in weired position
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    
    public AudioSource audioSource; 
    public AudioClip pop;

    void Awake()    //make sure it is executed earlier than the textureAssigner
    {        
        //initialization
        audioSource=GetComponent<AudioSource>();
        SetRandomNoiseOffset();
        InitializeGridData();
        InitializeTerrainData();  
        PreseveChactersSpawnPoint();  
        AllocateMemoryForDataMember();

        //Report Error If the inspector didnt set up correctly
        ReportIfMissingPresetParameter();

        //Setting Preset objects' bound interval
        SetNoise_MinMaxDeltaDistribution();
        RandomizedQuickSort(0,gridNumberX * gridNumberZ-1); //to sort the noise distribution
        SetNoiseValueUpperBoundOfType();
       
       //Generate
        GenerateTerrain();
        GenerateObjectsOnGrid();
        BuildTerrainNavMesh();
       // PrintSortedNoiseDistribution();
       // PrintNoiseUpperBounds();

    }

    void Update(){

    }

//Initializations
    void InitializeTerrainData(){
        width = (int) terrain.terrainData.size.x;
        length = (int) terrain.terrainData.size.z;
    }

    void InitializeGridData(){
        girdSizeX = terrain.terrainData.size.x / gridNumberX;
        girdSizeZ = terrain.terrainData.size.z / gridNumberZ;

        grids = new Grid[gridNumberZ,gridNumberX];  
        for(int z=0 ; z<gridNumberZ ; z++){
            for(int x=0 ; x<gridNumberX ; x++){ 
                grids[x,z] = new Grid(x, z, girdSizeX , girdSizeZ );
            }
        }
    }

    void PreseveChactersSpawnPoint(){   //prevent generating obj at player & enemy spawnPoint  
        int Xindex_Player =0 ,Zindex_Player =0 , 
        Xindex_Enemy=0,Zindex_Enemy =0;
        GetGridIndexByWorldPos(playerSpawnPoint.position,ref Xindex_Player,ref Zindex_Player);
        GetGridIndexByWorldPos(enemySpawnPoint.position,ref Xindex_Enemy,ref Zindex_Enemy);
        SetNeighbourGirdsUnspawnable(Xindex_Player,Zindex_Player);
        SetNeighbourGirdsUnspawnable(Xindex_Enemy,Zindex_Enemy);

       // grids[Xindex_Player,Zindex_Player].spawnable=false;
       // grids[Xindex_Enemy,Zindex_Enemy].spawnable=false;
       // Debug.Log("Player spawn at Grid (x,z) ="+Xindex_Player+" , "+Zindex_Player);
       // Debug.Log("Enemy spawn at Grid (x,z) ="+Xindex_Enemy+" , "+Zindex_Enemy);
    }

    void SetNeighbourGirdsUnspawnable(int x,int y){
        for(int dx = -1; dx<=1 ; dx++){
            for(int dy =-1 ; dy<=1 ;dy++){
                if( x+dx<0 || x+dx>gridNumberX || y+dy<0 || y+dy>gridNumberZ )       //note that gridNumberX and gridNumberZ shd be the same
                    continue;
                grids[x+dx,y+dy].spawnable=false;
            }
        }
    }

    void AllocateMemoryForDataMember(){
        objectList=new List<GameObject>();
        noiseDistribution = new float[gridNumberX * gridNumberZ];
        noiseUpperBound_Type= new float [objectTypePercentage.Length];
    }

//Set functions

    void SetRandomNoiseOffset(){
        offsetX=Random.Range(0f,99999f);
        offsetZ=Random.Range(0f,99999f);
    }

    void SetNoise_MinMaxDeltaDistribution(){
        for(int z=0 ; z<gridNumberZ ; z++){
            for(int x=0 ; x<gridNumberX ; x++){ 
                float result =GetNoiseResult(x,z);
                if(result<noiseMin)
                    noiseMin= result;

                if(result > noiseMax)
                    noiseMax=result;

                grids[x,z].noiseResult = result;    //store individual result
                noiseDistribution[x+ z *(gridNumberX)] = result;    //the distribution will be sorted later <- just to get the interval
            }
        }
        noiseDelta = noiseMax - noiseMin;
    }

    void SetNoiseValueUpperBoundOfType(){
        int initindex = (int)(emptyAreaPercentage/100.0f * (gridNumberX * gridNumberZ-1));
        emptyNoiseUpperBound = noiseDistribution[initindex];
        for(int i =0 ; i<objectTypePercentage.Length ; i++){  //setting up the noise upperbound value for type i
            float percentile = emptyAreaPercentage;
            for(int j= 0 ; j<=i ; j++){
                percentile += objectTypePercentage[j];
                if(percentile > 100.0f) 
                    percentile = 100f;
                int index= (int) ((percentile/100.0f) *(gridNumberX * gridNumberZ-1)) ;
                noiseUpperBound_Type[i]= noiseDistribution[index] ;
            }

        }
    }
//Get functions
    public float GetNoiseResult(int xIndex, int zIndex){
        float zComponent = (float) zIndex/gridNumberZ * noiseFrequencyZ;
        float xComponent = (float) xIndex/gridNumberX *noiseFrequencyX;
        float result = Mathf.Exp(Mathf.PerlinNoise(xComponent + offsetX, zComponent+ offsetZ)) /Mathf.Exp(1);
        return result;
    }

    //precondition : the noiseDistribution is sorted
    int GetPresetType( float noiseResult){
       
        for(int i=0; i<objectType.Length ; i++){
            if(i==0 ){
                if(IsBetweenRange(noiseResult,emptyNoiseUpperBound,noiseUpperBound_Type[0])){
                    return 0;
                }
                continue;
            }
            if (IsBetweenRange(noiseResult,noiseUpperBound_Type[i-1],noiseUpperBound_Type[i])){
                return i;
            }
        }
        return -1;
        
    }
    
    // return true if input belongs to [lowerBound, upperBound)
    bool IsBetweenRange(float input,float lowerBound, float upperBound){
        return input>=lowerBound && input<upperBound? true : false;
    }

    TerrainData GetTerrainData(TerrainData terrainData){
        terrainData.size = new Vector3 (width , depth , length);
        terrainData.heightmapResolution = width ;
        
        float [,] newHeightMap = new float [width,length];
        for(int x=0 ; x<width ; x++){
            for(int z= 0 ; z<length ; z++){
                float xComponent = (float) x/ width * noiseFrequencyX + offsetX;
                float zComponent = (float) z/ length * noiseFrequencyZ + offsetZ;
                newHeightMap[z,x] = Mathf.PerlinNoise(xComponent , zComponent);
            } 
        }
        terrainData.SetHeights(0,0,newHeightMap);
        return terrainData;
    }

    void GetGridIndexByWorldPos(Vector3 worldPos, ref int x , ref int z){
        x = (int) (worldPos.x / girdSizeX);
        z = (int) (worldPos.z / girdSizeZ);
    }

//Generate/Delete Functions:

    void GenerateTerrain(){
        terrain.terrainData = GetTerrainData(terrain.terrainData);
    }

    void BuildTerrainNavMesh(){
       // NavMeshBuilder.BuildNavMesh();
       surface = terrain.GetComponent<NavMeshSurface>();
       surface.BuildNavMesh();
    }

    void GenerateObjectsOnGrid(){
        for(int z=0 ; z<gridNumberZ ; z++){
            for(int x=0 ; x<gridNumberX ; x++){ 

                float result =GetNoiseResult(x,z);
                int type = GetPresetType(result);
                
                if(type<0 || type>objectType.Length || !grids[x,z].spawnable) //skip outside boundary or not spawnable grid
                    continue;

                //calculate the spawn coordinate of the object
                float objHeight = terrain.SampleHeight(grids[x,z].centerCoordinateInWorld);
                if(objHeight<=water.position.y) continue;   //cant generate on or below water
                Vector3 spawnCoordinate = new Vector3(0,objHeight,0) + grids[x,z].centerCoordinateInWorld ;
                
                //The noise value on that grid also affect the size of that object 
                float sizeOffset =  grids[x,z].noiseResult ; 
                
                GameObject obj = objectType[type].GenerateRandomObject_WorldCoordinate( spawnCoordinate , sizeOffset );

                if(obj != null){
                    objectList.Add(obj);
                    audioSource.PlayOneShot(pop,0.02f);
                 //   yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }

//Sorting algorithm
    void RandomizedQuickSort(int start,int end){
        if(start >= end)
            return;
        int sortedPivotIndex =  Partition(start,end);
        RandomizedQuickSort(start,sortedPivotIndex-1);
        RandomizedQuickSort(sortedPivotIndex+1,end);

    }

    int Partition(int start , int end){
        int initalPivot = (int) Random.Range(start,end);
        Swap(ref noiseDistribution[initalPivot], ref noiseDistribution[end]);   //now A[end] becomes our pivot
        float pivotValue = noiseDistribution[end];
        int i=start-1;

        for(int j = start; j<end ; j++){
            if(noiseDistribution[j]<=pivotValue){
                i++;
                Swap(ref noiseDistribution[i], ref noiseDistribution[j]);
            }
        }
        Swap(ref noiseDistribution[i+1], ref noiseDistribution[end]);
        return i+1;
    }

    void Swap(ref float a, ref float b){
        float temp = a;
        a=b;
        b=temp;
    }


//Error Report Functions
    void ReportIfMissingPresetParameter(){
        if(objectType.Length > objectTypePercentage.Length )
            Debug.LogError("Please Set the BoundPercentage for the preset type");
        else if(objectType.Length < objectTypePercentage.Length )
            Debug.LogError("Excessive BoundPercentage ");
    }
    
    void SpawDetectionTest(){
        for(int x = 0; x<gridNumberX ; x++){
            for(int z = 0; z <gridNumberZ ; z++){
                objectType[0].GenerateRandomObject_WorldCoordinate(grids[0,z].centerCoordinateInWorld,0f);
            }
        }
    }

    void PrintNoiseUpperBounds(){
        for(int i=0 ; i<objectType.Length ; i++){
            Debug.Log("Object Type ["+i+"] upper bound = "+ noiseUpperBound_Type[i]);
        }
    }

    void PrintSortedNoiseDistribution(){
        RandomizedQuickSort(0,gridNumberX * gridNumberZ-1);
        for(int x = 0; x <gridNumberX ; x++){
            for(int z=0 ; z<gridNumberZ ; z++){
                Debug.Log(noiseDistribution[x+ z *(gridNumberX)]);
            }
        }
    }

    //visualize the grid
    
/*
private void OnDrawGizmos() {
         for(int z=0 ; z<gridNumberZ ; z++){
            for(int x=0 ; x<gridNumberX ; x++){ 
               // Vector3 gridCenter = new Vector3( grids[x,z].worldCoordinateX + girdSizeX/2 , 0, grids[x,z].worldCoordinateZ + girdSizeZ/2);
                Gizmos.color = Color.yellow;
                Vector3 size = new Vector3(girdSizeX,1,girdSizeZ);
                Gizmos.DrawCube(grids[x,z].centerCoordinateInWorld,size );
            }
        }
    }
*/

}