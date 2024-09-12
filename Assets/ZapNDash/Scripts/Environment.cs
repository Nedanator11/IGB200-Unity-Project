using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public GameObject[] GoodOptions;
    public GameObject[] BadOptions;
    public GameObject SelectedGoodOption;
    public List<GameObject> SelectedBadOptions;

    public GameObject SelectGoodOption()
    {
        SelectedGoodOption = GoodOptions[Random.Range(0, GoodOptions.Length)];
        return SelectedGoodOption;
    }

    public List<GameObject> SelectBadOptions(int count = 3)
    {
        SelectedBadOptions.Clear();
        List<GameObject> badOptionsList = BadOptions.ToList();
        for (int i = 0; i < count; i ++)
        {
            GameObject badOption = badOptionsList[Random.Range(0, badOptionsList.Count)];
            SelectedBadOptions.Add(badOption);
            badOptionsList.Remove(badOption);
        }

        return SelectedBadOptions;
    }
}
