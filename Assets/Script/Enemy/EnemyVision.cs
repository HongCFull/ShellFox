using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The Enemy Vision is created by using geometry & intersection algorithm

[ExecuteInEditMode]
public class EnemyVision : MonoBehaviour
{

    public int scanFrequency = 30;
    public LayerMask layers;    // what objects could be seen by enemy
    public LayerMask occlusionLayer;    //specify what gameobject will block the enemy's vision
    public List<GameObject> objects = new List<GameObject>();   //the objects currently inside the enemy's vision cone
    Collider[] colliders = new Collider[50];    //store the colider of the gameobject inside the vision cone

    int count;
    float scanInterval;
    float scanTimer;

    public Transform visionFrom ;   //refer to the starting point of the vision cone
    [Range(0,180)]public float ConeAngle;
    [Range(0,100)]public float ConeHeight;
    public float ConeRadius;
    public int segments = 10;

    [Tooltip("We use 3 eye ray (left mid right) to check if the enemy can see the player")]
    [Range(0,1f)]
    public float eyeRayWidth;

    public Color meshColor = Color.red;
    Mesh mesh;
    
//functions 
    void Start() {
        scanInterval = 1.0f/scanFrequency;
    }

    void Update(){
        scanTimer -= Time.deltaTime;
        if (scanTimer<0){
            scanTimer += scanInterval;
            ScanVisionCone();
        }
    }

    void ScanVisionCone(){
        count = Physics.OverlapSphereNonAlloc(transform.position, ConeRadius, colliders, layers, QueryTriggerInteraction.Collide );

        objects.Clear();
        for(int i=0; i< count; i++){
            if(colliders[i]!=null){
                GameObject obj = colliders[i].gameObject;
                if(CanSee(obj)){
                    objects.Add(obj);
                }
            }
        }
    }

    
    public bool CanSee(GameObject obj){
        Vector3 origin = transform.position;

        Vector3 originLeft = transform.position;
        originLeft.x-=eyeRayWidth;      // TEMPORARYLY indicates left right

        Vector3 originRight = transform.position;
        originRight.x += eyeRayWidth;

        Vector3 dest = obj.transform.position;
        Vector3 direction = dest- origin;
        
        //checking if above or below the vision cone
        if(direction.y<-ConeHeight/2 || direction.y > ConeHeight/2){
            return false;
        }
        else{
            direction.y=0;
            float deltaAngle = Vector3.Angle(direction, transform.forward);
            if(deltaAngle > ConeAngle){
                return false;
            }

            dest.y = origin.y;  // check in the horizontal level of the eye
            if((dest-origin).magnitude>ConeRadius ||
                Physics.Linecast(origin, dest, occlusionLayer)){

                return false;
            }
            return true;
        }
        
    }

    


    //create Vision cone by geometry verticies
    Mesh CreateWedgeMesh(){
        Mesh mesh = new Mesh();

       // int segments = 10;
        int numTriangles=(segments*4)+2 +2;
        int numVertices = numTriangles*3;

        Vector3[] vertices = new Vector3[numVertices];
        int [] triangles = new int [numVertices];

        //geometical vertices for the vision cone 
        Vector3 bottomCenter = Vector3.zero;
        bottomCenter.y -= ConeHeight/2;

        Vector3 bottomLeft = Quaternion.Euler(0,-ConeAngle,0) * Vector3.forward *ConeRadius;
        bottomLeft.y -= ConeHeight/2;

        Vector3 bottomRight = Quaternion.Euler(0,ConeAngle,0) * Vector3.forward *ConeRadius;
        bottomRight.y -= ConeHeight/2;

        Vector3 topCenter = bottomCenter+Vector3.up*ConeHeight;
        Vector3 topLeft = bottomLeft+Vector3.up*ConeHeight;
        Vector3 topRight = bottomRight+Vector3.up*ConeHeight;

        int vert=0;

        //left side (2 triangles)
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        //right side (2 triangles)
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = - ConeAngle;
        float deltaAngle = (ConeAngle*2) / segments;
        for(int i=0 ; i< segments ; ++i){

            bottomLeft = Quaternion.Euler(0,currentAngle,0) * Vector3.forward *ConeRadius;
            bottomLeft.y -= ConeHeight/2;

            bottomRight = Quaternion.Euler(0,currentAngle+deltaAngle,0) * Vector3.forward *ConeRadius;
            bottomRight.y -= ConeHeight/2;

            topLeft = bottomLeft+Vector3.up*ConeHeight;
            topRight = bottomRight+Vector3.up*ConeHeight;

            //far side (2 triangles)
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            //top cone
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            //bottom cone
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;

            currentAngle +=deltaAngle;
        }

        for(int i = 0 ; i< numVertices ; ++i){
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles=triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }

    private void OnValidate(){
        mesh = CreateWedgeMesh();
        scanInterval = 1.0f/scanFrequency;
    
    }
   
// ENABLE to highlight the gameObject which are seen by the enemy

     private void OnDrawGizmos() {
         if(mesh){
             Gizmos.color=meshColor;
             Gizmos.DrawMesh(mesh,transform.position,transform.rotation);
         }

       //  Gizmos.DrawWireSphere(transform.position,ConeRadius);
         for(int i =0 ; i< count ; i++){
             Gizmos.DrawSphere(colliders[i].transform.position,0.4f);
         }

         Gizmos.color= Color.green;
         foreach (var obj in objects){
             Gizmos.DrawSphere(obj.transform.position , 0.4f);
         }
     }

}
