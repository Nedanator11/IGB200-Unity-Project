using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchThunderGame : MonoBehaviour
{
    public GameObject SceneTransitions;

    //variables
    public GameObject playerController;
    public GameObject arcadeMachine;
    public float distance;

    public float distanceThreshold = 10f;

    public Canvas canvas;

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(arcadeMachine.transform.position, playerController.transform.position);
        
        if (distance <= distanceThreshold)
        {
            canvas.GetComponent<DisplayText>().disaplyText("Press 'e' to play\nZap 'N' Dash!");
            if (Input.GetKeyDown("e"))
            {

                SceneManager.LoadScene(5);
            }

        }
        else
        {
            arcadeMachine.GetComponentInChildren<TextMeshPro>().enabled = false;
            
        }
    }
}
