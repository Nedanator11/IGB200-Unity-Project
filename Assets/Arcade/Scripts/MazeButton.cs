using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Rendering.HybridV2;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class MazeButton : MonoBehaviour
{

    //variables


    public float distanceThreshold = 10f;

    public Material uncheckedMaterial;
    public Material checkedMaterial;

    public bool isChecked;


    public float buttonNumb;

    private void Start()
    {
        isChecked = false;
    }

    // Update is called once per frame
    void Update()
    {


        materialControl();

    }

    private void materialControl()
    {
        if (isChecked == true)
        {
            gameObject.GetComponent<MeshRenderer>().material = checkedMaterial;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material = uncheckedMaterial;
        }
    }

    public void flipChecked()
    {
        isChecked = !isChecked;
    }
}
