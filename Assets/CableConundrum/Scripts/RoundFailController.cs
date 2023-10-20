using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundFailController : MonoBehaviour
{
    public enum FailType
    {
        None,
        TimerElapsed,
        IncompleteCircuit,
        HazardFire,
        HazardWater,
        HazardFrayed
    }

    [Header("FailureMessages")]
    public GameObject TimerElapsed;
    public GameObject IncompleteCircuit;
    public GameObject HazardFire;
    public GameObject HazardWater;
    public GameObject HazardFrayed;

    //Shows the corresponding failure description
    public void ShowFailureDescription(FailType failType)
    {
        switch (failType)
        {
            case FailType.TimerElapsed:
                TimerElapsed.SetActive(true);
                break;

            case FailType.IncompleteCircuit:
                IncompleteCircuit.SetActive(true);
                break;

            case FailType.HazardFire:
                HazardFire.SetActive(true);
                break;

            case FailType.HazardWater:
                HazardWater.SetActive(true);
                break;

            case FailType.HazardFrayed:
                HazardFrayed.SetActive(true);
                break;

        }
    }

    //Resets (hides) all failure messages
    public void ResetFailureDescription()
    {
        if (TimerElapsed != null) TimerElapsed.SetActive(false);
        IncompleteCircuit.SetActive(false);
        HazardFire.SetActive(false);
        HazardWater.SetActive(false);
        HazardFrayed.SetActive(false);
    }
}
