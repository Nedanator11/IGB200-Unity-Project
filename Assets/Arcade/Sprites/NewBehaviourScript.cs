using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public int arcadeCoins = 0;
    public bool tutorialComplete = false;
    // Start is called before the first frame update

    private void Awake()
    {
        
            GameObject[] objs = GameObject.FindGameObjectsWithTag("arcade");

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(this.gameObject);
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
