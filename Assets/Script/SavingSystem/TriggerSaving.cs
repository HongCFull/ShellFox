using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSaving : MonoBehaviour
{
    [HideInInspector] public static GameObject obj;
    [SerializeField] GameObject player;
    // Start is called before the first frame update
    void Start(){
        PreserveThisObj();
    }

    void PreserveThisObj(){
        if(obj!=null){  //if there is already one gameobject
            Destroy(gameObject);
            return ;
        }
        obj = gameObject;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update(){
        if(Input.GetKeyDown(KeyCode.N)){
            Debug.Log("Saved player's exp and position ");
            SavingSystem.SavePlayer(player.GetComponent<PlayerAttributes>());
        }
        else if(Input.GetKeyDown(KeyCode.M)){
            Debug.Log("loading player's exp and position ");
            PlayerData data= SavingSystem.LoadPlayer();
            player.GetComponent<PlayerAttributes>().SetCurrentExp(data.playerExp);
            Vector3 playerPos = new Vector3(data.position[0],data.position[1],data.position[2]);
            player.transform.position = playerPos;
        }
    }
}
