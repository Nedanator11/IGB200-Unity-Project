using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SoundHandler : MonoBehaviour
{
    private int currentScene;


    [Header("------Audio Source-----")]

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("--------- UI ---------")]

    public AudioClip buttonHover;
    public AudioClip buttonClick;

    [Header("------- Arcade --------")]

    public AudioClip BGMusicArcade;
    public AudioClip machineInteract;

    [Header("------ Zap N' Dash -------")]

    public AudioClip BGMusicZND;

    [Header("----- Cable Conundrum ------")]

    public AudioClip BGMusicCC;


    void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene == 3)
        {
            musicSource.clip = BGMusicArcade;
        }

        else if (currentScene == 2)
        {
            musicSource.clip = BGMusicZND;
        }

        else if (currentScene == 1)
        {
            musicSource.clip = BGMusicCC;
        }

        if (musicSource.clip != null) 
        {
            musicSource.Play();
        }
        
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }



}
