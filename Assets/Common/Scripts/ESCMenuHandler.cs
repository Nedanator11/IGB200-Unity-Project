using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ESCMenuHandler : MonoBehaviour
{

    private bool active = false;

    public GameObject menu;
    SceneHandler sceneHandler;

    private void Start()
    {
        sceneHandler = GetComponent<SceneHandler>();
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
