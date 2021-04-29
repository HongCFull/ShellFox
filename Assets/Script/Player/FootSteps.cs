using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    private AudioSource audioSource;
    [Range(0,2f)] public float playFootStepsPeriod;


    void Start(){
        audioSource=GetComponent<AudioSource>();
    }

    public void PlayFootStep(){
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    AudioClip GetRandomClip(){
        return clips[Random.Range(0,clips.Length-1)];
    }
}
