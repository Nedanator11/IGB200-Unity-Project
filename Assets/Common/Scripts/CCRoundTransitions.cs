using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCRoundTransitions : MonoBehaviour
{
    public Animator ccAnimator;

    public GameObject ccManager;
    public GameObject cclManager;

    CTGameManager cc;
    CCLGameManager ccNormal;

    // Start is called before the first frame update
    void Start()
    {
        cc = ccManager.GetComponent<CTGameManager>();
        ccNormal = cclManager.GetComponent<CCLGameManager>();
    }

    public void CTNextRound()
    {
        ccAnimator.SetTrigger("DoorOpen");
        cc.NextRound();
    }

    public void CCLevelSelect()
    {
        ccAnimator.SetTrigger("ReturnOpen");
        ccNormal.RestartGame();
    }

    public void CCNextLevel()
    {
        ccAnimator.SetTrigger("DoorOpen");
    }

}
