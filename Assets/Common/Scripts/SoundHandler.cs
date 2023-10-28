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
    public AudioClip mousePress;
    public AudioClip mouseRelease;

    [Header("------- Arcade --------")]

    public AudioClip BGMusicArcade;
    public AudioClip machineInteract;

    [Header("------ Zap N' Dash -------")]

    public AudioClip BGMusicZND;
    public AudioClip lightningStrike;
    public AudioClip playerElectrocute;
    public AudioClip OptionHover;
    public AudioClip OptionClick;
    public AudioClip ZDRoundGood;
    public AudioClip ZDRoundBad;

    [Header("----- Cable Conundrum ------")]

    public AudioClip BGMusicCC;
    public AudioClip BGMusicCCL;
    public AudioClip tileRotate;
    public AudioClip circuitCorrect;
    public AudioClip circuitIncorrect;
    public AudioClip doorClose;
    public AudioClip doorOpen;

    [Header("----- Common -----")]

    public AudioClip HiScoresMusic;
    public AudioClip NewHiScore;


    void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene == 3)
        {
            musicSource.clip = BGMusicArcade;
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

    public void PauseAll()
    {
        musicSource.Pause();
        sfxSource.Pause();
    }

    public void ResumeAll()
    {
        musicSource.Play();
        sfxSource.Play();
    }

    public void HiScores()
    {
        musicSource.Stop();
        musicSource.clip = HiScoresMusic;
        musicSource.Play();
    }

    public void ZNDMusic()
    {
        musicSource.Stop();
        musicSource.clip = BGMusicZND;
        musicSource.Play();
    }

    public void CCMusic()
    {
        musicSource.Stop();
        musicSource.clip = BGMusicCC;
        musicSource.Play();
    }

    public void CCLMusic()
    {
        musicSource.Stop();
        musicSource.clip = BGMusicCCL;
        musicSource.Play();
    }
}
