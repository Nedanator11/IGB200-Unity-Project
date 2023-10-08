using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    public GameObject Level;

    public void ButtonClick()
    {
        ((CCLGameManager)GameManager.instance).LoadLevel(Level.GetComponent<Level>());
    }
}
