using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeConfirmButton : MonoBehaviour
{
    public GameObject mazeControllerObject;

    public Material notpresssedMat;
    public Material presssedMat;

    private bool pressed = false;

    float timerThresh = 0.5f;
    float numb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pressed)
        {
            numb = numb + (1 * Time.deltaTime);
            if(numb > timerThresh)
            {
                pressed = false;
                gameObject.GetComponent<MeshRenderer>().material = notpresssedMat;
                numb = 0;
            }
        }
    }

    public void callMazeController()
    {
        mazeControllerObject.GetComponent<MazeController>().checkButtonsActive();
    }

    public void changeMat()
    {
        gameObject.GetComponent<MeshRenderer>().material = presssedMat;
        pressed = true;
    }
}
