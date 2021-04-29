using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    [SerializeField]int xSize =10;
    [SerializeField]int zSize=10;
    [SerializeField] [Range(0,0.999f)]float xScale=0.3f;
    [SerializeField] [Range(0,0.999f)]float yScale=0.3f;
    [SerializeField]float totalNoiseScale=2f;

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;


    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();  //create a new mesh object
        GetComponent<MeshFilter>().mesh = mesh;   //store the mesh in meshfilter

        CreateShape();        
        UpdateMesh();
    }

    void CreateShape()
    {
        //capture all geometry vertices
        vertices= new Vector3[(zSize+1)*(xSize+1)];

        for(int z=0; z<=zSize; z++){
            for(int x=0 ; x<= xSize ; x++){
                float y = Mathf.PerlinNoise(x*xScale, z*yScale)*2f;
                vertices[z*(xSize+1)+x] = new Vector3(x,y,z);
            }
        }

        triangles = new int [xSize*zSize*2*3];  // 1 by 1 has 2 triangles, each triangle store 3 pt

        for(int z=0, numTriangle=0; z<zSize; z++){
            for(int x=0 ; x< xSize ; x++){          //render a 1x1 square for each coordinate

                    triangles[numTriangle + 0] = (z)*(xSize+1)+x;
                    triangles[numTriangle + 1] = (z+1)*(xSize+1)+x;
                    triangles[numTriangle + 2] = (z)*(xSize+1)+(x+1);

                    triangles[numTriangle + 3] = (z)*(xSize+1)+(x+1);
                    triangles[numTriangle + 4] = (z+1)*(xSize+1)+ x;
                    triangles[numTriangle + 5] = (z+1)*(xSize+1)+(x+1);
                numTriangle+=6;
            }
        }
    }


    void UpdateMesh(){
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
