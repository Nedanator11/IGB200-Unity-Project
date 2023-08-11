using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    top.ConnectedSides[2] = 1;
                }
            }
        }
        else //else, there is no top cable ...
        {
            //Disconnect cable
            ConnectedSides[0] = 0;

            //If there is an adjacent tile, ...
            if (top)
                top.ConnectedSides[2] = 0; //Disconnect adjacent tile's connected cable (if connected)
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
                    right.ConnectedSides[3] = 1;
                }
            }
        }
        else //else, there is no right cable ...
        {
            //Disconnect cable
            ConnectedSides[1] = 0;

            //If there is an adjacent tile, ...
            if (right)
                right.ConnectedSides[3] = 0; //Disconnect adjacent tile's connected cable (if connected)
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
                    bottom.ConnectedSides[0] = 1;
                }
            }
        }
        else //else, there is no bottom cable ...
        {
            //Disconnect cable
            ConnectedSides[2] = 0;

            //If there is an adjacent tile, ...
            if (bottom)
                bottom.ConnectedSides[2] = 0; //Disconnect adjacent tile's connected cable (if connected)
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
                    left.ConnectedSides[1] = 1;
                }
            }
        }
        else //else, there is no left cable ...
        {
            //Disconnect cable
            ConnectedSides[3] = 0;

            //If there is an adjacent tile, ...
            if (left)
                left.ConnectedSides[3] = 0; //Disconnect adjacent tile's connected cable (if connected)
        }














        ////Check for new connections
        //if (CableSides[0] == 1 && Top()) //Top
        //{
        //    Tile topTile = Top();
        //    if (topTile.CableSides[2] == 1)
        //    {
        //        ConnectedSides[0] = 1;
        //        topTile.ConnectedSides[2] = 1;
        //    }
        //}
        //if (CableSides[1] == 1 && Right()) //Right
        //{
        //    Tile rightTile = Right();
        //    if (rightTile.CableSides[3] == 1)
        //    {
        //        ConnectedSides[1] = 1;
        //        rightTile.ConnectedSides[3] = 1;
        //    }
        //}
        //if (CableSides[2] == 1 && Bottom()) //Bottom
        //{
        //    Tile bottomTile = Bottom();
        //    if (bottomTile.CableSides[0] == 1)
        //    {
        //        ConnectedSides[2] = 1;
        //        bottomTile.ConnectedSides[0] = 1;
        //    }
        //}
        //if (CableSides[3] == 1 && Left()) //Left
        //{
        //    Tile leftTile = Left();
        //    if (leftTile.CableSides[1] == 1)
        //    {
        //        ConnectedSides[3] = 1;
        //        leftTile.ConnectedSides[1] = 1;
        //    }
        //}

        ////Check for disconnections
        //if (CableSides[0] == 0 && ConnectedSides[0] == 1) //Top
        //{
        //    Tile topTile = Top();
        //    ConnectedSides[0] = 1;
        //    topTile.ConnectedSides[2] = 1;
        //}
        //if (CableSides[1] == 0 && ConnectedSides[1] == 1) //Right
        //{
        //    Tile rightTile = Right();
        //    ConnectedSides[1] = 1;
        //    rightTile.ConnectedSides[3] = 1;
        //}
        //if (CableSides[2] == 0 && ConnectedSides[2] == 1) //Bottom
        //{
        //    Tile bottomTile = Bottom();
        //    ConnectedSides[2] = 1;
        //    bottomTile.ConnectedSides[0] = 1;
        //}
        //if (CableSides[3] == 0 && ConnectedSides[3] == 1) //Left
        //{
        //    Tile leftTile = Left();
        //    ConnectedSides[3] = 1;
        //    leftTile.ConnectedSides[1] = 1;
        //}
    }

    #region Adjacent Tile Retrievers

    //Return the tile above this one (if exists)
    private Tile Top()
    {
        int[] adjacentGridPosition = new int[2] { GridPosition[0], GridPosition[1] + 1 };
        if (TileGrid.GridPositionInBounds(adjacentGridPosition))
            return TileGrid.Board[adjacentGridPosition[0], adjacentGridPosition[1]].GetComponent<Tile>();
        return null;
    }

    //Return the tile to the right of this one (if exists)
    private Tile Right()
    {
        int[] adjacentGridPosition = new int[2] { GridPosition[0] + 1, GridPosition[1] };
        if (TileGrid.GridPositionInBounds(adjacentGridPosition))
            return TileGrid.Board[adjacentGridPosition[0], adjacentGridPosition[1]].GetComponent<Tile>();
        return null;
    }

    //Return the tile below this one (if exists)
    private Tile Bottom()
    {
        int[] adjacentGridPosition = new int[2] { GridPosition[0], GridPosition[1] - 1 };
        if (TileGrid.GridPositionInBounds(adjacentGridPosition))
            return TileGrid.Board[adjacentGridPosition[0], adjacentGridPosition[1]].GetComponent<Tile>();
        return null;
    }

    //Return the tile to the left of this one (if exists)
    private Tile Left()
    {
        int[] adjacentGridPosition = new int[2] { GridPosition[0] - 1, GridPosition[1] };
        if (TileGrid.GridPositionInBounds(adjacentGridPosition))
            return TileGrid.Board[adjacentGridPosition[0], adjacentGridPosition[1]].GetComponent<Tile>();
        return null;
    }

    #endregion
}
