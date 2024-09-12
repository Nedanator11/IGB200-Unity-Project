using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class playerInteraction : MonoBehaviour
{
    public Canvas canvas;
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
            
            string hitfeedback = hitGameObject.name;
            Debug.Log(hitfeedback);
            

            if(hitGameObject.tag == "helpBlock")
            {
                hitGameObject.GetComponent<QuestionsBlock>().displayHelp();
            }
            if (hitGameObject.tag == "toy" && hitGameObject.GetComponent<itemsCostMoney>().enabled == true)
            {
                int i = hitGameObject.GetComponent<itemsCostMoney>().cost;
                string j = hitGameObject.GetComponent<itemsCostMoney>().itemDes;
                    GameObject arcadeController = GameObject.Find("arcade controller");
                    int k = arcadeController.GetComponent<NewBehaviourScript>().arcadeCoins;     
                
                if(k < i)
                {
                    canvas.GetComponent<DisplayText>().disaplyText(j + "\nCost: <color=#FF0000>" + i + " </color>\nPress 'e' to purcahse");
                }
                else
                {
                    canvas.GetComponent<DisplayText>().disaplyText(j + "\nCost: <color=#0BFF00>" + i + " </color>\nPress 'e' to purcahse");
                }
                if (Input.GetKeyDown("e"))
                {

                    if (k >= i)
                    {
                        hitGameObject.GetComponent<PickUpObject>().enabled = true;
                        arcadeController.GetComponent<NewBehaviourScript>().arcadeCoins -= i;
                        hitGameObject.GetComponent<itemsCostMoney>().enabled = false;
                    }
                }
            }


            if (Input.GetKeyDown("e"))
                {
                    if (hitGameObject.tag == "aMazeButton")
                    {
                        hitGameObject.GetComponent<MazeButton>().flipChecked();
                    }
                    else if(hitGameObject.tag == "mazeConfirmButton")
                    {
                        hitGameObject.GetComponent<MazeConfirmButton>().callMazeController();
                        hitGameObject.GetComponent<MazeConfirmButton>().changeMat();
                    }
                    else if (hitGameObject.tag == "helpBlock")
                    {
                        hitGameObject.GetComponent<QuestionsBlock>().displayHelp();
                        Debug.Log("test");
                    }
                else if (hitGameObject.tag == "Maze reset button")
                {
                    hitGameObject.GetComponent<ResetButton>().callMazeController();
                    hitGameObject.GetComponent<ResetButton>().changeMat();
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
