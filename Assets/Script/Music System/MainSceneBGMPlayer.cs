using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneBGMPlayer : MonoBehaviour
{
    AudioSource audioSource;
    private float accumulator;    
    [Tooltip("Best so far around 0.02-0.035")][Range(0f,0.05f)]  public float volume;
    [Range(0.1f,5f)]  public float transitionTime;

    void Start(){
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;
        audioSource.loop = true;
        audioSource.Play();
    }

    void Update() {
        if (audioSource.volume >=volume)    return;
        audioSource.volume += volume*Time.deltaTime/transitionTime;
    }



}
