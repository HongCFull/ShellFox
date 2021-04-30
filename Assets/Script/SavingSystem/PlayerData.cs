using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float playerExp;
    public float[] position;

    public PlayerData(float exp, SerializableVector3 pos){
        playerExp = exp;

        Vector3 vector = pos.ToVector();
        position = new float[3];
        position[0] = vector.x;
        position[1] = vector.y;
        position[2] = vector.z;
    }
}
