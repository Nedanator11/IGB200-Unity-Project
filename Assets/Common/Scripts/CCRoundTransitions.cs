using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCRoundTransitions : MonoBehaviour
{
    public Animator ccAnimator;
    public GameObject ccManager;
    CTGameManager cc;

    // Start is called before the first frame update
    void Start()
    {
        cc = ccManager.GetComponent<CTGameManager>();
    }

    public void CTNextRound()
    {
        ccAnimator.SetTrigger("DoorOpen");
        cc.NextRound();
    }

}
