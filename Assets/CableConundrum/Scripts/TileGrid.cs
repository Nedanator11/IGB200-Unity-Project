using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public GameObject[,] Board;

    //Path-Finding
    private GameObject StartTile;
    private GameObject EndTile;
    private Dictionary<Tile, Tile> CameFrom = new Dictionary<Tile, Tile>(); // <To, From>
    private List<Tile> OpenList = new List<Tile>();
    private List<Tile> ClosedList = new List<Tile>();
    private List<Tile> GeneratedPath = new List<Tile>();
    private List<Tile> DetectedPath = new List<Tile>();
    public Color DetectedPathColor = Color.green;

    [Header("Grid Geometry")]
    public int GridDimension = 4;
    public Vector2 GridMinBounds = new Vector2(-8f, -8f);
    public Vector2 GridMaxBounds = new Vector2(8f, 8f);

    [Header("Tiles")]
    public GameObject StartTilePrefab;
    public int StartTilePosZ;
    public GameObject EndTilePrefab;
    public int EndTilePosZ;
    public GameObject Turn90TilePrefab;
    public float Turn90Weight;
    public GameObject StraightTilePrefab;
    public float StraightWeight;
    public GameObject TSplitTilePrefab;
    public float TSplitWeight;
    public GameObject CrossSplitTilePrefab;
    public float CrossSplitWeight;

    private void Update()
    {
        if (DetectedPath != null && DetectedPath.Count > 0) DebugDrawDetectedPath();
    }

    //Generate a new board of tiles
    public void GenerateBoard()
    {
        //Grid geometry calculations
        float gridLengthX = GridMaxBounds[0] - GridMinBounds[0];
        float gridLengthZ = GridMaxBounds[1] - GridMinBounds[1];
        float gridCentreX = gridLengthX / 2f;
        float gridCentreZ = gridLengthZ / 2f;

        //Tile geometrey calculations
        float tileLengthX = gridLengthX / GridDimension;
        float tileLengthZ = gridLengthZ / GridDimension;
        float tileCentreX = tileLengthX / 2f;
        float tileCentreZ = tileLengthZ / 2f;

        #region Start & End Tile Instantiation

        //World position calculation
        float startTileX = GridMinBounds[0] - tileCentreX;
        float startTileZ = GridMinBounds[1] - tileCentreZ + (tileLengthZ * StartTilePosZ);
        float endTileX = GridMaxBounds[0] + tileCentreX;
        float endTileZ = GridMinBounds[1] - tileCentreZ + (tileLengthZ * EndTilePosZ);

        //Instantiate the board
        Board = new GameObject[GridDimension + 2, GridDimension + 2];

        //Instantiate tiles
        StartTile = Instantiate(StartTilePrefab, transform.position, transform.rotation, transform);
        EndTile = Instantiate(EndTilePrefab, transform.position, transform.rotation, transform);

        //Move tiles to world position & scale
        StartTile.transform.Translate(startTileX, 0f, startTileZ);
        StartTile.transform.localScale = new Vector3(0.1f * tileLengthX, transform.localScale.y, 0.1f * tileLengthZ);
        EndTile.transform.Translate(endTileX, 0f, endTileZ);
        EndTile.transform.localScale = new Vector3(0.1f * tileLengthX, transform.localScale.y, 0.1f * tileLengthZ);

        //Set tile variables
        StartTile.GetComponent<Tile>().TileGrid = this;
        StartTile.GetComponent<Tile>().GridPosition = new int[2] { 0, StartTilePosZ };
        EndTile.GetComponent<Tile>().TileGrid = this;
        EndTile.GetComponent<Tile>().GridPosition = new int[2] { GridDimension + 1, EndTilePosZ };

        //Add tiles to board
        Board[0, StartTilePosZ] = StartTile;
        Board[GridDimension + 1, EndTilePosZ] = EndTile;

        #endregion

        //Iterate over grid positions & instantiate tiles
        float currentTileX = GridMinBounds[0] + tileCentreX;
        float currentTileZ = GridMinBounds[1] + tileCentreZ;
        for (int i = 1; i < GridDimension + 1; i++)
        {
            for (int j = 1; j < GridDimension + 1; j++)
            {
                //Choose tile prefab
                float Turn90Threshold = Turn90Weight;
                float StraightThreshold = Turn90Threshold + StraightWeight;
                float TSplitThreshold = StraightThreshold + TSplitWeight;
                float CrossSplitThreshold = TSplitThreshold + CrossSplitWeight;
                float rand = Random.Range(0f, CrossSplitThreshold);
                GameObject prefab;
                if (rand <= Turn90Threshold)
                    prefab = Turn90TilePrefab;
                else if (rand <= StraightThreshold)
                    prefab = StraightTilePrefab;
                else if (rand <= TSplitThreshold)
                    prefab = TSplitTilePrefab;
                else //CrossSplit
                    prefab = CrossSplitTilePrefab;

                //Instantiate a new tile, move to current tile position and scale to tile size
                GameObject tile = Instantiate(prefab, transform.position, transform.rotation, transform);
                tile.transform.Translate(currentTileX, 0f, currentTileZ);
                tile.transform.localScale = new Vector3(0.1f * tileLengthX, transform.localScale.y, 0.1f * tileLengthZ);

                //Add tile to the board
                tile.GetComponent<Tile>().TileGrid = this;
                tile.GetComponent<Tile>().GridPosition = new int[2] { i, j };
                Board[i, j] = tile;

                //Increment current tile z-position
                currentTileZ += tileLengthZ;
            }

            //Increment current tile x-position & reset current tile z-position
            currentTileX += tileLengthX;
            currentTileZ = GridMinBounds[1] + tileCentreZ;
        }

        //Loop over all tiles, jumble rotations, initialise cable connections
        for (int i = 1; i < GridDimension + 1; i++)
        {
            for (int j = 1; j < GridDimension + 1; j++)
            {
                Tile tile = Board[i, j].GetComponent<Tile>();
                for (int r = 0; r < Random.Range(0, 4); r++)
                    tile.ClockwiseRotate();
                tile.CheckConnectedSides();
            }
        }
    }

    //Destroys all board tiles
    public void DestroyBoard()
    {
        //Path-clearing
        StartTile = null;
        EndTile = null;
        CameFrom.Clear();
        OpenList.Clear();
        ClosedList.Clear();
        GeneratedPath.Clear();
        DetectedPath.Clear();

        //Loop over all board elements, and destroy each object found
        for (int i = 0; i < Board.GetLength(0); i++)
            for (int j = 0; j < Board.GetLength(1); j++)
                if (Board[i, j])
                    Destroy(Board[i, j]);
    }

    //Detect whether or not the circuit contains a vaild path
    public bool DetectCompleteCircuit()
    {
        //Perform A* search
        DetectedPath = AStarSearch(StartTile.GetComponent<Tile>(), EndTile.GetComponent<Tile>());

        if (DetectedPath == null) return false;
        if (DetectedPath.Count == 0) return false;
        return true;
    }

    //Draw the detected path
    private void DebugDrawDetectedPath()
    {
        Debug.Log("Drawing detected path!");
        for (int i = 0; i < DetectedPath.Count - 1; i++)
        {
            Vector3 posStart = DetectedPath[i].transform.position;
            posStart.x += 0.1f;
            posStart.z += 0.1f;
            Vector3 posEnd = DetectedPath[i + 1].transform.position;
            posEnd.x += 0.1f;
            posEnd.z += 0.1f;

            Debug.DrawLine(posStart, posEnd, DetectedPathColor);
        }
    }

    //Perform an A* path-finding search from the given 'start' tile to the given 'end' tile
    private List<Tile> AStarSearch(Tile start, Tile end)
    {
        //Clear Lists/Dictionaries at start
        OpenList.Clear();
        ClosedList.Clear();
        CameFrom.Clear();

        //Begin
        OpenList.Add(start);
        float gScore = 0;

        //While there are open tiles remaining
        while (OpenList.Count > 0)
        {
            //Find the tile in openList that has the lowest fScore
            Tile currentTile = BestOpenListFScore(start, end);

            //Found the end, reconstruct entire path and return
            if (currentTile == end)
                return ReconstructPath(CameFrom, currentTile);

            //Close current Tile
            OpenList.Remove(currentTile);
            ClosedList.Add(currentTile);

            //Look at each connected tile to current tile
            List<Tile> connectedTiles = currentTile.GetConnectedTiles();
            for (int i = 0; i < connectedTiles.Count; i++)
            {
                Tile thisNeighbourTile = connectedTiles[i];

                //Ignore if neighbour tile is attached
                if (!ClosedList.Contains(thisNeighbourTile))
                {
                    //Distance from current to the next tile
                    float tentativeGScore = Heuristic(start, currentTile) + Heuristic(currentTile, thisNeighbourTile);

                    //Check to see if in openList or if new GScore is more sensible
                    if (!OpenList.Contains(thisNeighbourTile) || tentativeGScore < gScore)
                        OpenList.Add(thisNeighbourTile);

                    //Add to Dictionary - this neighour came from this parent
                    if (!CameFrom.ContainsKey(thisNeighbourTile))
                        CameFrom.Add(thisNeighbourTile, currentTile);

                    gScore = tentativeGScore;
                }
            }
        }

        //Return null if not path exists
        return null;
    }

    //The heuristic function for A* search
    private float Heuristic(Tile a, Tile b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    //Find the best tile in the open list with the lowest f-score
    private Tile BestOpenListFScore(Tile start, Tile goal)
    {
        int bestIndex = 0;

        for (int i = 0; i < OpenList.Count; i++)
        {
            if ((Heuristic(OpenList[i], start) + Heuristic(OpenList[i], goal)) < (Heuristic(OpenList[bestIndex], start) + Heuristic(OpenList[bestIndex], goal)))
                bestIndex = i;
        }

        Tile bestTile = OpenList[bestIndex];
        return bestTile;
    }

    //Reconstruct path from the current tile to the start tile
    private List<Tile> ReconstructPath(Dictionary<Tile, Tile> CF, Tile current)
    {
        List<Tile> finalPath = new List<Tile>();
        finalPath.Add(current);

        while (CF.ContainsKey(current))
        {
            current = CF[current];
            finalPath.Add(current);
        }

        finalPath.Reverse();
        return finalPath;
    }

    //Check whether a tile exists at the input grid position
    public bool TileExists(int[] gridPosition)
    {
        if (gridPosition[0] < 0 || gridPosition[0] >= Board.GetLength(0) || gridPosition[1] < 0 || gridPosition[1] >= Board.GetLength(1))
            return false;
        return (Board[gridPosition[0], gridPosition[1]] != null);
    }
}
