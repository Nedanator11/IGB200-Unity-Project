using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public List<GameObject> Destinations;
    private GameObject Destination;
    public float MoveSpeed;
    public bool FinishedAnimating;
    public GameObject LightningStrikePrefab;
    public GameObject LightningMarker;
    public float LightningStrikeDuration;

    public GameObject StrikeFaceAnimation;
    private float StrikeFaceAnimationDuration = 0.25f;
    private float StrikeFaceAnimationTimer = -1f;

    SoundHandler soundHandler;
    Player playerScript;

    private void Start()
    {
        soundHandler = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<SoundHandler>();
        playerScript = GameObject.FindGameObjectWithTag("LSPlayer").GetComponent<Player>();
    }

    private void Update()
    {
        if (Destinations != null)
            AnimateCloud();
    }

    //Process cloud animation for this frame
    private void AnimateCloud()
    {
        //If there is no current destination
        if (Destination == null)
        {
            //Get the next destination
            if (Destinations.Count > 0)
            {
                Destination = Destinations.First();
                Destinations.Remove(Destination);
            }
            else //Finish animating
            {
                Destinations = null;
                FinishedAnimating = true;
                return;
            }
        }

        //Calculate vectors
        Vector3 displacement = Destination.transform.position - transform.position;
        Vector3 direction = displacement.normalized;
        Vector3 movementStep = direction * MoveSpeed * Time.deltaTime;

        //Check is this step will bring the player to the destination
        if (movementStep.magnitude >= displacement.magnitude)
        {
            //Move to the destination exactly
            transform.position = Destination.transform.position;

            if (StrikeFaceAnimationTimer <= 0)
            {
                //Begin animation
                StrikeFaceAnimation.SetActive(true);

                //Start animation timer
                StrikeFaceAnimationTimer = StrikeFaceAnimationDuration;
            }
            else //StrikeFaceAnimationTimer > 0
            {
                //Decrement timer
                StrikeFaceAnimationTimer -= Time.deltaTime;

                //If timer elapses
                if (StrikeFaceAnimationTimer <= 0)
                {
                    //If the destination is a bad option, strike it with lightning
                    foreach (Transform child in Destination.transform.parent)
                    {
                        if (child.CompareTag("BadOption"))
                        {
                            StrikeLightning();
                            break;
                        }
                    }

                    //Disable animation
                    StrikeFaceAnimation.SetActive(false);

                    //Unset the destination
                    Destination = null;
                }
            }
        }
        else
        {
            //Step towards the destination
            transform.position += movementStep;
        }
    }

    //Instantiate lightning strike and initiate destroy delay
    private void StrikeLightning()
    {
        GameObject strike = Instantiate(LightningStrikePrefab, LightningMarker.transform.position, LightningMarker.transform.rotation);
        Destroy(strike, LightningStrikeDuration);

        soundHandler.PlaySFX(soundHandler.lightningStrike);
    }
}
