using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundHandler : MonoBehaviour
{

    [Header("------Audio Source-----")]

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("--------- UI ---------")]

    public AudioClip buttonHover;
    public AudioClip buttonClick;

    [Header("------- Arcade --------")]

    public AudioClip BGMusicArcade;
    public AudioClip machineInteract;



    void Start()
    {
        musicSource.clip = BGMusicArcade;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }



}
