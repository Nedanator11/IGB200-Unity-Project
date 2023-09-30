using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LSGameManager : GameManager {

    [Header("HUD References")]
    public GameObject GameStartHUD;
    public GameObject RoundHUD;
    public GameObject RoundEndGoodHUD;
    public GameObject RoundEndBadHUD;

    [Header("Place Markers")]
    public GameObject EnvironmentMarker;
    public GameObject[] OptionMarkers;
    public GameObject PlayerInitialMarker;
    public GameObject CloudInitialMarker;

    [Header("Prefab References")]
    public GameObject[] EnvironmentPrefabs;
    public GameObject OptionGoodPrefab;
    public GameObject OptionBadPrefab;
    public GameObject PlayerPrefab;
    public GameObject CloudPrefab;

    [Header("Object References")]
    public GameObject EnvironmentScape;
    public GameObject[] Options;
    public GameObject Player;
    public GameObject Cloud;
    public GameObject SelectedOption;

    private bool GameStarted;
    private bool GameOver;

    [Header("Other")]
    //Score Variables
    public float ScorePerRound;
    private float Score;

    //Cycle variables
    public float InitialTimerDuration;
    public float TimerDurationDecrease;
    private int Cycle = 0;
    public int CycleLength;
    private int RoundCount = 0;

    //Round variables
    private bool RoundOver;
    private float RoundTimer;
    private float RoundTimerDuration;

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
        //Don't process if game is paused
        if (GameManager.instance.Paused)
            return;

        //Game not started condition
        if (!GameStarted)
        {
            if (Input.GetKeyDown("space"))
            {
                StartGame();
                return;
            }
        }

        //Round is running condition
        if (GameStarted && !RoundOver)
        {
            //If player has not yet selected an option
            if (SelectedOption == null)
            {
                //Detect player input
                if (Input.GetMouseButtonDown(0))
                    ClickObject();

                //Elapse round timer
                ElapseRoundTimer();

                return;
            }

            //If animation has finished
            if (Player.GetComponent<Player>().FinishedAnimating && Cloud.GetComponent<Cloud>().FinishedAnimating)
            {
                RoundOver = true;
                CheckWinCondition();
                return;
            }
        }

        //Game over condition
        if (GameOver)
        {
            if (Input.GetKeyDown("space"))
            {
                RestartGame();
                return;
            }
        }

        //Round over condition
        if (RoundOver)
        {
            if (Input.GetKeyDown("space"))
            {
                NextRound();
                return;
            }
        }
    }

    //Starts the game
    private void StartGame()
    {
        GameStartHUD.SetActive(false);
        GameStarted = true;
        Score = 0;
        StartRound();
    }

    //Starts a new round
    private void StartRound()
    {
        //Increment RoundCount
        RoundCount++;

        //Cycle Check
        Cycle = (RoundCount + CycleLength - 1) / CycleLength;
        RoundTimerDuration = Mathf.Max(InitialTimerDuration - (TimerDurationDecrease * (Cycle - 1)), 1);

        //Start new round
        RoundOver = false;
        SelectedOption = null;
        RoundHUD.SetActive(true);

        //Randomly select evironment
        EnvironmentScape = Instantiate(EnvironmentPrefabs[Random.Range(0, EnvironmentPrefabs.Length)], EnvironmentMarker.transform.position, EnvironmentMarker.transform.rotation);

        //Randomly populate options
        List<GameObject> options = EnvironmentScape.GetComponent<Environment>().SelectBadOptions().ToList();
        options.Insert(Random.Range(0, 4), EnvironmentScape.GetComponent<Environment>().SelectGoodOption());
        Options = options.ToArray();
        for (int i = 0; i < Options.Length; i++)
            Options[i] = Instantiate(Options[i], OptionMarkers[i].transform.position, OptionMarkers[i].transform.rotation, OptionMarkers[i].transform);

        //Initialise player & cloud objects
        Player = Instantiate(PlayerPrefab, PlayerInitialMarker.transform.position, PlayerInitialMarker.transform.rotation);
        Cloud = Instantiate(CloudPrefab, CloudInitialMarker.transform.position, CloudInitialMarker.transform.rotation);

        //Start round timer
        StartRoundTimer(RoundTimerDuration);
    }

    //Checks if the player has picked the correct option or not, and ends the round accordingly
    private void CheckWinCondition()
    {
        if (SelectedOption.CompareTag("GoodOption"))
            EndRoundGood();
        else
            EndRoundBad();
    }

    //End current round with correct choice by player
    private void EndRoundGood()
    {
        Score += ScorePerRound;
        Score += Mathf.Ceil(RoundTimer);
        RoundHUD.GetComponent<RoundHUDController>().SetScoreText("Score: " + Score);

        RoundOver = true;
        RoundHUD.SetActive(false);
        RoundEndGoodHUD.SetActive(true);
    }

    //End round with incorrect choice by player
    private void EndRoundBad()
    {
        RoundOver = true;
        RoundHUD.SetActive(false);
        RoundEndBadHUD.SetActive(true);
        GameOver = true;
    }

    //Clean up current round and start the next round
    private void NextRound()
    {
        //Hide round end HUD
        RoundEndGoodHUD.SetActive(false);

        //Destroy round objects
        Destroy(EnvironmentScape);
        Destroy(Options[0]);
        Destroy(Options[1]);
        Destroy(Options[2]);
        Destroy(Options[3]);
        Destroy(Player);
        Destroy(Cloud);

        //Start a new round
        StartRound();
    }

    //Restarts the game
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Start the current round timer
    private void StartRoundTimer(float duration)
    {
        RoundTimer = duration;
        RoundHUD.GetComponent<RoundHUDController>().SetTimerText(RoundTimer);
    }

    //Elapse timer for current update cycle
    private void ElapseRoundTimer()
    {
        RoundTimer -= Time.deltaTime;
        if (RoundTimer < 0f)
        {
            RoundTimer = 0f;
            EndRoundBad();
        }
        RoundHUD.GetComponent<RoundHUDController>().SetTimerText(RoundTimer);
    }

    //Raycast from camera to determine if an object is clicked
    private void ClickObject()
    {
        //Raycast from camera to mouse position to detect click on object
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //Get the hit GameObject
            GameObject hitObject = hit.collider.gameObject;

            //If hitObject is an option, set it as the player's destination
            if (hitObject.CompareTag("GoodOption") || hitObject.CompareTag("BadOption"))
            {
                SelectedOption = hitObject;
                TriggerPlayerAnimation();
            }
        }
    }

    //Trigger the player object to begin animating
    public void TriggerPlayerAnimation()
    {
        Player.GetComponent<Player>().Destination = SelectedOption.transform.parent.Find("PlayerPlaceMarker").gameObject;
        Player.GetComponent<Player>().FinishedAnimating = false;
    }

    //Trigger the cloud object to begin animating
    public void TriggerCloudAnimation()
    {
        Debug.Log(Options[0].transform.parent == null);
        Cloud.GetComponent<Cloud>().Destinations = Options.ToList().Select(o => o.transform.parent.Find("CloudPlaceMarker").gameObject).ToList();
        Cloud.GetComponent<Cloud>().FinishedAnimating = false;
    }
}
