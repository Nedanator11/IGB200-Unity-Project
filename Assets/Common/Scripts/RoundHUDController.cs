using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundHUDController : MonoBehaviour
{
    public Image TimerBarObject;
    public GameObject TimerHUDObject;
    public GameObject ScoreHUDObject;

    //Sets the text of the timer HUD object to given value, rounded up to whole seconds
    public void SetTimerText(float value)
    {
        TimerHUDObject.GetComponent<TextMeshProUGUI>().text = "Time: " + Mathf.CeilToInt(value).ToString();
    }

    //Sets the text of the score HUD object to given value, rounded up to next whole number
    public void SetScoreText(string text)
    {
        ScoreHUDObject.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void SetTimerBar(float fill)
    {
        if (TimerBarObject != null)
        {
            TimerBarObject.fillAmount = fill / 10;
        }
    }
}
