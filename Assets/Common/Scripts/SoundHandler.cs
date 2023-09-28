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
    public AudioClip buttonQuit;
    public AudioClip menuOpen;
    public AudioClip menuClose;
    public AudioClip gameStart;

    [Header("------- Arcade --------")]

    public AudioClip BGMusicArcade;
    public AudioClip machineInteract;
    public AudioClip machineMusic;
    public AudioClip ballCollide;

    [Header("------ Zap N' Dash -------")]

    public AudioClip BGMusicZND;
    public AudioClip playerWalk;
    public AudioClip lightningStrike;
    public AudioClip playerElectrocute;
    public AudioClip parkSoundscape;
    public AudioClip citySoundscape;

    [Header("----- Cable Conundrum ------")]

    public AudioClip BGMusicCC;
    public AudioClip tileRotate;
    public AudioClip circuitCorrect;
    public AudioClip circuitIncorrect;


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
