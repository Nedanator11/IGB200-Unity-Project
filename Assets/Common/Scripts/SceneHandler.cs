using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{

    public Animator animator;
    private int sceneToLoad;

    public void FadeToLevel(int levelIndex)
    {
        sceneToLoad = levelIndex;
        animator.SetTrigger("FadeToBlack");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

}
