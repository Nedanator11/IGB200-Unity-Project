using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ESCMenuHandler : MonoBehaviour
{
    private bool active;
    public bool menuBase;

    public GameObject sceneTransitions;
    public GameObject escMenu;
    public GameObject audioManager;

    SceneHandler sceneHandler;
    SoundOptions soundOptions;
    SoundHandler soundHandler;

    Animator animator;

    private void Start()
    {
        sceneHandler = sceneTransitions.GetComponent<SceneHandler>();
        soundOptions = escMenu.GetComponent<SoundOptions>();
        soundHandler = audioManager.GetComponent<SoundHandler>();

        soundOptions.GetSliderVolumes();

        animator = escMenu.GetComponent<Animator>();

        active = false;
        menuBase = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuBase)
            {
                TogglePause();
            }
            else
            {
                animator.SetTrigger("ESCReturn");
                menuBase = true;
            }
        }
    }

    public void TogglePause()
    {
        active = !active;

        if (active)
        {
            soundHandler.PauseAll();
            escMenu.SetActive(true);
            animator.SetTrigger("ESCOpen");
            soundHandler.PlaySFX(soundHandler.menuOpen);
            menuBase = true;
        }
        else
        {
            // Sets !active in animation
            animator.SetTrigger("ESCClose");
            soundHandler.PlaySFX(soundHandler.menuClose);
            soundHandler.ResumeAll();
        }

        GameManager.instance.Paused = active;
    }

    public void QuitToMenu(int targetScene)
    {
        sceneHandler.FadeToLevel(targetScene);
    }

    public void ESCOptions()
    {
        menuBase = false;
        animator.SetTrigger("ESCOptions");
    }

    public void ESCHelp()
    {
        menuBase = false;
        animator.SetTrigger("ESCHelp");
    }

    public void ESCQuit()
    {
        menuBase = false;
        animator.SetTrigger("ESCQuit");
    }

    public void ESCReturn()
    {
        menuBase = true;
        animator.SetTrigger("ESCReturn");
    }
}
