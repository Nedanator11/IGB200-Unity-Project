using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public GameObject[,] Board;

    [Header("Grid Geometry")]
    public int GridDimension = 4;
    public Vector2 GridMinBounds = new Vector2(-8f, -8f);
    public Vector2 GridMaxBounds = new Vector2(8f, 8f);

    [Header("Tiles")]
    public GameObject StartTile;
    public int StartTilePosZ;
    public GameObject EndTile;
    public int EndTilePosZ;
    public GameObject Turn90Tile;
    public float Turn90Weight;
    public GameObject StraightTile;
    public float StraightWeight;
    public GameObject TSplitTile;
    public float TSplitWeight;
    public GameObject CrossSplitTile;
    public float CrossSplitWeight;

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
        GameObject startTile = Instantiate(StartTile, transform.position, transform.rotation, transform);
        GameObject endTile = Instantiate(EndTile, transform.position, transform.rotation, transform);

        //Move tiles to world position & scale
        startTile.transform.Translate(startTileX, 0f, startTileZ);
        startTile.transform.localScale = new Vector3(0.1f * tileLengthX, transform.localScale.y, 0.1f * tileLengthZ);
        endTile.transform.Translate(endTileX, 0f, endTileZ);
        endTile.transform.localScale = new Vector3(0.1f * tileLengthX, transform.localScale.y, 0.1f * tileLengthZ);

        //Set tile variables
        startTile.GetComponent<Tile>().TileGrid = this;
        startTile.GetComponent<Tile>().GridPosition = new int[2] { 0, StartTilePosZ };
        endTile.GetComponent<Tile>().TileGrid = this;
        endTile.GetComponent<Tile>().GridPosition = new int[2] { GridDimension + 1, EndTilePosZ };

        //Add tiles to board
        Board[0, StartTilePosZ] = startTile;
        Board[GridDimension + 1, EndTilePosZ] = endTile;

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
                    prefab = Turn90Tile;
                else if (rand <= StraightThreshold)
                    prefab = StraightTile;
                else if (rand <= TSplitThreshold)
                    prefab = TSplitTile;
                else //CrossSplit
                    prefab = CrossSplitTile;

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
        //Loop over all board elements, and destroy each object found
        for (int i = 0; i < Board.GetLength(0); i++)
            for (int j = 0; j < Board.GetLength(1); j++)
                if (Board[i, j])
                    Destroy(Board[i, j]);
    }

    public bool DetectCompleteCircuit()
    {
        //Initialise tile search lists
        List<Tile> visitedTiles = new List<Tile>();
        List<Tile> frontierTiles = new List<Tile>();

        //Add the start tile to the frontier
        frontierTiles.Add(Board[0, StartTilePosZ].GetComponent<Tile>());

        //Loop over all frontier tiles until none remain
        while (frontierTiles.Count > 0)
        {
            //Get the first tile in the frontier
            Tile frontierTile = frontierTiles[0];

            //Retrieve the tiles connected to the current frontier tile
            List<Tile> connectedTiles = frontierTile.GetConnectedTiles();

            //Loop over all new tiles and add them to the frontier, or discard if already visited
            foreach (Tile newTile in connectedTiles)
            {
                if (!frontierTiles.Contains(newTile) && !visitedTiles.Contains(newTile))
                    frontierTiles.Add(newTile);
            }

            //Remove current frontier tile from the frontier and add it to the visited tiles
            frontierTiles.Remove(frontierTile);
            visitedTiles.Add(frontierTile);
        }

        //If end tile has been visited, the circuit connects start to end
        bool circuitComplete = visitedTiles.Contains(Board[GridDimension + 1, EndTilePosZ].GetComponent<Tile>());

        //If circuit is completed, show all visited connected cables
        if (circuitComplete)
            foreach (Tile tile in visitedTiles)
                tile.ConnectedColour = Color.green;

        return circuitComplete;
    }

    //Check whether a tile exists at the input grid position
    public bool TileExists(int[] gridPosition)
    {
        if (gridPosition[0] < 0 || gridPosition[0] >= Board.GetLength(0) || gridPosition[1] < 0 || gridPosition[1] >= Board.GetLength(1))
            return false;
        return (Board[gridPosition[0], gridPosition[1]] != null);
    }
}
