using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class LaunchThunderGame : MonoBehaviour
{
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
                
                SceneManager.LoadScene("LightningSafetyGame");
            }

        }
        else
        {
            arcadeMachine.GetComponentInChildren<TextMeshPro>().enabled = false;
            
        }
    }
}
