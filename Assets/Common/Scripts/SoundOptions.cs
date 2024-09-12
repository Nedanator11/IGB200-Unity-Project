using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundOptions : MonoBehaviour
{

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;


    public void SetMusicVolume()
    {
        float volumeRaw = musicSlider.value;
        float volume = Mathf.Log10(volumeRaw) * 20;

        audioMixer.SetFloat("music", volume);

        PlayerPrefs.SetFloat("music", volumeRaw);
    }

    public void SetSFXVolume()
    {
        float volumeRaw = sfxSlider.value;
        float volume = Mathf.Log10(volumeRaw) * 20;

        audioMixer.SetFloat("sfx", volume);

        PlayerPrefs.SetFloat("sfx", volumeRaw);
    }

    public void GetSliderVolumes()
    {
        musicSlider.value = PlayerPrefs.GetFloat("music", 0.75f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfx", 0.75f);
    }
}
