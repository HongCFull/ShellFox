using UnityEngine;


//precondition: the first grid[0][0] always start at (0 0 0) in the world coordinate system
public class Grid 
{
//Input data :
    public float gridIndex_X;
    public float gridIndex_Z;

    public float gridSizeX;
    public float gridSizeZ;
    
    public bool spawnable = true;  // the grid is spawnable in default

//Calculated result :
    
    public float worldCoordinateX;
    public float worldCoordinateZ;
    public Vector3 centerCoordinateInWorld;
    public float noiseResult;

    public Grid(){}

    public Grid(float gridIndex_X, float gridIndex_Z, float gridSizeX , float gridSizeZ ){
        this.gridIndex_X = gridIndex_X;
        this.gridIndex_Z = gridIndex_Z;

        this.gridSizeX = gridSizeX;
        this.gridSizeZ = gridSizeZ;

        this.worldCoordinateX = gridIndex_X * gridSizeX ;
        this.worldCoordinateZ = gridIndex_Z * gridSizeZ ;

        this.centerCoordinateInWorld = new Vector3 (worldCoordinateX + gridSizeX/2 , 0 , worldCoordinateZ + gridSizeZ/2);
    }

    

}
