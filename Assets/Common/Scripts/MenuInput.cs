using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInput : MonoBehaviour
{

    public Animator animator;
    SceneHandler sceneHandler;

    // Start is called before the first frame update
    void Start()
    {
        sceneHandler = GetComponent<SceneHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey) {
            sceneHandler.FadeToLevel(3);
            animator.SetTrigger("KeyPress");
        }
    }
}
