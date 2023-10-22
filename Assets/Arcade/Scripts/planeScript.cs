using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeScript : MonoBehaviour
{
    private bool colliding;

    private Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body=GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(colliding == false)
        {
            transform.Rotate(50 * Time.deltaTime,0,0);
            
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        colliding = true;
        
    }


    private void OnCollisionExit(Collision collision)
    {
        colliding = false;
        
    }
}
