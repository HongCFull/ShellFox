using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSaving : MonoBehaviour
{
    [HideInInspector] public static GameObject obj;
    [SerializeField] GameObject player;
    private SceneHandler sceneHandler;
    // Start is called before the first frame update
    void Start(){
        sceneHandler = GameObject.FindGameObjectWithTag("SceneHandler").GetComponent<SceneHandler>();
        PreserveThisObj();
        Invoke("LoadPlayerData",0.05f);
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
        if(Input.GetKeyDown(KeyCode.N))
            SavePlayerData();
        
        if(Input.GetKeyDown(KeyCode.M))
            LoadPlayerData();
        
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(sceneHandler.IsInMainScene())
                SavePlayerData();
            Application.Quit();
        }
        
        UpgradeDifficultyWithStage();
    }

    public void SavePlayerData(){
        Debug.Log("Saved player's exp and position ");
        SavingSystem.SavePlayer(player.GetComponent<PlayerAttributes>());
    }

    public void LoadPlayerData(){
        if(sceneHandler.IsInBattleScene())  return;
        Debug.Log("loading player's exp and position ");
        PlayerData data= SavingSystem.LoadPlayer();
        if(data==null)  return;
        player.GetComponent<PlayerAttributes>().SetCurrentExp(data.playerExp);
        player.GetComponent<PlayerAttributes>().UpGradePlayerLevelAndAttributes();
        Vector3 playerPos = new Vector3(data.position[0],data.position[1],data.position[2]);
        player.gameObject.SetActive(true);
        player.GetComponent<CharacterController>().enabled=false;
        player.transform.position = playerPos;
        player.GetComponent<CharacterController>().enabled=true;
        
    }

//JUST FOR THE CONVINIENCE OF SEEING THOSE ENEMIES IN HIGHER STAGE
    void UpgradeDifficultyWithStage(){
        if(Input.GetKeyDown(KeyCode.I)){    //stage 1
            player.GetComponent<PlayerAttributes>().SetCurrentExp(0);
            player.GetComponent<PlayerAttributes>().UpGradePlayerLevelAndAttributes();
        }
        if(Input.GetKeyDown(KeyCode.O)){    //stage 2
            player.GetComponent<PlayerAttributes>().SetCurrentExp(9501);
            player.GetComponent<PlayerAttributes>().UpGradePlayerLevelAndAttributes();
        }
        if(Input.GetKeyDown(KeyCode.P)){    //stage 3
            player.GetComponent<PlayerAttributes>().SetCurrentExp(49501);
            player.GetComponent<PlayerAttributes>().UpGradePlayerLevelAndAttributes();
        }
    }
}
