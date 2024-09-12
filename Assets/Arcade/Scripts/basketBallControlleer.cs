using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class basketBallControlleer : MonoBehaviour
{
    public int highScore = 000;
    public int curreentScore = 000;
    public float timerTotal = 60;
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    private float currentTimer;

    private bool isColliding = false;

    public float cooldownCheck = 5;
    private float cooldownTimer;

    private bool gameOn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(curreentScore > highScore)
        {
            highScore = curreentScore;
        }
        //HighScore
        highScoreText.text = "HighScore\n"+highScore;

        scoreText.text = "Score\n" + curreentScore;

        timerText.text = "Timer\n" + Mathf.Floor(currentTimer);

        if(gameOn)
        {
            currentTimer = currentTimer - Time.deltaTime;
        }
        else
        {
            currentTimer = timerTotal;
        }

        if(currentTimer < 0)
        {
            gameOn = false; 
            
        }

        if(gameOn == false)
        {
            cooldownTimer += Time.deltaTime;
        }

        if(gameOn == false && cooldownTimer < cooldownCheck)
        {
            timerText.text = "Wait\n" + (5f - Mathf.Floor(cooldownTimer));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if(gameOn == false && cooldownTimer > cooldownCheck)
        {
            gameOn = true;
            curreentScore = 0;
            cooldownTimer = 0;

        }

        if(gameOn == true)
        {
            curreentScore += 2;
        }
        





        if (other.tag == "Ball")
        {
            if (!isColliding)
            {
                isColliding = true;
                curreentScore += 2;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isColliding = false;
    }
}
