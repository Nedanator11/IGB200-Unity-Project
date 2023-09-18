using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textScript : MonoBehaviour
{

    public GameObject playerController;
    private Camera cameraToLookAt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraToLookAt = playerController.GetComponentInParent<Camera>();
        transform.LookAt(cameraToLookAt.transform);
        transform.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);

    }
}

