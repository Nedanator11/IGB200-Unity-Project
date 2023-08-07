using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    private Tile[,] Grid;
    public int GridDimension = 4;

    // Start is called before the first frame update
    void Start()
    {
        Grid = new Tile[GridDimension, GridDimension];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
