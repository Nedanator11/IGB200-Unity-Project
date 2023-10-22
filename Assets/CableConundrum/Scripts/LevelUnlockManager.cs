using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUnlockManager : MonoBehaviour
{
    public Button[] levelButtons;

    public void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = false;
        }

        for (int i = 0; i < unlockedLevel; i++)
        {
            levelButtons[i].interactable = true;
        }
    }

    public void ResetUnlockedLevels()
    {
        PlayerPrefs.SetInt("ReachedLevel", 1);
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();
    }

    public void UnlockAllLevels()
    {
        PlayerPrefs.SetInt("ReachedLevel", 12);
        PlayerPrefs.SetInt("UnlockedLevel", 12);
        PlayerPrefs.Save();
    }
}
