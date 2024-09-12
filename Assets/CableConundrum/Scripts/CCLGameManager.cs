using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CCLGameManager : GameManager
{
    public enum GameStates
    {
        LevelSelect,
        Gameplay,
        TestingCircuit,
        HighlightingTiles,
        RoundEndGood,
        RoundEndBad
    }

    private GameStates GameState;

    // Levels
    public GameObject[] levels;

    //Reference Variables
    public Camera MainCamera;
    public GameObject TileGridObject;

    //Game variables
    private Tile ClickedTile;
    private float GameTimer;
    private RoundFailController.FailType Failure;

    public int currentLevel;

    [Header("HUD References")]
    public GameObject LevelSelectHUD;
    public GameObject RoundHUD;
    public GameObject RoundEndGoodHUD;
    public GameObject RoundEndBadHUD;

    // Animator for round transitions
    public Animator ccAnimator;

    // Animator for hazard feedback
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
        GameState = GameStates.LevelSelect;

        soundHandler = audioManager.GetComponent<SoundHandler>();
    }

    private void Update()
    {
        //Don't process further if game is paused
        if (GameManager.instance.Paused)
            return;

        switch (GameState)
        {
            case GameStates.LevelSelect:
                GameState_LevelSelect();
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
        }
    }

    #region GameState Methods

    private void GameState_LevelSelect()
    {
        //Show GameStartHUD
        if (!LevelSelectHUD.activeSelf) LevelSelectHUD.SetActive(true);
    }

    private void GameState_Gameplay()
    {
        //Show RoundHUD
        if (!RoundHUD.activeSelf) RoundHUD.SetActive(true);

        ElapseRoundTimer();

        //Check for mouse button down event
        if (Input.GetMouseButtonDown(0))
            ClickTile();

        //Check for space bar down event
        if (Input.GetKeyDown("space"))
            GameState = GameStates.TestingCircuit;
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
        if (!RoundEndGoodHUD.activeSelf) RoundEndGoodHUD.SetActive(true);

        // Trigger round/level transitions. Functions called as animation events.
        if (Input.GetKeyDown("space"))
        {
            if (currentLevel != 12)
            {
                ccAnimator.SetTrigger("NextLevel");
                soundHandler.PlaySFX(soundHandler.doorClose);

                if (currentLevel == levels.Length)
                    ccAnimator.SetTrigger("Return");
            }
            else ccAnimator.SetTrigger("Return");
        }
    }

    private void GameState_RoundEndBad()
    {
        //Show RoundEndBadHUD
        if (!RoundEndBadHUD.activeSelf) RoundEndBadHUD.SetActive(true);

        // Trigger round/level transitions. Functions called as animation events.
        if (Input.GetKeyDown("space"))
        {
            soundHandler.PlaySFX(soundHandler.doorClose);
            ccAnimator.SetTrigger("Retry");
        }
    }

    #endregion

    //Starts the game
    public void LoadLevel(Level level)
    {
        //Play Music
        soundHandler.CCLMusic();

        // Reset hazard feedback to default idle state
        feedbackAnimator.SetBool("RoundEndIdle", true);

        // Current index = currentLevel - 1
        currentLevel = level.levelInt;

        //Hide level select HUD
        LevelSelectHUD.SetActive(false);

        //Change game state & load level
        GameState = GameStates.Gameplay;
        TileGridObject.GetComponent<TileGrid>().LoadLevel(level);

        //Start game timer
        StartRoundTimer(level.TimeLimit);

        // Set level title HUD
        RoundHUD.GetComponent<RoundHUDController>().SetLevelNumber(currentLevel);
    }

    //End current round with completed circuit
    private void EndRoundGood()
    {
        // Play SFX and trigger feedback animations.
        soundHandler.PlaySFX(soundHandler.circuitCorrect);
        feedbackAnimator.SetBool("RoundEndIdle", false);
        feedbackAnimator.SetTrigger("RoundEndGood");
        
        GameState = GameStates.RoundEndGood;
    }

    //End round with incorrect circuit
    private void EndRoundBad()
    {
        // Play SFX and trigger feedback animations.
        soundHandler.PlaySFX(soundHandler.circuitIncorrect);
        feedbackAnimator.SetBool("RoundEndIdle", false);
        feedbackAnimator.SetTrigger("RoundEndBad");

        GameState = GameStates.RoundEndBad;
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

            //End the level
            RoundEndBadHUD.GetComponent<RoundFailController>().ShowFailureDescription(RoundFailController.FailType.TimerElapsed);
            EndRoundBad();
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

    // Load the next level
    public void LoadNextLevel()
    {
        //Hide round end HUD
        RoundEndGoodHUD.SetActive(false);
        RoundEndBadHUD.SetActive(false);
        RoundEndBadHUD.GetComponent<RoundFailController>().ResetFailureDescription();

        TileGridObject.GetComponent<TileGrid>().DestroyBoard();

        LoadLevel(levels[currentLevel].GetComponent<Level>());
        UnlockLevel();
    }

    // Reload the current level
    public void RetryLevel()
    {
        //Hide round end HUD
        RoundEndGoodHUD.SetActive(false);
        RoundEndBadHUD.SetActive(false);
        RoundEndBadHUD.GetComponent<RoundFailController>().ResetFailureDescription();

        TileGridObject.GetComponent<TileGrid>().DestroyBoard();
        LoadLevel(levels[currentLevel - 1].GetComponent<Level>());
    }

    // Enable next level button
    public void UnlockLevel()
    {
        if (currentLevel - 1 >= PlayerPrefs.GetInt("ReachedLevel"))
        {
            PlayerPrefs.SetInt("ReachedLevel", currentLevel);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }

        else if (currentLevel >= 11)
        {
            return;
        }
    }
}
