using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject Destination;
    public float MoveSpeed;
    public bool FinishedAnimating;

    private void Update()
    {
        if (Destination != null)
            AnimatePlayer();
    }

    //Process player animation for this frame
    private void AnimatePlayer()
    {
        //Calculate vectors
        Vector3 displacement = Destination.transform.position - transform.position;
        Vector3 direction = displacement.normalized;
        Vector3 movementStep = direction * MoveSpeed * Time.deltaTime;

        //Check is this step will bring the player to the destination
        if (movementStep.magnitude >= displacement.magnitude)
        {
            //Move to the destination exactly
            transform.position = Destination.transform.position;
            Destination = null;
            FinishedAnimating = true;

            //Trigger cloud animation
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<LSGameManager>().TriggerCloudAnimation();
        }
        else
        {
            //Step towards the destination
            transform.position += movementStep;
        }
    }
}
