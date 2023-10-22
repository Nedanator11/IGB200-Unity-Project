using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchCableGame : MonoBehaviour
{
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
            canvas.GetComponent<DisplayText>().disaplyText("Press 'e' to play\nCable Conundrum");
            if (Input.GetKeyDown("e"))
            {

                SceneManager.LoadScene(4);
            }

        }
        else
        {
            arcadeMachine.GetComponentInChildren<TextMeshPro>().enabled = false;
            
        }
    }
}
