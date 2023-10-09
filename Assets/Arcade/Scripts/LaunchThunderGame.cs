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

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(arcadeMachine.transform.position, playerController.transform.position);
        
        if (distance <= distanceThreshold)
        {
            arcadeMachine.GetComponentInChildren<TextMeshPro>().enabled = true;
            if(Input.GetKeyDown("e"))
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
