using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundHUDController : MonoBehaviour
{
    public float TimerBarMax = 10f;
    public Image TimerBarObject;
    public GameObject TimerHUDObject;
    public GameObject ScoreHUDObject;
    public GameObject LevelNumberHUDObject;

    //Sets the text of the timer HUD object to given value, rounded up to whole seconds
    public void SetTimerText(float value)
    {
        TimerHUDObject.GetComponent<TextMeshProUGUI>().text = Mathf.CeilToInt(value).ToString();
    }

    //Sets the text of the score HUD object to given value, rounded up to next whole number
    public void SetScoreText(string text)
    {
        ScoreHUDObject.GetComponent<TextMeshProUGUI>().text = text;
    }

    //Initialised the maximum value of the timer bar
    public void SetTimerBarMax(float value)
    {
        TimerBarMax = value;
    }

    //Sets the fill of the timer bar to the ratio of time/TimerBarMax
    public void SetTimerBar(float time)
    {
        if (TimerBarObject != null)
        {
            TimerBarObject.fillAmount = time / TimerBarMax;
        }
    }

    public void SetLevelNumber(int level)
    {
        LevelNumberHUDObject.GetComponent<TextMeshProUGUI>().text = "Level " + level.ToString();
    }
}
