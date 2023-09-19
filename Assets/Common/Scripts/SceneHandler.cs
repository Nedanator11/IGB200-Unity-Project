using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{

    public Animator animator;
    private int sceneToLoad;
    public int currentScene;

    public void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }


    public void FadeToLevel(int levelIndex)
    {
        sceneToLoad = levelIndex;
        animator.SetTrigger("FadeToBlack");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitApp()
    {
        Application.Quit();
    }

}
