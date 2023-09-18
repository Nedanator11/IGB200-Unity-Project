using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CTGameManager : GameManager {

    private bool GameStarted;
    private bool GameOver;

    //Reference Variables
    public Camera MainCamera;
    public GameObject TileGrid;

    [Header("Difficulties")]
    public CCDifficulty Easy = new CCDifficulty();
    public CCDifficulty Medium = new CCDifficulty();
    public CCDifficulty Hard = new CCDifficulty();

    //Game variables
    private Tile ClickedTile;
    private bool TestingCircuit;
    private bool RoundOver;
    private float GameTimer;
    private float GameTimerDuration;
    private float Score;

    [Header("HUD References")]
    public GameObject GameStartHUD;
    public GameObject RoundHUD;
    public GameObject RoundEndGoodHUD;
    public GameObject RoundEndBadHUD;
    public GameObject GameOverHUD;

    // Awake Checks - Singleton setup
    private void Awake() {

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
    }

    private void Start()
    {
        GameStarted = false;
        GameOver = false;
    }

    private void Update()
    {
        //Don't process further if game is paused
        if (GameManager.instance.Paused)
            return;

        if (GameStarted && !RoundOver)
        {
            ElapseRoundTimer();

            if (Input.GetMouseButtonDown(0))
            {
                ClickTile();
            }
            if (Input.GetKeyDown("space"))
            {
                RoundOver = true;
                TestingCircuit = true;
            }
        }
        else if (TestingCircuit)
        {
            TestCircuit();
        }
        else if (GameStarted && RoundOver)
        {
            if (Input.GetKeyDown("space"))
            {
                if (!GameOver)
                    NextRound();
                else
                    RestartGame();
            }
        }
    }

    //Button event to set difficulty to Easy
    public void SetDifficultyEasy()
    {
        SetDifficulty(Easy);
        StartGame();
    }

    //Button event to set difficulty to Medium
    public void SetDifficultyMedium()
    {
        SetDifficulty(Medium);
        StartGame();
    }

    //Button event to set difficulty to Hard
    public void SetDifficultyHard()
    {
        SetDifficulty(Hard);
        StartGame();
    }

    //Apply difficulty settings of specified difficulty
    private void SetDifficulty(CCDifficulty difficulty)
    {
        TileGrid.GetComponent<TileGrid>().GridDimension = difficulty.GridDimension;
        TileGrid.GetComponent<TileGrid>().StartTilePosZ = difficulty.StartTilePosZ;
        TileGrid.GetComponent<TileGrid>().EndTilePosZ = difficulty.EndTilePosZ;
        GameTimerDuration = difficulty.GameTimerDuration;
        TileGrid.GetComponent<TileGrid>().Turn90Weight = difficulty.Turn90Weight;
        TileGrid.GetComponent<TileGrid>().StraightWeight = difficulty.StraightWeight;
        TileGrid.GetComponent<TileGrid>().TSplitWeight = difficulty.TSplitWeight;
        TileGrid.GetComponent<TileGrid>().CrossSplitWeight = difficulty.CrossSplitWeight;
    }
    
    //Starts the game
    private void StartGame()
    {
        GameStartHUD.SetActive(false);
        Score = 0;
        StartRound();

        GameStarted = true;

        //Start game timer
        StartRoundTimer(GameTimerDuration);
    }

    //Starts a new round
    private void StartRound()
    {
        //Start new round
        RoundOver = false;
        RoundHUD.SetActive(true);

        //Generate a new tile board
        TileGrid.GetComponent<TileGrid>().GenerateBoard();
    }

    //End current round with completed circuit
    private void EndRoundGood()
    {
        Score += 1;
        RoundHUD.GetComponent<RoundHUDController>().SetScoreText("Completed: " + Score);

        RoundHUD.SetActive(false);
        RoundEndGoodHUD.SetActive(true);
    }

    //End round with incorrect circuit
    private void EndRoundBad()
    {
        RoundHUD.SetActive(false);
        RoundEndBadHUD.SetActive(true);
    }

    //Clean up current round and start the next round
    private void NextRound()
    {
        //Hide round end HUD
        RoundEndGoodHUD.SetActive(false);
        RoundEndBadHUD.SetActive(false);

        //Destroy round objects
        TileGrid.GetComponent<TileGrid>().DestroyBoard();

        //Start a new round
        StartRound();
    }

    //Ends the current game
    private void EndGame()
    {
        //Hide round & round end HUDs
        RoundHUD.SetActive(false);
        RoundEndGoodHUD.SetActive(false);
        RoundEndBadHUD.SetActive(false);

        //Show game over HUD
        GameOverHUD.GetComponent<GameOverHUDController>().SetText(Score);
        GameOverHUD.SetActive(true);
    }

    //Restarts the game
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Start the current round timer
    private void StartRoundTimer(float duration)
    {
        GameTimer = duration;
        RoundHUD.GetComponent<RoundHUDController>().SetTimerText(GameTimer);
    }

    //Elapse timer for current update cycle
    private void ElapseRoundTimer()
    {
        GameTimer -= Time.deltaTime;
        if (GameTimer < 0f)
        {
            GameTimer = 0f;
            TestCircuit();
            EndGame();
            GameOver = true;
        }
        RoundHUD.GetComponent<RoundHUDController>().SetTimerText(GameTimer);
    }

    //Detects if player has clicked on a tile, and rotates it if true
    private void ClickTile()
    {
        //Raycast from camera to mouse position to detect click on object
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //Get the hit gameObject
            GameObject hitObject = hit.collider.gameObject;

            //If gameObject is a tile, rotate it clockwise
            if (hitObject.GetComponent<Tile>())
            {
                ClickedTile = hitObject.GetComponent<Tile>();
                ClickedTile.RotateTile();
            }
        }
    }

    //Tests the circuit to see if win condition is met
    private void TestCircuit()
    {
        if (!TileGrid.GetComponent<TileGrid>().FinishedAnimating())
            return;

        if (TileGrid.GetComponent<TileGrid>().DetectCompleteCircuit())
            EndRoundGood();
        else
            EndRoundBad();

        TestingCircuit = false;
    }
}

[System.Serializable]
public class CCDifficulty {

    public int GridDimension;
    public int StartTilePosZ;
    public int EndTilePosZ;
    public float GameTimerDuration;

    [Header("Tile Weights")]
    public float Turn90Weight;
    public float StraightWeight;
    public float TSplitWeight;
    public float CrossSplitWeight;
}
