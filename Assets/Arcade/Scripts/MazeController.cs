using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MazeController : MonoBehaviour
{
    // Start is called before the first frame update

    List<float> mazeButtonsActive = new List<float>();

    public GameObject[] mazeButtons;
    private int totalCheckedButtons;

    public Canvas canvas;

    //variablers for spawning in hazards

    public GameObject[] hazardList;

    public GameObject[] hazardSpawnLoations;
    
    private List<float> hazardNumbAlive = new List<float>();

    private bool hazardsAlreadyExist = false;

    private bool mazeComplete;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void checkButtonsActive()
    {
        mazeButtonsActive.Clear();
        totalCheckedButtons = 0;
        //go through each button
        for(int i = 0; i < mazeButtons.Length; i++)
        {
            if (mazeButtons[i].GetComponent<MazeButton>().isChecked == true)
            {
                mazeButtonsActive.Add(mazeButtons[i].GetComponent<MazeButton>().buttonNumb);
                totalCheckedButtons++;
            }
        }

        //displayButtonsActive();
        if (totalCheckedButtons > 3 || totalCheckedButtons < 3)
        {
            canvas.GetComponent<DisplayText>().disaplyText("Make sure there are only\n 3 buttons highlighted", 4);
        }
        else
        {
            if(testIfOptionsSelectedAreCorrect() == 3)
            {
                if(mazeComplete == false)
                {
                    canvas.GetComponent<DisplayText>().disaplyText("Congradulations!\nMaze Complete!", 4);
                    mazeComplete = true;
                    GameObject arcadeController = GameObject.Find("arcade controller");
                    arcadeController.GetComponent<NewBehaviourScript>().arcadeCoins += 50;
                    
                }
                else
                {
                    canvas.GetComponent<DisplayText>().disaplyText("Reset the maze to have\n another go!", 4);
                }
                
            }
            else
            {
                canvas.GetComponent<DisplayText>().disaplyText("Try Again!", 2); ;
            }
        }

        
    }

    private void displayButtonsActive()
    {


        if (totalCheckedButtons == 0)
        {
            Debug.Log("No buttons Checked");
        }
        else 
        {
            for (int i = 0; i < mazeButtonsActive.Count; i++)
            {
                Debug.Log("Button: " + mazeButtonsActive[i] + " active!");
            }
        }
    }

    private void spawnHazards()
    {
        //checkls if hazards already exists then destorys them if they do while resting the lsit
        if (hazardsAlreadyExist == true)
        {
            hazardNumbAlive.Clear();

            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("mazeHazard");
            foreach(GameObject o in gameObjects)
            {
                Destroy(o);
            }
        }
        hazardsAlreadyExist = true;
        int randNumb;
        bool alreadyExists = false;

        for (int i = 0; i < 3; i++)
        {
            randNumb = UnityEngine.Random.Range(0, 6);
            for(int j = 0; j < hazardNumbAlive.Count; j++)
            {
                if (hazardNumbAlive[j] == randNumb)
                {
                    alreadyExists = true;
                    
                }
            }

            if(alreadyExists == false)
            {
                hazardNumbAlive.Add(randNumb);
                //spawn the hazard then move it to the spawn location then add 
                GameObject h = Instantiate(hazardList[randNumb]);
                h.transform.position = hazardSpawnLoations[i].transform.position;
                h.transform.rotation = hazardSpawnLoations[i].transform.rotation;
            }
            else
            {
                i--;
                alreadyExists = false;
            }

            
            
        }
    }

    public int testIfOptionsSelectedAreCorrect()
    {
        int totalCorrect = 0;

        foreach(float h in hazardNumbAlive)
        {
            foreach(float b in mazeButtonsActive)
            {
                if(h == b - 1)
                {
                    totalCorrect++;
                }
            }
        }

        return totalCorrect;
    }

    private void resetButtons()
    {
        foreach(GameObject o in mazeButtons)
        {
            if(o.GetComponent<MazeButton>().isChecked == true)
            {
                o.GetComponent<MazeButton>().flipChecked();
            }
        }
    }

    public void restMaze()
    {
        spawnHazards();
        resetButtons();
        mazeComplete = false;
        
    }
}
