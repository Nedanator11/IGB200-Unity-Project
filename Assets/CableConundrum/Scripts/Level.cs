using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int levelInt;

    public float TimeLimit;
    public TileSetting StartTile;
    public TileSetting EndTile;
    public TileRow[] Rows;

    //Returns the dimension of the set grid
    public int GetGridDimension()
    {
        return Rows.Length + 2;
    }

    //Returns the tile prefab for the specified grid position
    public GameObject GetTileForGridPosition(int i, int j)
    {
        TileRow row = Rows[Rows.Length - i];
        GameObject tile = row.Tiles[j - 1];
        return tile;
    }
}

[System.Serializable]
public struct TileRow
{
    public GameObject[] Tiles;
}
