using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInput : MonoBehaviour
{
    private int currentScene;
    public Animator animator;
    SceneHandler sceneHandler;
    SoundHandler soundHandler;

    private bool clicked = false;

    // Start is called before the first frame update
    void Start()
    {
        soundHandler = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<SoundHandler>();
        sceneHandler = GetComponent<SceneHandler>();
        currentScene = sceneHandler.currentScene;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentScene != 0)
        {
            if (Input.GetMouseButton(0) && currentScene == 5 && clicked == false)
            {
                clicked = true;
                animator.SetTrigger("KeyPress");
                soundHandler.PlaySFX(soundHandler.machineInteract);
                sceneHandler.FadeToLevel(2);
            }
            if (Input.GetMouseButton(1))
            {
                sceneHandler.FadeToLevel(3);
            }
        }

        if (currentScene == 0)
        {
            if (clicked == false)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    animator.SetBool("RightMouseDown", true);
                    soundHandler.PlaySFX(soundHandler.mousePress);
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    animator.SetBool("RightMouseDown", false);
                    soundHandler.PlaySFX(soundHandler.mouseRelease);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    animator.SetBool("LeftMouseDown", true);
                    soundHandler.PlaySFX(soundHandler.mousePress);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    soundHandler.PlaySFX(soundHandler.mouseRelease);
                    animator.SetBool("LeftMouseDown", false);
                }

                if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
                {
                    clicked = true;
                    animator.SetTrigger("CircleBloom");
                    soundHandler.PlaySFX(soundHandler.gameStart);
                }
            }
        }
    }

    public void StartCableTileGame()
    {
        animator.SetTrigger("KeyPress");
        sceneHandler.FadeToLevel(1);
    }

    public void StartLightningSafetyGame()
    {
        animator.SetTrigger("KeyPress");
        sceneHandler.FadeToLevel(2);
    }
}
