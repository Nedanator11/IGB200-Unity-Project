using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CTGameManager : GameManager {

    public enum GameStates
    {
        DifficultyMenu,
        Gameplay,
        TestingCircuit,
        HighlightingTiles,
        RoundEndGood,
        RoundEndBad,
        GameOver,
        HiScores
    }

    private GameStates GameState;

    //Reference Variables
    public Camera MainCamera;
    public GameObject TileGridObject;

    [Header("Difficulties")]
    public CCDifficulty Easy = new CCDifficulty();
    public CCDifficulty Medium = new CCDifficulty();
    public CCDifficulty Hard = new CCDifficulty();

    //Game variables
    private Tile ClickedTile;
    private float GameTimer;
    private float GameTimerDuration;
    private float Score;
    private RoundFailController.FailType Failure;

    [Header("HUD References")]
    public GameObject GameStartHUD;
    public GameObject RoundHUD;
    public GameObject RoundEndGoodHUD;
    public GameObject RoundEndBadHUD;
    public GameObject GameOverHUD;
    public GameObject HiScoresHUD;

    // Animator for round transitions
    public Animator ccAnimator;

    // Animator for RoundEnd feedback
    public Animator feedbackAnimator;

    // Sound Handler
    SoundHandler soundHandler;
    public GameObject audioManager;

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
        GameState = GameStates.DifficultyMenu;

        soundHandler = audioManager.GetComponent<SoundHandler>();
    }

    private void Update()
    {
        //Don't process if game is paused
        if (GameManager.instance.Paused)
            return;

        switch (GameState)
        {
            //Main Menu Disabled - Gameplay starts immediately instead
            case GameStates.DifficultyMenu:
                GameState_DifficultyMenu();
                break;

            case GameStates.Gameplay:
                GameState_Gameplay();
                break;

            case GameStates.TestingCircuit:
                GameState_TestingCircuit();
                break;

            case GameStates.HighlightingTiles:
                GameState_HighlightingTiles();
                break;

            case GameStates.RoundEndGood:
                GameState_RoundEndGood();
                break;

            case GameStates.RoundEndBad:
                GameState_RoundEndBad();
                break;

            case GameStates.GameOver:
                GameState_GameOver();
                break;

            case GameStates.HiScores:
                GameState_HiScores();
                break;
        }
    }

    #region GameState Methods

    private void GameState_DifficultyMenu()
    {
        //Show GameStartHUD
        if (!GameStartHUD.activeSelf) GameStartHUD.SetActive(true);
    }

    private void GameState_Gameplay()
    {
        // Reset Hazard Feedback to idle state
        feedbackAnimator.SetBool("RoundEndIdle", true);

        //Show RoundHUD
        if (!RoundHUD.activeSelf) RoundHUD.SetActive(true);

        ElapseRoundTimer();

        //Check for mouse button down event
        if (Input.GetMouseButtonDown(0))
            ClickTile();

        //Check for space bar down event
        if (Input.GetKeyDown("space"))
            TestCircuitTrigger();
    }

    private void GameState_TestingCircuit()
    {
        TestCircuit();
    }

    private void GameState_HighlightingTiles()
    {
        //Wait for tiles to finish highlighting
        if (!TileGridObject.GetComponent<TileGrid>().FinishedAnimating())
            return;

        //Determine round end
        if (Failure == RoundFailController.FailType.None)
        {
            EndRoundGood();
        }
        else
        {
            RoundEndBadHUD.GetComponent<RoundFailController>().ShowFailureDescription(Failure);
            EndRoundBad();
        }
    }

    private void GameState_RoundEndGood()
    {
        //Show RoundEndGoodHUD
        // if (!RoundEndGoodHUD.activeSelf) RoundEndGoodHUD.SetActive(true);
        feedbackAnimator.SetBool("RoundEndIdle", false);
        feedbackAnimator.SetTrigger("RoundEndGood");

        if (Input.GetKeyDown("space"))
            NextRoundTrigger();
    }

    private void GameState_RoundEndBad()
    {
        //Show RoundEndBadHUD
        // if (!RoundEndBadHUD.activeSelf) RoundEndBadHUD.SetActive(true);
        feedbackAnimator.SetBool("RoundEndIdle", false);
        feedbackAnimator.SetTrigger("RoundEndBad");

        if (Input.GetKeyDown("space"))
            NextRoundTrigger();
    }

    private void GameState_GameOver()
    {
        //Show GameOverHUD
        if (!GameOverHUD.activeSelf) GameOverHUD.SetActive(true);
    }

    private void GameState_HiScores()
    {
        //Show HiScoresHUD & load data on first showing
        if (!HiScoresHUD.activeSelf)
        {
            HiScoresHUD.GetComponent<HiScoresManager>().LoadHiScores();
            HiScoresHUD.SetActive(true);
            soundHandler.HiScores();
        }
    }

    #endregion

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
        TileGridObject.GetComponent<TileGrid>().GridDimension = difficulty.GridDimension;
        TileGridObject.GetComponent<TileGrid>().StartTileSettings.GridPosition = new Vector2(0, difficulty.StartTilePosZ);
        TileGridObject.GetComponent<TileGrid>().EndTileSettings.GridPosition = new Vector2(difficulty.GridDimension + 1, difficulty.EndTilePosZ);
        GameTimerDuration = difficulty.GameTimerDuration;
        TileGridObject.GetComponent<TileGrid>().Turn90Weight = difficulty.Turn90Weight;
        TileGridObject.GetComponent<TileGrid>().StraightWeight = difficulty.StraightWeight;
        TileGridObject.GetComponent<TileGrid>().TSplitWeight = difficulty.TSplitWeight;
        TileGridObject.GetComponent<TileGrid>().CrossSplitWeight = difficulty.CrossSplitWeight;
        TileGridObject.GetComponent<TileGrid>().HazardTilePercent = difficulty.HazardTilePercent;

        HiScoresHUD.GetComponent<HiScoresManager>().FileName = "CableConundrum" + difficulty.Name;
    }
    
    //Starts the game
    private void StartGame()
    {
        //Play Music
        soundHandler.CCMusic();

        GameStartHUD.SetActive(false);
        Score = 0;
        StartRound();

        //Start game timer
        StartRoundTimer(GameTimerDuration);
    }

    //Starts a new round
    private void StartRound()
    {
        //Sets the game state to 'Gameplay'
        GameState = GameStates.Gameplay;

        //Generate a new tile board
        TileGridObject.GetComponent<TileGrid>().GenerateBoard();
    }

    //End current round with completed circuit
    private void EndRoundGood()
    {
        soundHandler.PlaySFX(soundHandler.circuitCorrect);

        Score += 1;
        RoundHUD.GetComponent<RoundHUDController>().SetScoreText("Completed: " + Score);

        RoundHUD.SetActive(false);
        GameState = GameStates.RoundEndGood;
    }

    //End round with incorrect circuit
    private void EndRoundBad()
    {
        soundHandler.PlaySFX(soundHandler.circuitIncorrect);
        RoundHUD.SetActive(false);
        GameState = GameStates.RoundEndBad;
    }

    //Clean up current round and start the next round
    public void NextRound()
    {
        //Hide round end HUD
        RoundEndGoodHUD.SetActive(false);
        RoundEndBadHUD.SetActive(false);
        RoundEndBadHUD.GetComponent<RoundFailController>().ResetFailureDescription();

        //Destroy round objects
        TileGridObject.GetComponent<TileGrid>().DestroyBoard();

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
        RoundEndBadHUD.GetComponent<RoundFailController>().ResetFailureDescription();

        //Show game over HUD
        GameOverHUD.GetComponent<GameOverHUDController>().SetText(Score);
        GameState = GameStates.GameOver;
    }

    //Checks the player's final score against the current hi-scores
    private void CheckHiScores()
    {
        HiScoresHUD.GetComponent<HiScoresManager>().CompareHiScore("Boards", new int[] { (int)Score });
    }

    //Restarts the game
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Start the current round timer
    private void StartRoundTimer(float duration)
    {
        GameTimer = duration;
        RoundHUD.GetComponent<RoundHUDController>().SetTimerText(GameTimer);
        RoundHUD.GetComponent<RoundHUDController>().SetTimerBarMax(GameTimer);
    }
    
    //Elapse timer for current update cycle
    private void ElapseRoundTimer()
    {
        GameTimer -= Time.deltaTime;
        if (GameTimer < 0f)
        {
            GameTimer = 0f;
            EndGame();
        }
        RoundHUD.GetComponent<RoundHUDController>().SetTimerText(GameTimer);
        RoundHUD.GetComponent<RoundHUDController>().SetTimerBar(GameTimer);
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
                if (hitObject.CompareTag("HazardCable"))
                {
                    //TODO: Show 'clicked on hazardous cable' player feedback message
                }
                else //Basic Tile
                {
                    ClickedTile = hitObject.GetComponent<Tile>();
                    ClickedTile.RotateTile();
                }
            }
        }
    }

    //Tests the circuit to see if win condition is met
    private void TestCircuit()
    {
        if (!TileGridObject.GetComponent<TileGrid>().FinishedAnimating())
            return;

        //Detect any failures in the circuit
        Failure = TileGridObject.GetComponent<TileGrid>().DetectCircuitFailure();

        //Set GameState to HighlightTiles
        GameState = GameStates.HighlightingTiles;
    }

    #region Button Events

    public void TestCircuitTrigger()
    {
        GameState = GameStates.TestingCircuit;
    }

    public void NextRoundTrigger()
    {
        // NextRound calls from animation event
        soundHandler.PlaySFX(soundHandler.doorClose);
        ccAnimator.SetTrigger("NextRound");
    }

    public void EndGameButtonClick()
    {
        GameOverHUD.SetActive(false);
        GameState = GameStates.HiScores;
        HiScoresHUD.GetComponent<HiScoresManager>().LoadHiScores();
        CheckHiScores();
    }

    public void RestartGameTrigger()
    {
        HiScoresHUD.GetComponent<HiScoresManager>().SaveHiScores();
        ccAnimator.SetTrigger("Return");
    }

    #endregion
}

[System.Serializable]
public class CCDifficulty {

    public string Name;

    public int GridDimension;
    public int StartTilePosZ;
    public int EndTilePosZ;
    public float GameTimerDuration;

    [Header("Tile Weights")]
    public float Turn90Weight;
    public float StraightWeight;
    public float TSplitWeight;
    public float CrossSplitWeight;
    public float HazardTilePercent;
}
