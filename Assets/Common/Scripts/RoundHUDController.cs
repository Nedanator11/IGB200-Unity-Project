using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundHUDController : MonoBehaviour
{
    public GameObject TimerHUDObject;
    public GameObject ScoreHUDObject;

    //Sets the text of the timer HUD object to given value, rounded up to whole seconds
    public void SetTimerText(float value)
    {
        TimerHUDObject.GetComponent<TextMeshProUGUI>().text = "Time: " + Mathf.CeilToInt(value).ToString();
    }

    //Sets the text of the score HUD object to given value, rounded up to next whole number
    public void SetScoreText(float value)
    {
        ScoreHUDObject.GetComponent<TextMeshProUGUI>().text = "Score: " + Mathf.CeilToInt(value).ToString();
    }
}
