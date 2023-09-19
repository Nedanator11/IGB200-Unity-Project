using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInput : MonoBehaviour
{
    private int currentScene;
    public Animator animator;
    SceneHandler sceneHandler;

    // Start is called before the first frame update
    void Start()
    {
        sceneHandler = GetComponent<SceneHandler>();
        currentScene = sceneHandler.currentScene;
    }

    // Update is called once per frame
    void Update()
    {

        if (currentScene != 0)
        {
            if (Input.GetMouseButton(0))
            {
                animator.SetTrigger("KeyPress");
                if (currentScene == 4) //Cable Tile
                {
                    sceneHandler.FadeToLevel(1);
                }
                if (currentScene == 5) // Lightning Safety
                {
                    sceneHandler.FadeToLevel(2);
                }
            }
            if (Input.GetMouseButton(1))
            {
                sceneHandler.FadeToLevel(3);
            }
        }

        if (currentScene == 0)
        {
            if (Input.GetMouseButtonDown(1))
            {
                animator.SetBool("RightMouseDown", true);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                animator.SetBool("RightMouseDown", false);
            }

            if (Input.GetMouseButtonDown(0))
            {
                animator.SetBool("LeftMouseDown", true);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                animator.SetBool("LeftMouseDown", false);
            }

            if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
            {
                sceneHandler.FadeToLevel(3);
                animator.SetTrigger("CircleBloom");
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
