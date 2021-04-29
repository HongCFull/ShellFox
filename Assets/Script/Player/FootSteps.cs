using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] stepClips;
    [SerializeField] private AudioClip[] jumpClips;
    [Range(0,1)] public float atkVol;
    [SerializeField] private AudioClip[] atkClips;

    private AudioSource audioSource;
    [Range(0,2f)] public float playFootStepsPeriod;


    void Start(){
        audioSource=GetComponent<AudioSource>();
    }

    public void PlayFootStep(){
        AudioClip clip = GetRandomStepClip();
        audioSource.PlayOneShot(clip);
    }
    
    public void PlayFootStep(float vol){
        AudioClip clip = GetRandomStepClip();
        audioSource.PlayOneShot(clip,vol);
    }

    public void PlayJumpSFX(){
        AudioClip clip = GetRandomJumpClip();
        audioSource.PlayOneShot(clip);
    }

    public void PlayJumpSFX(float vol){
        AudioClip clip = GetRandomJumpClip();
        audioSource.PlayOneShot(clip,vol);
    }

    public void PlayAtkSFX(){
        AudioClip clip = GetRandomAtkClip();
        audioSource.PlayOneShot(clip,atkVol);
    }

     public void PlayAtkSFX(float vol){
        AudioClip clip = GetRandomAtkClip();
        audioSource.PlayOneShot(clip,vol);
    }


    AudioClip GetRandomStepClip(){
        return stepClips[Random.Range(0,stepClips.Length)];
    }

    AudioClip GetRandomJumpClip(){
        return jumpClips[Random.Range(0,jumpClips.Length)];
    }

    AudioClip GetRandomAtkClip(){
        return atkClips[Random.Range(0,atkClips.Length)];
    }

}
