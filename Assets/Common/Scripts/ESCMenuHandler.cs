using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ESCMenuHandler : MonoBehaviour
{
    private bool active = false;

    public GameObject sceneTransitions;
    public GameObject escMenu;
    public GameObject audioManager;

    SceneHandler sceneHandler;
    SoundOptions soundOptions;
    SoundHandler soundHandler;

    private void Start()
    {
        sceneHandler = sceneTransitions.GetComponent<SceneHandler>();
        soundOptions = escMenu.GetComponent<SoundOptions>();
        soundHandler = audioManager.GetComponent<SoundHandler>();

        soundOptions.GetSliderVolumes();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        active = !active;
        escMenu.SetActive(active);
        GameManager.instance.Paused = active;

        if (active)
            soundHandler.PlaySFX(soundHandler.menuOpen);
        else
            soundHandler.PlaySFX(soundHandler.menuClose);
    }

    public void QuitToMenu(int targetScene)
    {
        sceneHandler.FadeToLevel(targetScene);
    }
}
