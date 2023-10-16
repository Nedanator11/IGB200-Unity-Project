using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CCRoundTransitions : MonoBehaviour
{
    public Animator ccAnimator;

    public GameObject ccManager;
    public GameObject cclManager;
    public GameObject audioManager;

    CTGameManager cc;
    CCLGameManager ccNormal;

    SoundHandler soundHandler;

    // Start is called before the first frame update
    void Start()
    {
        cc = ccManager.GetComponent<CTGameManager>();
        ccNormal = cclManager.GetComponent<CCLGameManager>();
        soundHandler = audioManager.GetComponent<SoundHandler>();
    }

    public void CTNextRound()
    {
        soundHandler.PlaySFX(soundHandler.doorOpen);
        cc.NextRound();
    }

    public void CCLevelSelect()
    {
        ccNormal.RestartGame();
    }

    public void CCNextLevel()
    {
        soundHandler.PlaySFX(soundHandler.doorOpen);
        ccNormal.LoadNextLevel();
    }

    public void CCRetryLevel()
    {
        soundHandler.PlaySFX(soundHandler.doorOpen);
        ccNormal.RetryLevel();
    }

    public void CCDifficultySelect()
    {
        cc.RestartGame();
    }

    public void CCReturn()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        if (currentScene == 1)
        {
            cc.RestartGame();
        }
        else if (currentScene == 6)
        {
            ccNormal.RestartGame();
        }
    }
}
