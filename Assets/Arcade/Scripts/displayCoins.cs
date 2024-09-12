using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class displayCoins : MonoBehaviour
{
    public TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject arcadeController = GameObject.Find("arcade controller");
        int i = arcadeController.GetComponent<NewBehaviourScript>().arcadeCoins;
        text.text = "Coins: "+i ;
    }
}
