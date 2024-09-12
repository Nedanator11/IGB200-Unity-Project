using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionsBlock : MonoBehaviour
{
    public Canvas canvas;

    public string helpText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void displayHelp()
    {
        canvas.GetComponent<DisplayText>().disaplyText(helpText);
    }
}
