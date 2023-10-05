using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerInteraction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        interactRayCast();
    }

    void interactRayCast()
    {
        Vector3 playerPosition = transform.position;
        Vector3 forwardDirection = transform.forward;

        Ray interactionRay = new Ray(playerPosition, forwardDirection);
        RaycastHit interactionRayHit;
        float interactionRayLength = 5.0f;

        //this tests the ray cast
        Vector3 interactionRayEndpoint = forwardDirection * interactionRayLength;
        Debug.DrawLine(playerPosition, interactionRayEndpoint);

        bool hitFound = Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength);

        if (hitFound)
        {
            GameObject hitGameObject = interactionRayHit.transform.gameObject;

            //test if an object is hit
            /*
            string hitfeedback = hitGameObject.name;
            Debug.Log(hitfeedback);
            */

                if (Input.GetKeyDown("e"))
                {
                    if (hitGameObject.tag == "aMazeButton")
                    {
                        hitGameObject.GetComponent<MazeButton>().flipChecked();
                    }
                    else if(hitGameObject.tag == "mazeConfirmButton")
                    {
                    hitGameObject.GetComponent<MazeConfirmButton>().callMazeController();
                }
                    
                    
                }

        }
        else
        {
            /*
            string nothinHitFeedback = "-";
            Debug.Log(nothinHitFeedback);
            */
        }
    }
}
