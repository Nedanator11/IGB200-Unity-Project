using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayText : MonoBehaviour
{
    public TMP_Text text;
    public RawImage Image;

    private float counter;

    private bool textOnScreen;

    private float timer = 3;

    // Start is called before the first frame update
    void Start()
    {
        Image.enabled = false;
        text.enabled = false; 
    }

    // Update is called once per frame
    void Update()
    {
        counter = counter + 1 * Time.deltaTime;

        if (counter > timer)
        {
            disableTextOnScreen();
        }
    }

    private void disableTextOnScreen()
    {
        Image.enabled = false;
        text.enabled = false;
        textOnScreen = false;
    }

    public void disaplyText(string input, int onScreenTimer)
    {
        textOnScreen = true;
        counter = 0;
        Image.enabled = true;
        text.enabled = true;
        text.text = input;
        timer = onScreenTimer;

    }

    public void disaplyText(string input)
    {
        textOnScreen = true;
        counter = 0;
        Image.enabled = true;
        text.enabled = true;
        text.text = input;
        timer = 0.1f;
    }
}
