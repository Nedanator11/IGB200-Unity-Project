using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ZDRoundEndHUD : MonoBehaviour
{
    public GameObject DescriptionObject;

    public void SetDescription(string text)
    {
        DescriptionObject.GetComponent<TextMeshProUGUI>().text = text;
    }
}
