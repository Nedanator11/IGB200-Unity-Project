using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //Singleton Setup
    public static GameManager instance = null;

    //Reference Variables
    public Camera MainCamera;
    public GameObject TileGrid;

    [Header("HUD")]
    public GameObject CompleteHUD;

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

    private void Update()
    {
        //Detect player input events
        if (Input.GetMouseButtonDown(0))
            LeftMouseButtonDown();
        if (Input.GetKeyDown("space"))
            SpaceBarDown();
        if (Input.GetKeyDown("r"))
            RKeyDown();
    }

    //Left mouse button down event
    private void LeftMouseButtonDown()
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
                hitObject.GetComponent<Tile>().ClockwiseRotate();
        }
    }

    //Space bar down event
    private void SpaceBarDown()
    {
        if (TileGrid.GetComponent<TileGrid>().DetectCompleteCircuit())
            CompleteHUD.SetActive(true);
    }

    //'R' key down event
    private void RKeyDown()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
