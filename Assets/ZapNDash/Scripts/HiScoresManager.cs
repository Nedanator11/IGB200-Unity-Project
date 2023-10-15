using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HiScoresManager : MonoBehaviour
{
    //Object references
    public GameObject NewHiScoreForm;
    public GameObject HiScoresObject;
    public GameObject[] Rows;

    public string FileName;

    public string[] Headers;

    private string CompareField;
    public int[] NewHiScoreValues;

    private void Awake()
    {
        //Retrieve row objects on start
        Rows = HiScoresObject.transform.Cast<Transform>().Where(t => t.name.Contains("Row"))
            .Select(t => t.gameObject).ToArray();
    }

    //Loads the data for the Hi-Scores game object from it's respective file
    public void LoadHiScores()
    {
        //Ensure HiScore files exist
        CreateHiScores();

        //Read HiScores file
        using (StreamReader sr = new StreamReader(GetHiScoresFilePath(FileName)))
        {
            //Read the header row
            Headers = sr.ReadLine().Split(",");

            //Read following lines (up to 5)
            int currentRow = 1;
            while (!sr.EndOfStream && currentRow <= 5)
            {
                //Get the corresponding Row game object
                GameObject row = HiScoresObject.transform.Find("Row" + currentRow).gameObject;

                //Read fields from current line and write to row field game objects
                string[] fields = sr.ReadLine().Split(",");
                for (int i = 0; i < fields.Length; i++)
                {
                    row.transform.Find(Headers[i]).GetComponent<TextMeshProUGUI>().text = fields[i];
                }

                //Increment current row
                currentRow += 1;
            }
        }
    }

    //Outputs the data in the Hi-Scores game object to it's respective file
    public void SaveHiScores()
    {
        //Ensure HiScore files exist
        CreateHiScores();

        //Read HiScores file to retrieve headers
        using (StreamReader sr = new StreamReader(GetHiScoresFilePath(FileName)))
        {
            //Read the header row
            Headers = sr.ReadLine().Split(",");
        }

        //Overwrite existing HiScores file with data from hi-score game objects
        using (StreamWriter sw = new StreamWriter(GetHiScoresFilePath(FileName), false))
        {
            //Re-write the header row
            sw.WriteLine(string.Join(",", Headers));

            //Write data lines (up to 5)
            int currentRow = 1;
            while (currentRow <= 5)
            {
                //Get the corresponding Row game object
                GameObject row = HiScoresObject.transform.Find("Row" + currentRow).gameObject;

                //Write lines from field game objects of current row
                string[] fields = new string[Headers.Length];
                for (int i = 0; i < Headers.Length; i++)
                {
                    fields[i] = row.transform.Find(Headers[i]).GetComponent<TextMeshProUGUI>().text;
                }
                sw.WriteLine(string.Join(",", fields));

                //Increment current row
                currentRow += 1;
            }
        }
    }

    //Deletes the existing hi-scores and re-creates them
    public void ResetHiScores()
    {
        File.Delete(GetHiScoresFilePath(FileName));
        CreateHiScores();
    }

    //Constructs the file path to the specified hi-scores file name
    private string GetHiScoresFilePath(string fileName)
    {
        DirectoryInfo hiScoresDir = Directory.CreateDirectory(Path.GetDirectoryName(Application.dataPath) + @"\HiScores");
        return Path.Combine(hiScoresDir.FullName, fileName + ".txt");
    }

    //Creates and initialises the hi-scores data folder
    public void CreateHiScores()
    {
        //Get or create HiScores directory
        DirectoryInfo hiScoresDir = Directory.CreateDirectory(Path.GetDirectoryName(Application.dataPath) + @"\HiScores");

        //Get or create Cable Conundrum HiScores file
        string filePath = Path.Combine(hiScoresDir.FullName, "CableConundrumEasy" + ".txt");
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();

            //Initialise Cable Conundrum HiScores file
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("Name,Boards");
                for (int i = 0; i < 5; i++)
                {
                    sw.WriteLine("---,---");
                }
            }
        }

        //Get or create Cable Conundrum HiScores file
        filePath = Path.Combine(hiScoresDir.FullName, "CableConundrumMedium" + ".txt");
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();

            //Initialise Cable Conundrum HiScores file
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("Name,Boards");
                for (int i = 0; i < 5; i++)
                {
                    sw.WriteLine("---,---");
                }
            }
        }

        //Get or create Cable Conundrum HiScores file
        filePath = Path.Combine(hiScoresDir.FullName, "CableConundrumHard" + ".txt");
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();

            //Initialise Cable Conundrum HiScores file
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("Name,Boards");
                for (int i = 0; i < 5; i++)
                {
                    sw.WriteLine("---,---");
                }
            }
        }

        //Get or create Zap N' Dash HiScores file
        filePath = Path.Combine(hiScoresDir.FullName, "ZapNDash" + ".txt");
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();

            //Initialise Zap N' Dash HiScores file
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("Name,Rounds,Score");
                for (int i = 0; i < 5; i++)
                {
                    sw.WriteLine("---,---,---");
                }
            }
        }
    }

    //Compares specified score against current hi-scores
    public void CompareHiScore(string compareField, int[] values)
    {
        //Get the hi-score rows
        Rows = HiScoresObject.transform.Cast<Transform>().Where(t => t.name.Contains("Row"))
            .Select(t => t.gameObject).ToArray();

        //Get field values from Row game objects
        int[] fieldValues = Rows.Select(o => o.transform.Find(compareField).GetComponent<TextMeshProUGUI>().text).Where(s => s != "---")
            .Select(s => int.Parse(s)).ToArray();

        //If the Hi-Scores board is not full, or the specified value is greater than an existing hi-score
        if (fieldValues.Length < 5 || fieldValues.Any(v => v < values[Array.IndexOf(Headers, compareField) - 1]))
        {
            //Flag new hi-score
            CompareField = compareField;
            NewHiScoreValues = values;

            //Show NewHiScoreForm
            NewHiScoreForm.SetActive(true);
            transform.Find("RestartGameButton").GetComponent<Button>().interactable = false;
        }
        else //Not a new hi-score
        {
            NewHiScoreForm.SetActive(false);
            transform.Find("RestartGameButton").GetComponent<Button>().interactable = true;
        }
    }

    //Adds the new hi-score specified
    public void AddNewHiScore()
    {
        //Retrieve the compare value & the values of the current hi-scores
        int compareValue = NewHiScoreValues[Array.IndexOf(Headers, CompareField) - 1];
        int[] fieldValues = Rows.Select(o => o.transform.Find(CompareField).GetComponent<TextMeshProUGUI>().text).Where(s => s != "---")
            .Select(s => int.Parse(s)).ToArray();

        //Find the index where the new hi-score will be inserted
        int newHiScoreRow;
        if (fieldValues.Length == 0)
            newHiScoreRow = 0;
        else if (fieldValues.Length < 5 && !fieldValues.Any(v => v <= compareValue))
            newHiScoreRow = fieldValues.Length;
        else
            newHiScoreRow = Array.IndexOf(fieldValues, fieldValues.First(v => v <= compareValue));

        //Copy from rows above, starting from the last row until reaching the new hi-score row
        for (int i = 4; i > newHiScoreRow; i--)
            for (int h = 0; h < Headers.Length; h++)
                Rows[i].transform.Find(Headers[h]).GetComponent<TextMeshProUGUI>().text = Rows[i - 1].transform.Find(Headers[h]).GetComponent<TextMeshProUGUI>().text;

        //Write new hi-score to row
        Rows[newHiScoreRow].transform.Find("Name").GetComponent<TextMeshProUGUI>().text = NewHiScoreForm.transform.Find("InputField").GetComponent<TMP_InputField>().text;
        for (int i = 0; i < NewHiScoreValues.Length; i++)
        {
            Rows[newHiScoreRow].transform.Find(Headers[i + 1]).GetComponent<TextMeshProUGUI>().text = NewHiScoreValues[i].ToString();
        }

        //Re-enable RestartGameButton functionality
        transform.Find("RestartGameButton").GetComponent<Button>().interactable = true;
    }

    #region UI Events

    public void InputFieldOnEndEdit()
    {
        //If there is text in the input field, allow the player to confirm their entry
        if (NewHiScoreForm.transform.Find("InputField").GetComponent<TMP_InputField>().text.Length > 0)
            NewHiScoreForm.transform.Find("ConfirmNewHiScoreButton").GetComponent<Button>().interactable = true;
        else
            NewHiScoreForm.transform.Find("ConfirmNewHiScoreButton").GetComponent<Button>().interactable = false;
    }
    
    public void ConfirmHiScoreButtonClick()
    {
        NewHiScoreForm.SetActive(false);
        AddNewHiScore();
    }

    #endregion
}
