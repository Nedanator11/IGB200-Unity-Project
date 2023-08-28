using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESCMenuHandler : MonoBehaviour
{

    private bool active = false;

    public GameObject menu;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            active = !active;
            menu.SetActive(active);
        }
    }
}
