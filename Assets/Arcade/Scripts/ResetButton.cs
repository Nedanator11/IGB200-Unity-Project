using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public GameObject mazeControllerObject;

    public Material notpresssedMat;
    public Material presssedMat;

    private bool pressed = false;

    float timerThresh = 0.5f;
    float numb;

    public Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        mazeControllerObject.GetComponent<MazeController>().restMaze();
    }

    // Update is called once per frame
    void Update()
    {
        if (pressed)
        {
            canvas.GetComponent<DisplayText>().disaplyText("Maze Reset!\n Good Luck!", 2);
            numb = numb + (1 * Time.deltaTime);
            if (numb > timerThresh)
            {
                pressed = false;
                gameObject.GetComponent<MeshRenderer>().material = notpresssedMat;
                numb = 0;
            }
        }
    }

    public void callMazeController()
    {
        mazeControllerObject.GetComponent<MazeController>().restMaze();

    }

    public void changeMat()
    {
        gameObject.GetComponent<MeshRenderer>().material = presssedMat;
        pressed = true;
    }
}