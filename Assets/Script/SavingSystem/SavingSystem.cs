using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public static class SavingSystem {

    public static void SavePlayer(PlayerAttributes player ){
        BinaryFormatter formatter = new BinaryFormatter();
        string saveFilePath = Application.persistentDataPath + "/shellfox.dat";
        FileStream fileStream = new FileStream(saveFilePath,FileMode.Create);

        PlayerData data =new PlayerData(player.GetCurrentExp(),new SerializableVector3(player.transform.position));
        
        formatter.Serialize(fileStream,data);
        fileStream.Close();

        Debug.Log("written in file ");
    }

    public static PlayerData LoadPlayer(){
        string saveFilePath = Application.persistentDataPath + "/shellfox.dat";
        if(File.Exists(saveFilePath)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(saveFilePath,FileMode.Open);

            PlayerData data =formatter.Deserialize(fileStream) as PlayerData;
            fileStream.Close();
            return data;

        }else{
            Debug.Log("shellfox.sav not found in "+saveFilePath);
            return null;
        }
    }

    

}


