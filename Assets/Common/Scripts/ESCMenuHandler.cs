using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ESCMenuHandler : MonoBehaviour
{
    private bool active = false;

    public GameObject menu;
    public GameObject sceneTransitions;

    SceneHandler sceneHandler;
    SoundOptions soundOptions;

    private void Start()
    {
        sceneHandler = sceneTransitions.GetComponent<SceneHandler>();
        soundOptions = menu.GetComponent<SoundOptions>();

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
        menu.SetActive(active);
        GameManager.instance.Paused = active;
    }

    public void QuitToMenu(int targetScene)
    {
        sceneHandler.FadeToLevel(targetScene);
    }
}
