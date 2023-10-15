using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CCLGameManager : GameManager
{
    // Levels
    public GameObject[] levels;

    //Reference Variables
    public Camera MainCamera;
    public GameObject TileGridObject;

    //Game variables
    private Tile ClickedTile;
    private bool LevelStarted;
    private bool TestingCircuit;
    private bool RoundOver;
    private float GameTimer;

    public int currentLevel;
    private bool levelPass;

    [Header("HUD References")]
    public GameObject LevelSelectHUD;
    public GameObject RoundHUD;
    public GameObject RoundEndGoodHUD;
    public GameObject RoundEndBadHUD;

    // Animator for round transitions
    public Animator ccAnimator;

    // Sound Handler
    SoundHandler soundHandler;

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
        LevelStarted = false;
    }

    private void Update()
    {
        //Don't process further if game is paused
        if (GameManager.instance.Paused)
            return;

        if (LevelStarted && !RoundOver)
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
        else if (LevelStarted && RoundOver)
        {
            // Trigger round/level transitions. Functions called as animation events.
            if (Input.GetKeyDown("space"))  
            {
                if (levelPass)
                {
                    ccAnimator.SetTrigger("NextLevel");

                    if (currentLevel == 6)
                    {
                        ccAnimator.SetTrigger("LevelSelect");
                    }
                }
                else if (!levelPass)
                {
                    ccAnimator.SetTrigger("Retry");
                }
            }
        }
    }
    
    //Starts the game
    public void LoadLevel(Level level)
    {
        // Current index = currentLevel - 1
        currentLevel = level.levelInt;

        RoundEndGoodHUD.SetActive(false);
        RoundEndBadHUD.SetActive(false);

        LevelSelectHUD.SetActive(false);
        RoundHUD.SetActive(true);

        TestingCircuit = false;
        RoundOver = false;
        LevelStarted = true;

        TileGridObject.GetComponent<TileGrid>().LoadLevel(level);

        //Start game timer
        StartRoundTimer(level.TimeLimit);
    }

    //End current round with completed circuit
    private void EndRoundGood()
    {
        levelPass = true;
        RoundHUD.SetActive(false);
        RoundEndGoodHUD.SetActive(true);
    }

    //End round with incorrect circuit
    private void EndRoundBad()
    {
        levelPass = false;
        RoundHUD.SetActive(false);
        RoundEndBadHUD.SetActive(true);
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
    }

    //Elapse timer for current update cycle
    private void ElapseRoundTimer()
    {
        GameTimer -= Time.deltaTime;
        if (GameTimer < 0f)
        {
            GameTimer = 0f;
            TestCircuit();
            RoundOver = true;
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
        if (!TileGridObject.GetComponent<TileGrid>().FinishedAnimating())
            return;

        if (TileGridObject.GetComponent<TileGrid>().DetectCompleteCircuit())
            EndRoundGood();
        else
            EndRoundBad();

        TestingCircuit = false;
    }

    // Load the next level
    public void LoadNextLevel()
    {
        TileGridObject.GetComponent<TileGrid>().DestroyBoard();
        LoadLevel(levels[currentLevel].GetComponent<Level>());
    }

    // Reload the current level
    public void RetryLevel()
    {
        TileGridObject.GetComponent<TileGrid>().DestroyBoard();
        LoadLevel(levels[currentLevel - 1].GetComponent<Level>());
    }
}
