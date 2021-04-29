using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] stepClips;
    [SerializeField] private AudioClip[] jumpClips;
    private AudioSource audioSource;
    [Range(0,2f)] public float playFootStepsPeriod;


    void Start(){
        audioSource=GetComponent<AudioSource>();
    }

    public void PlayFootStep(){
        AudioClip clip = GetRandomStepClip();
        audioSource.PlayOneShot(clip);
    }

    public void PlayJumpSFX(){
        AudioClip clip = GetRandomJumpClip();
        audioSource.PlayOneShot(clip);
    }

    AudioClip GetRandomStepClip(){
        return stepClips[Random.Range(0,stepClips.Length-1)];
    }

    AudioClip GetRandomJumpClip(){
        return jumpClips[Random.Range(0,stepClips.Length-1)];
    }
}
