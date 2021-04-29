using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneBGMPlayer : MonoBehaviour
{
    AudioSource audioSource;
    AudioClip bgm;  
    private float bgmLength;
    private float accumulator;
    [Range(0.0f,0.05f)] public float Volume; 
    [Range(0.0f,5f)] public float timeDelay; 
    void Start(){
        StartCoroutine(PlayMainSceneBGM(timeDelay));
    }

    IEnumerator PlayMainSceneBGM(float t){
        yield return new WaitForSeconds(t);
        audioSource = GetComponent<AudioSource>();
        bgm = audioSource.clip;
        audioSource.volume = Volume;     // best range from (0.01 to 0.05)
        audioSource.Play();
    }

}
