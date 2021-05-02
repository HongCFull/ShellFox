using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerData data = SavingSystem.LoadPlayer();
        if(data!=null)  SceneManager.LoadScene("MainScene");
        Invoke("StartGameScene",60f);
    }

    void StartGameScene(){
        SceneManager.LoadScene("MainScene");
    }
}
