using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    //variables
    public float throwForce = 600;
    private Vector3 objectPos;
    public float distance;

    public bool canHold = true;
    public GameObject item;
    public GameObject tempParent;
    public bool isHolding = false;

    public float holdDistance = 1.5f;

    // Update is called once per frame

    private void Start()
    {
        if(tempParent = null)
        {
           
            
        }
        tempParent = GameObject.Find("FPSController/FirstPersonCharacter/Guide");
    }
    void Update()
    {

        distance = Vector3.Distance(item.transform.position, tempParent.transform.position);
        if(distance >= holdDistance)
        {
            isHolding = false;
        }
        //Cheack if isHolding
        if(isHolding == true)
        {
            item.GetComponent<Rigidbody>().velocity = Vector3.zero;
            item.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            item.transform.rotation = tempParent.transform.rotation;
            item.transform.SetParent(tempParent.transform);
            if (Input.GetMouseButtonDown(1))
            {
                //throw
                item.GetComponent <Rigidbody>().AddForce(tempParent.transform.forward * throwForce);
                isHolding=false;
            }
        }
        else
        {
            objectPos = item.transform.position;
            item.transform.SetParent(null);
            item.GetComponent<Rigidbody>().useGravity = true;
            item.transform.position = objectPos;
        }
    }

    private void OnMouseDown()
    {
        if(distance <= holdDistance)
        {
            isHolding = true;
            item.GetComponent<Rigidbody>().useGravity = false;
            item.GetComponent<Rigidbody>().detectCollisions = true;
        }

    }

    private void OnMouseUp()
    {
        isHolding = false;
    }
}
