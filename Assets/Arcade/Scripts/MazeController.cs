using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour
{
    // Start is called before the first frame update

    List<float> mazeButtonsActive = new List<float>();

    public GameObject[] mazeButtons;
    private int totalCheckedButtons;

    //variablers for spawning in hazards

    public GameObject[] hazardList;

    public GameObject[] hazardSpawnLoations;
    
    private List<float> hazardNumbAlive = new List<float>();

    private bool hazardsAlreadyExist = false;

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
            Debug.Log("Please make sure only 3 buttons are on");
        }
        else
        {
            if(testIfOptionsSelectedAreCorrect() == 3)
            {
                Debug.Log("Maze Correct");
            }
            else
            {
                Debug.Log("Please try again");
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
    }
}
