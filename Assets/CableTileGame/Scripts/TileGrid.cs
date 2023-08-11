using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public int GridDimension = 4;
    public Vector2 GridMinBounds = new Vector2(-8f, -8f);
    public Vector2 GridMaxBounds = new Vector2(8f, 8f);
    public GameObject[] TilePrefabs;

    private GameObject[,] Board;

    void Start()
    {
        //Instantiate Variables
        Board = new GameObject[GridDimension, GridDimension];

        //Generate a new board
        GenerateBoard();
    }

    private void GenerateBoard()
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

        //Iterate over grid positions & instantiate tiles
        float currentTileX = GridMinBounds[0] + tileCentreX;
        float currentTileZ = GridMinBounds[1] + tileCentreZ;
        for (int i = 0; i < GridDimension; i++)
        {
            for (int j = 0; j < GridDimension; j++)
            {
                //Instantiate a new tile, move to current tile position and scale to tile size
                GameObject tile = Instantiate(TilePrefabs[0], transform.position, transform.rotation);
                tile.transform.Translate(currentTileX, 0f, currentTileZ);
                tile.transform.localScale = new Vector3(0.1f * tileLengthX * 0.9f, transform.localScale.y, 0.1f * tileLengthZ * 0.9f);

                //Add tile to the board
                Board[i, j] = tile;

                //Increment current tile z-position
                currentTileZ += tileLengthZ;
            }

            //Increment current tile x-position & reset current tile z-position
            currentTileX += tileLengthX;
            currentTileZ = GridMinBounds[1] + tileCentreZ;
        }
    }
}
