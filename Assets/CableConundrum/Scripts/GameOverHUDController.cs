using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverHUDController : MonoBehaviour
{
    public GameObject Text;

    //Sets the text of the timer HUD object to given value, rounded up to whole seconds
    public void SetText(float value)
    {
        Text.GetComponent<TextMeshProUGUI>().text =
            "Time's Up!" + "\n\r" +
            "\n\r" +
            "You Completed " + value + " Circuits." + "\n\r" +
            "Well Done!";
    }
}
