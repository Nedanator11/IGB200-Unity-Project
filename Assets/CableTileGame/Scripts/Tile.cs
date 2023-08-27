using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile : MonoBehaviour
{
    public TileGrid TileGrid;
    public int[] GridPosition;
    public int[] CableSides = new int[4] { 0, 0, 0, 0 }; //Filled from prefabs: { Top, Right, Bottom, Left }
    public int[] ConnectedSides = new int[4] { 0, 0, 0, 0 };

    //TEMP VARS
    public Color ConnectedColour = Color.green;
    public Color DisconnectedColour = Color.red;

    private void Update()
    {
        CheckConnectedSides();

        //========= TEMPORARY CODE: Placeholder for tile image assets ==========
        if (CableSides[0] == 1)
        {
            Vector3 start = transform.position;
            start.y += 0.01f;
            Vector3 dest = transform.position;
            dest.y += 0.01f;
            dest.z += transform.localScale.z * 5f;
            Debug.DrawLine(transform.position, dest, ConnectedSides[0] == 1 ? ConnectedColour : DisconnectedColour);
        }
        if (CableSides[1] == 1)
        {
            Vector3 start = transform.position;
            start.y += 0.01f;
            Vector3 dest = transform.position;
            dest.y += 0.01f;
            dest.x += transform.localScale.x * 5f;
            Debug.DrawLine(transform.position, dest, ConnectedSides[1] == 1 ? ConnectedColour : DisconnectedColour);
        }
        if (CableSides[2] == 1)
        {
            Vector3 start = transform.position;
            start.y += 0.01f;
            Vector3 dest = transform.position;
            dest.y += 0.01f;
            dest.z -= transform.localScale.z * 5f;
            Debug.DrawLine(transform.position, dest, ConnectedSides[2] == 1 ? ConnectedColour : DisconnectedColour);
        }
        if (CableSides[3] == 1)
        {
            Vector3 start = transform.position;
            start.y += 0.01f;
            Vector3 dest = transform.position;
            dest.y += 0.01f;
            dest.x -= transform.localScale.x * 5f;
            Debug.DrawLine(transform.position, dest, ConnectedSides[3] == 1 ? ConnectedColour : DisconnectedColour);
        }
        //========= TEMPORARY CODE: Placeholder for tile image assets ==========
    }

    public void ClockwiseRotate()
    {
        //Rotate gameObejct
        transform.Rotate(new Vector3(0f, 90f, 0f));

        //Rightshift CableSides array
        int[] newCableSides = new int[] { 0, 0, 0, 0 };
        newCableSides[0] = CableSides[3];
        newCableSides[1] = CableSides[0];
        newCableSides[2] = CableSides[1];
        newCableSides[3] = CableSides[2];
        CableSides = newCableSides;

        CheckConnectedSides();
    }

    //Check cable sides for connection to adjacent tiles
    public void CheckConnectedSides()
    {
        //Get adjacent tiles (if exists)
        Tile top = Top();
        Tile right = Right();
        Tile bottom = Bottom();
        Tile left = Left();

        //If there is a top cable, ...
        if (CableSides[0] == 1)
        {
            //Disconnect cable initially
            ConnectedSides[0] = 0;

            //If there is an adjacent tile, ...
            if (top)
            {
                if (top.CableSides[2] == 1) //If adjacent tile's adjacent side has a cable, ...
                {
                    //Connect cables
                    ConnectedSides[0] = 1;
                    //top.ConnectedSides[2] = 1;
                }
            }
        }
        else //else, there is no top cable ...
        {
            //Disconnect cable
            ConnectedSides[0] = 0;

            //If there is an adjacent tile, ...
            //if (top)
                //top.ConnectedSides[2] = 0; //Disconnect adjacent tile's connected cable (if connected)
        }

        //If there is a right cable, ...
        if (CableSides[1] == 1)
        {
            //Disconnect cable initially
            ConnectedSides[1] = 0;

            //If there is an adjacent tile, ...
            if (right)
            {
                if (right.CableSides[3] == 1) //If adjacent tile's adjacent side has a cable, ...
                {
                    //Connect cables
                    ConnectedSides[1] = 1;
                    //right.ConnectedSides[3] = 1;
                }
            }
        }
        else //else, there is no right cable ...
        {
            //Disconnect cable
            ConnectedSides[1] = 0;

            //If there is an adjacent tile, ...
            //if (right)
                //right.ConnectedSides[3] = 0; //Disconnect adjacent tile's connected cable (if connected)
        }

        //If there is a bottom cable, ...
        if (CableSides[2] == 1)
        {
            //Disconnect cable initially
            ConnectedSides[2] = 0;

            //If there is an adjacent tile, ...
            if (bottom)
            {
                if (bottom.CableSides[0] == 1) //If adjacent tile's adjacent side has a cable, ...
                {
                    //Connect cables
                    ConnectedSides[2] = 1;
                    //bottom.ConnectedSides[0] = 1;
                }
            }
        }
        else //else, there is no bottom cable ...
        {
            //Disconnect cable
            ConnectedSides[2] = 0;

            //If there is an adjacent tile, ...
            //if (bottom)
                //bottom.ConnectedSides[2] = 0; //Disconnect adjacent tile's connected cable (if connected)
        }

        //If there is a left cable, ...
        if (CableSides[3] == 1)
        {
            //Disconnect cable initially
            ConnectedSides[3] = 0;

            //If there is an adjacent tile, ...
            if (left)
            {
                if (left.CableSides[1] == 1) //If adjacent tile's adjacent side has a cable, ...
                {
                    //Connect cables
                    ConnectedSides[3] = 1;
                    //left.ConnectedSides[1] = 1;
                }
            }
        }
        else //else, there is no left cable ...
        {
            //Disconnect cable
            ConnectedSides[3] = 0;

            //If there is an adjacent tile, ...
            //if (left)
                //left.ConnectedSides[3] = 0; //Disconnect adjacent tile's connected cable (if connected)
        }
    }

    #region Adjacent Tile Retrievers

    //Return the tile above this one (if exists)
    private Tile Top()
    {
        int[] adjacentGridPosition = new int[2] { GridPosition[0], GridPosition[1] + 1 };
        if (TileGrid.TileExists(adjacentGridPosition))
            return TileGrid.Board[adjacentGridPosition[0], adjacentGridPosition[1]].GetComponent<Tile>();
        return null;
    }

    //Return the tile to the right of this one (if exists)
    private Tile Right()
    {
        int[] adjacentGridPosition = new int[2] { GridPosition[0] + 1, GridPosition[1] };
        if (TileGrid.TileExists(adjacentGridPosition))
            return TileGrid.Board[adjacentGridPosition[0], adjacentGridPosition[1]].GetComponent<Tile>();
        return null;
    }

    //Return the tile below this one (if exists)
    private Tile Bottom()
    {
        int[] adjacentGridPosition = new int[2] { GridPosition[0], GridPosition[1] - 1 };
        if (TileGrid.TileExists(adjacentGridPosition))
            return TileGrid.Board[adjacentGridPosition[0], adjacentGridPosition[1]].GetComponent<Tile>();
        return null;
    }

    //Return the tile to the left of this one (if exists)
    private Tile Left()
    {
        int[] adjacentGridPosition = new int[2] { GridPosition[0] - 1, GridPosition[1] };
        if (TileGrid.TileExists(adjacentGridPosition))
            return TileGrid.Board[adjacentGridPosition[0], adjacentGridPosition[1]].GetComponent<Tile>();
        return null;
    }

    //Returns the list of adjacent tiles that this tile has connected cables with
    public List<Tile> GetConnectedTiles()
    {
        List<Tile> tiles = new List<Tile>();

        //Test sides for connected cable, add adjacent tile if connected
        if (ConnectedSides[0] == 1)
            tiles.Add(Top());
        if (ConnectedSides[1] == 1)
            tiles.Add(Right());
        if (ConnectedSides[2] == 1)
            tiles.Add(Bottom());
        if (ConnectedSides[3] == 1)
            tiles.Add(Left());

        return tiles;
    }

    #endregion
}
