using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneBGMPlayer : MonoBehaviour
{
    public AudioClip[] clips;
    public AudioClip victorySFX;
    public AudioClip defeatSFX;
    AudioSource audioSource;
    [Tooltip("Best so far around 0.02-0.035")][Range(0f,0.05f)]  public float volume;
    [Range(0.1f,5f)]  public float transitionTime;


    void Start(){
        audioSource = GetComponent<AudioSource>();
        int songIndex = Random.Range(0,clips.Length);
        audioSource.clip = clips[songIndex];
        audioSource.volume = 0f;
        audioSource.loop = true;
        audioSource.Play();
    }

    void Update() {
        if (audioSource.volume >=volume)    return;
        audioSource.volume += volume*Time.deltaTime/transitionTime;
    }

    public void PlayVictorSFX(){
        audioSource.Stop();
        audioSource.PlayOneShot(victorySFX);
    }

    public void PlayDefeatSFX(){
        audioSource.Stop();
        audioSource.PlayOneShot(defeatSFX);
    }

    public float GetVictorySFXLength(){
        return victorySFX.length;
    }

    public float GetDefeatSFXLength(){
        return defeatSFX.length;
    }

}


