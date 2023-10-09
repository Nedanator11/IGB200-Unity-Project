using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public GameObject[,] Graph;
    public GameObject[,] Board;

    //Path-Finding
    private GameObject StartTile;
    private GameObject EndTile;
    public List<Tile> HazardTiles = new List<Tile>();
    private List<GridNode> OpenList = new List<GridNode>();
    private List<GridNode> ClosedList = new List<GridNode>();
    private List<GridNode> GeneratedPath = new List<GridNode>();
    private List<GridNode> DetectedPath = new List<GridNode>();
    public Color GeneratedPathColor = Color.yellow;
    public Color DetectedPathColor = Color.green;

    [Header("Grid Geometry")]
    public int GridDimension = 4;
    public Vector2 GridMinBounds = new Vector2(-8f, -8f);
    public Vector2 GridMaxBounds = new Vector2(8f, 8f);

    [Header("Tiles")]
    public TileSetting StartTileSettings;
    public TileSetting EndTileSettings;
    public GameObject Turn90TilePrefab;
    public float Turn90Weight;
    public GameObject StraightTilePrefab;
    public float StraightWeight;
    public GameObject TSplitTilePrefab;
    public float TSplitWeight;
    public GameObject CrossSplitTilePrefab;
    public float CrossSplitWeight;
    public GameObject HazardTilePrefab;
    public float HazardTilePercent;

    private void Update()
    {
        if (GeneratedPath != null && GeneratedPath.Count > 0) DebugDrawGeneratedPath();
        if (DetectedPath != null && DetectedPath.Count > 0) DebugDrawDetectedPath();
    }

    //Generate a new board of tiles
    public void GenerateBoard()
    {
        //Grid geometry calculations
        float gridLengthX = GridMaxBounds[0] - GridMinBounds[0];
        float gridLengthZ = GridMaxBounds[1] - GridMinBounds[1];

        //Tile geometry calculations
        float tileLengthX = gridLengthX / (GridDimension);
        float tileLengthZ = gridLengthZ / (GridDimension);
        float tileCentreX = tileLengthX / 2f;
        float tileCentreZ = tileLengthZ / 2f;

        #region Node, Graph & Path Generation

        #region Start & End Node Instantiation

        //World position calculation
        float startNodeX = GridMinBounds[0] - tileCentreX;
        float startNodeZ = GridMinBounds[1] - tileCentreZ + (tileLengthZ * StartTileSettings.GridPosition.y);
        float endNodeX = GridMaxBounds[0] + tileCentreX;
        float endNodeZ = GridMinBounds[1] - tileCentreZ + (tileLengthZ * EndTileSettings.GridPosition.y);

        //Instantiate the board
        Graph = new GameObject[GridDimension + 2, GridDimension + 2];

        //Instantiate start/end nodes
        GameObject startNode = new GameObject("Start Node");
        startNode.AddComponent<GridNode>();
        startNode.transform.parent = transform;
        startNode.transform.position = transform.position;
        startNode.transform.rotation = transform.rotation;
        startNode.transform.Translate(startNodeX, 0f, startNodeZ);
        GameObject endNode = new GameObject("End Node");
        endNode.AddComponent<GridNode>();
        endNode.transform.parent = transform;
        endNode.transform.position = transform.position;
        endNode.transform.rotation = transform.rotation;
        endNode.transform.Translate(endNodeX, 0f, endNodeZ);

        //Add nodes to board
        startNode.GetComponent<GridNode>().GridPosition = new int[] { 0, (int)StartTileSettings.GridPosition.y };
        Graph[0, (int)StartTileSettings.GridPosition.y] = startNode;
        endNode.GetComponent<GridNode>().GridPosition = new int[] { GridDimension + 1, (int)EndTileSettings.GridPosition.y };
        Graph[GridDimension + 1, (int)EndTileSettings.GridPosition.y] = endNode;

        #endregion

        //Loop over grid positions & instantiate nodes
        float currentNodeX = GridMinBounds[0] - tileCentreX + tileLengthX;
        float currentNodeZ = GridMinBounds[1] - tileCentreZ + tileLengthZ;
        for (int i = 1; i < GridDimension + 1; i++)
        {
            for (int j = 1; j < GridDimension + 1; j++)
            {
                //Create new node and move to position
                GameObject newNode = new GameObject("Node [" + i + "," + j + "]");
                newNode.AddComponent<GridNode>();
                newNode.transform.parent = transform;
                newNode.transform.position = transform.position;
                newNode.transform.rotation = transform.rotation;
                newNode.transform.Translate(currentNodeX, 0f, currentNodeZ);

                //Add node to the board
                newNode.GetComponent<GridNode>().GridPosition = new int[] { i, j };
                Graph[i, j] = newNode;

                //Increment current tile z-position
                currentNodeZ += tileLengthZ;
            }

            //Increment current tile x-position & reset current tile z-position
            currentNodeX += tileLengthX;
            currentNodeZ = GridMinBounds[1] - tileCentreZ + tileLengthZ;
        }

        //Loop over all nodes and populate node connections & costsswitch (StartTileSettings.Rotation)
        switch (StartTileSettings.Rotation)
        {
            case TileSetting.TileRotation.Up:
                startNode.GetComponent<GridNode>().connectedNodes[0] = Graph[(int)StartTileSettings.GridPosition.x, (int)StartTileSettings.GridPosition.y + 1].GetComponent<GridNode>();
            break;
            case TileSetting.TileRotation.Right:
                startNode.GetComponent<GridNode>().connectedNodes[1] = Graph[(int)StartTileSettings.GridPosition.x + 1, (int)StartTileSettings.GridPosition.y].GetComponent<GridNode>();
            break;
            case TileSetting.TileRotation.Down:
                startNode.GetComponent<GridNode>().connectedNodes[2] = Graph[(int)StartTileSettings.GridPosition.x, (int)StartTileSettings.GridPosition.y - 1].GetComponent<GridNode>();
            break;
            case TileSetting.TileRotation.Left:
                startNode.GetComponent<GridNode>().connectedNodes[3] = Graph[(int)StartTileSettings.GridPosition.x - 1, (int)StartTileSettings.GridPosition.y].GetComponent<GridNode>();
            break;
        }
        switch (EndTileSettings.Rotation)
        {
            case TileSetting.TileRotation.Up:
                endNode.GetComponent<GridNode>().connectedNodes[0] = Graph[(int)EndTileSettings.GridPosition.x, (int)EndTileSettings.GridPosition.y + 1].GetComponent<GridNode>();
                break;
            case TileSetting.TileRotation.Right:
                endNode.GetComponent<GridNode>().connectedNodes[1] = Graph[(int)EndTileSettings.GridPosition.x + 1, (int)EndTileSettings.GridPosition.y].GetComponent<GridNode>();
                break;
            case TileSetting.TileRotation.Down:
                endNode.GetComponent<GridNode>().connectedNodes[2] = Graph[(int)EndTileSettings.GridPosition.x, (int)EndTileSettings.GridPosition.y - 1].GetComponent<GridNode>();
                break;
            case TileSetting.TileRotation.Left:
                endNode.GetComponent<GridNode>().connectedNodes[3] = Graph[(int)EndTileSettings.GridPosition.x - 1, (int)EndTileSettings.GridPosition.y].GetComponent<GridNode>();
                break;
        }
        for (int i = 1; i < GridDimension + 1; i++)
        {
            for (int j = 1; j < GridDimension + 1; j++)
            {
                GridNode node = Graph[i, j].GetComponent<GridNode>();

                //Node above
                if (Graph[i, j + 1])
                {
                    //Connect to node above
                    node.connectedNodes[0] = Graph[i, j + 1].GetComponent<GridNode>();
                    node.adjacentNodes[0] = Graph[i, j + 1].GetComponent<GridNode>();

                    //If cost has not yet been set
                    if (node.connectionCosts[0] < 0f)
                    {
                        //Generate random cost
                        //float cost = Random.Range(tileLengthZ * 0.5f, tileLengthZ * 1.5f);
                        float cost = Random.Range(100, 1000);

                        //Set cost between both nodes
                        node.connectionCosts[0] = cost;
                        node.connectedNodes[0].connectionCosts[2] = cost;
                    }
                }

                //Node right
                if (Graph[i + 1, j])
                {
                    //Connect to node right
                    node.connectedNodes[1] = Graph[i + 1, j].GetComponent<GridNode>();
                    node.adjacentNodes[1] = Graph[i + 1, j].GetComponent<GridNode>();

                    //If cost has not yet been set
                    if (node.connectionCosts[1] < 0f)
                    {
                        //Generate random cost
                        //float cost = Random.Range(tileLengthX * 0.5f, tileLengthX * 1.5f);
                        float cost = Random.Range(100, 1000);

                        //Set cost between both nodes
                        node.connectionCosts[1] = cost;
                        node.connectedNodes[1].connectionCosts[3] = cost;
                    }
                }

                //Node down
                if (Graph[i, j - 1])
                {
                    //Connect to node below
                    node.connectedNodes[2] = Graph[i, j - 1].GetComponent<GridNode>();
                    node.adjacentNodes[2] = Graph[i, j - 1].GetComponent<GridNode>();

                    //If cost has not yet been set
                    if (node.connectionCosts[2] < 0f)
                    {
                        //Generate random cost
                        //float cost = Random.Range(tileLengthZ * 0.5f, tileLengthZ * 1.5f);
                        float cost = Random.Range(100, 1000);

                        //Set cost between both nodes
                        node.connectionCosts[2] = cost;
                        node.connectedNodes[2].connectionCosts[0] = cost;
                    }
                }

                //Node left
                if (Graph[i - 1, j])
                {
                    //Connect to node left
                    node.connectedNodes[3] = Graph[i - 1, j].GetComponent<GridNode>();
                    node.adjacentNodes[3] = Graph[i - 1, j].GetComponent<GridNode>();

                    //If cost has not yet been set
                    if (node.connectionCosts[3] < 0f)
                    {
                        //Generate random cost
                        //float cost = Random.Range(tileLengthX * 0.5f, tileLengthX * 1.5f);
                        float cost = Random.Range(100, 1000);

                        //Set cost between both nodes
                        node.connectionCosts[3] = cost;
                        node.connectedNodes[3].connectionCosts[1] = cost;
                    }
                }
            }
        }

        //Calculate straight-line distances to endNode for each node
        startNode.GetComponent<GridNode>().StraightLineDistanceToEnd = startNode.GetComponent<GridNode>().StraightLineDistanceTo(endNode.GetComponent<GridNode>());
        endNode.GetComponent<GridNode>().StraightLineDistanceToEnd = endNode.GetComponent<GridNode>().StraightLineDistanceTo(endNode.GetComponent<GridNode>());
        for (int i = 1; i < GridDimension + 1; i++)
            for (int j = 1; j < GridDimension + 1; j++)
                Graph[i, j].GetComponent<GridNode>().StraightLineDistanceToEnd = Graph[i, j].GetComponent<GridNode>().StraightLineDistanceTo(endNode.GetComponent<GridNode>());

        #region Hazard Node Generation

        //Populate list of potential hazard nodes
        List<GridNode> possibleHazardNodes = new List<GridNode>();
        for (int i = 1; i < GridDimension + 1; i++)
            for (int j = 1; j < GridDimension + 1; j++)
                possibleHazardNodes.Add(Graph[i, j].GetComponent<GridNode>());
        possibleHazardNodes.Remove(startNode.GetComponent<GridNode>().connectedNodes[1].GetComponent<GridNode>());
        possibleHazardNodes.Remove(endNode.GetComponent<GridNode>().connectedNodes[3].GetComponent<GridNode>());

        //Calculate max number of hazard nodes
        int totalNodes = GridDimension * GridDimension;
        int hazardMax = (int)(totalNodes * HazardTilePercent / 100f);
        if ((totalNodes * HazardTilePercent / 100f) % 1f > 0f && Random.Range(0f, 1f) <= (totalNodes * HazardTilePercent / 100f) % 1f)
            hazardMax += 1;

        //Generate hazard nodes
        List<GridNode> hazardNodes = new List<GridNode>();
        for (int i = 0; i < hazardMax; i++)
        {
            //If list is exhausted, break
            if (possibleHazardNodes.Count <= 0) break;

            //Retrieve random node and remove it from list of potential hazard nodes
            GridNode newHazard = possibleHazardNodes[Random.Range(0, possibleHazardNodes.Count)];
            possibleHazardNodes.Remove(newHazard);

            //Loop through connected nodes
            for (int c = 0; c < newHazard.connectedNodes.Length; c++)
            {
                //If there is an adjacent tile
                if (newHazard.connectedNodes[c])
                {
                    //Remove adjacent tile & diagonal clock-wise tile from possible hazard nodes
                    possibleHazardNodes.Remove(newHazard.connectedNodes[c]);
                    possibleHazardNodes.Remove(newHazard.connectedNodes[c].connectedNodes[(c + 1) % 4]);

                    //Sever connection between adjacent node and this one
                    newHazard.connectedNodes[c].connectedNodes[(c + 2) % 4] = null;
                    newHazard.connectedNodes[c] = null;
                }
            }

            //Add this node to the list of hazard nodes
            hazardNodes.Add(newHazard);
        }

        #endregion

        //Generate the least-cost path from start to end
        GeneratedPath = AStarSearch(startNode.GetComponent<GridNode>(), endNode.GetComponent<GridNode>());

        #endregion

        #region Tile Population

        //Instantiate the board
        Board = new GameObject[GridDimension + 2, GridDimension + 2];

        #region Start & End Tile Instantiation

        //Instantiate tiles & scale
        StartTile = Instantiate(StartTileSettings.TilePrefab, startNode.transform.position, startNode.transform.rotation, startNode.transform);
        EndTile = Instantiate(EndTileSettings.TilePrefab, endNode.transform.position, endNode.transform.rotation, endNode.transform);
        StartTile.transform.localScale = new Vector3(0.1f * tileLengthX, transform.localScale.y, 0.1f * tileLengthZ);
        EndTile.transform.localScale = new Vector3(0.1f * tileLengthX, transform.localScale.y, 0.1f * tileLengthZ);

        //Set tile variables
        StartTile.GetComponent<Tile>().TileGrid = this;
        StartTile.GetComponent<Tile>().GridPosition = new int[2] { 0, (int)StartTileSettings.GridPosition.y };
        EndTile.GetComponent<Tile>().TileGrid = this;
        EndTile.GetComponent<Tile>().GridPosition = new int[2] { GridDimension + 1, (int)EndTileSettings.GridPosition.y };

        //Add tiles to board
        Board[0, (int)StartTileSettings.GridPosition.y] = StartTile;
        Board[GridDimension + 1, (int)EndTileSettings.GridPosition.y] = EndTile;

        //Rotate tiles
        for (int i = 0; i < (int)StartTileSettings.Rotation; i++)
            StartTile.GetComponent<Tile>().RotateTile(true);
        for (int i = 0; i < (int)EndTileSettings.Rotation; i++)
            EndTile.GetComponent<Tile>().RotateTile(true);

        #endregion

        //Loop over all generated path nodes (excluding start & end)
        for (int i = 1; i < GeneratedPath.Count - 1; i++)
        {
            //Get the set of three sequential nodes
            GridNode nodePrev = GeneratedPath[i - 1];
            GridNode node = GeneratedPath[i];
            GridNode nodeNext = GeneratedPath[i + 1];

            //Get the number of adjacent hazard tiles
            int adjacentHazards = node.adjacentNodes.Where(cn => hazardNodes.Contains(cn)).Count();


            //If the three sequenced nodes lie on the same x or same y, the path is straight
            GameObject tilePrefab;
            if ((nodePrev.GridPosition[0] == node.GridPosition[0] && node.GridPosition[0] == nodeNext.GridPosition[0]) ||
                (nodePrev.GridPosition[1] == node.GridPosition[1] && node.GridPosition[1] == nodeNext.GridPosition[1]))
            {
                //Calculate weight thresholds
                float StraightThreshold = Turn90Weight + StraightWeight;
                float TSplitThreshold = StraightThreshold + TSplitWeight;
                float CrossSplitThreshold = TSplitThreshold + CrossSplitWeight;
                if (adjacentHazards >= 2) TSplitThreshold = StraightThreshold;
                if (adjacentHazards >= 1) CrossSplitThreshold = TSplitThreshold;

                //Choose tile prefab
                float rand = Random.Range(0f, CrossSplitThreshold);
                if (rand <= StraightThreshold)
                    tilePrefab = StraightTilePrefab;
                else if (rand <= TSplitThreshold)
                    tilePrefab = TSplitTilePrefab;
                else //CrossSplit
                    tilePrefab = CrossSplitTilePrefab;
            }
            else //The path is a turn
            {
                //Calculate weight thresholds
                float Turn90Threshold = Turn90Weight + StraightWeight;
                float TSplitThreshold = Turn90Threshold + TSplitWeight;
                float CrossSplitThreshold = TSplitThreshold + CrossSplitWeight;
                if (adjacentHazards >= 2) TSplitThreshold = Turn90Threshold;
                if (adjacentHazards >= 1) CrossSplitThreshold = TSplitThreshold;

                //Choose tile prefab
                float rand = Random.Range(0f, CrossSplitThreshold);
                if (rand <= Turn90Threshold)
                    tilePrefab = Turn90TilePrefab;
                else if (rand <= TSplitThreshold)
                    tilePrefab = TSplitTilePrefab;
                else //CrossSplit
                    tilePrefab = CrossSplitTilePrefab;
            }

            //Instantiate a new tile and scale to tile size
            GameObject tile = Instantiate(tilePrefab, node.transform.position, node.transform.rotation, node.transform);
            tile.transform.localScale = new Vector3(0.1f * tileLengthX, transform.localScale.y, 0.1f * tileLengthZ);

            //Add tile to the board
            tile.GetComponent<Tile>().TileGrid = this;
            tile.GetComponent<Tile>().GridPosition = new int[2] { node.GridPosition[0], node.GridPosition[1] };
            Board[node.GridPosition[0], node.GridPosition[1]] = tile;

            //Apply random rotation to tile
            for (int r = 0; r < Random.Range(0, 4); r++)
                tile.GetComponent<Tile>().RotateTile(true);
        }

        //Loop over all hazard nodes
        for (int i = 0; i < hazardNodes.Count; i++)
        {
            GridNode node = hazardNodes[i];

            //Instantiate a new tile and scale to tile size
            GameObject tile = Instantiate(HazardTilePrefab, node.transform.position, node.transform.rotation, node.transform);
            tile.transform.localScale = new Vector3(0.1f * tileLengthX, transform.localScale.y, 0.1f * tileLengthZ);

            //Add tile to the board
            tile.GetComponent<Tile>().TileGrid = this;
            tile.GetComponent<Tile>().GridPosition = new int[2] { node.GridPosition[0], node.GridPosition[1] };
            Board[node.GridPosition[0], node.GridPosition[1]] = tile;

            //Apply random rotation to tile
            for (int r = 0; r < Random.Range(0, 4); r++)
                tile.GetComponent<Tile>().RotateTile(true);

            //Add tile to hazard tiles list
            HazardTiles.Add(tile.GetComponent<Tile>());
        }

        //Loop over all remaining empty board spaces
        for (int i = 1; i < Board.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < Board.GetLength(1) - 1; j++)
            {
                //Skip already generated tiles
                if (Board[i, j] != null)
                    continue;

                //Get correponding node
                GameObject node = Graph[i, j];

                //Get the number of adjacent hazard tiles
                int adjacentHazards = node.GetComponent<GridNode>().adjacentNodes.Where(cn => hazardNodes.Contains(cn)).Count();

                //Calculate weight thresholds
                float StraightThreshold = StraightWeight;
                float Turn90Threshold = StraightThreshold + Turn90Weight;
                float TSplitThreshold = Turn90Threshold + TSplitWeight;
                float CrossSplitThreshold = TSplitThreshold + CrossSplitWeight;
                if (adjacentHazards >= 2) Turn90Threshold = StraightThreshold;
                if (adjacentHazards >= 2) TSplitThreshold = Turn90Threshold;
                if (adjacentHazards >= 1) CrossSplitThreshold = TSplitThreshold;

                //Choose tile prefab
                float rand = Random.Range(0f, CrossSplitThreshold);
                GameObject tilePrefab;
                if (rand <= StraightThreshold)
                    tilePrefab = StraightTilePrefab;
                else if (rand <= Turn90Threshold)
                    tilePrefab = Turn90TilePrefab;
                else if (rand <= TSplitThreshold)
                    tilePrefab = TSplitTilePrefab;
                else //CrossSplit
                    tilePrefab = CrossSplitTilePrefab;

                //Instantiate a new tile and scale to tile size
                GameObject tile = Instantiate(tilePrefab, node.transform.position, node.transform.rotation, node.transform);
                tile.transform.localScale = new Vector3(0.1f * tileLengthX, transform.localScale.y, 0.1f * tileLengthZ);

                //Add tile to the board
                tile.GetComponent<Tile>().TileGrid = this;
                tile.GetComponent<Tile>().GridPosition = new int[2] { i, j };
                Board[i, j] = tile;

                //Apply random rotation to tile
                for (int r = 0; r < Random.Range(0, 4); r++)
                    tile.GetComponent<Tile>().RotateTile(true);
            }
        }

        #endregion
    }

    //Generate a new board of tiles
    public void LoadLevel(Level level)
    {
        //Load level settings
        GridDimension = level.GetGridDimension();
        StartTileSettings = level.StartTile;
        EndTileSettings = level.EndTile;

        //Grid geometry calculations
        float gridLengthX = GridMaxBounds[0] - GridMinBounds[0];
        float gridLengthZ = GridMaxBounds[1] - GridMinBounds[1];

        //Tile geometry calculations
        float tileLengthX = gridLengthX / (GridDimension - 2);
        float tileLengthZ = gridLengthZ / (GridDimension - 2);
        float tileCentreX = tileLengthX / 2f;
        float tileCentreZ = tileLengthZ / 2f;

        #region Node & Graph Generation

        #region Start & End Node Instantiation

        //World position calculation
        float startNodeX = GridMinBounds[0] - tileCentreX + (tileLengthX * StartTileSettings.GridPosition.x);
        float startNodeZ = GridMinBounds[1] - tileCentreZ + (tileLengthZ * StartTileSettings.GridPosition.y);
        float endNodeX = GridMinBounds[0] - tileCentreX + (tileLengthX * EndTileSettings.GridPosition.x);
        float endNodeZ = GridMinBounds[1] - tileCentreZ + (tileLengthZ * EndTileSettings.GridPosition.y);

        //Instantiate the board
        Graph = new GameObject[GridDimension, GridDimension];

        //Instantiate start/end nodes
        GameObject startNode = new GameObject("Start Node");
        startNode.AddComponent<GridNode>();
        startNode.transform.parent = transform;
        startNode.transform.position = transform.position;
        startNode.transform.rotation = transform.rotation;
        startNode.transform.Translate(startNodeX, 0f, startNodeZ);
        GameObject endNode = new GameObject("End Node");
        endNode.AddComponent<GridNode>();
        endNode.transform.parent = transform;
        endNode.transform.position = transform.position;
        endNode.transform.rotation = transform.rotation;
        endNode.transform.Translate(endNodeX, 0f, endNodeZ);

        //Add nodes to board
        startNode.GetComponent<GridNode>().GridPosition = new int[] { (int)StartTileSettings.GridPosition.x, (int)StartTileSettings.GridPosition.y };
        Graph[(int)StartTileSettings.GridPosition.x, (int)StartTileSettings.GridPosition.y] = startNode;
        endNode.GetComponent<GridNode>().GridPosition = new int[] { (int)EndTileSettings.GridPosition.x, (int)EndTileSettings.GridPosition.y };
        Graph[(int)EndTileSettings.GridPosition.x, (int)EndTileSettings.GridPosition.y] = endNode;

        #endregion

        //Loop over grid positions & instantiate nodes
        float currentNodeX = GridMinBounds[0] - tileCentreX + tileLengthX;
        float currentNodeZ = GridMinBounds[1] - tileCentreZ + tileLengthZ;
        for (int i = 1; i < GridDimension - 1; i++)
        {
            for (int j = 1; j < GridDimension - 1; j++)
            {
                //Create new node and move to position
                GameObject newNode = new GameObject("Node [" + i + "," + j + "]");
                newNode.AddComponent<GridNode>();
                newNode.transform.parent = transform;
                newNode.transform.position = transform.position;
                newNode.transform.rotation = transform.rotation;
                newNode.transform.Translate(currentNodeX, 0f, currentNodeZ);

                //Add node to the board
                newNode.GetComponent<GridNode>().GridPosition = new int[] { i, j };
                Graph[i, j] = newNode;

                //Increment current tile z-position
                currentNodeZ += tileLengthZ;
            }

            //Increment current tile x-position & reset current tile z-position
            currentNodeX += tileLengthX;
            currentNodeZ = GridMinBounds[1] - tileCentreZ + tileLengthZ;
        }

        #endregion

        #region Tile Population

        //Instantiate the board
        Board = new GameObject[GridDimension, GridDimension];

        #region Start & End Tile Instantiation

        //Instantiate tiles & scale
        StartTile = Instantiate(StartTileSettings.TilePrefab, startNode.transform.position, startNode.transform.rotation, startNode.transform);
        EndTile = Instantiate(EndTileSettings.TilePrefab, endNode.transform.position, endNode.transform.rotation, endNode.transform);
        StartTile.transform.localScale = new Vector3(0.1f * tileLengthX, transform.localScale.y, 0.1f * tileLengthZ);
        EndTile.transform.localScale = new Vector3(0.1f * tileLengthX, transform.localScale.y, 0.1f * tileLengthZ);

        //Set tile variables
        StartTile.GetComponent<Tile>().TileGrid = this;
        StartTile.GetComponent<Tile>().GridPosition = new int[2] { (int)StartTileSettings.GridPosition.x, (int)StartTileSettings.GridPosition.y };
        EndTile.GetComponent<Tile>().TileGrid = this;
        EndTile.GetComponent<Tile>().GridPosition = new int[2] { (int)EndTileSettings.GridPosition.x, (int)EndTileSettings.GridPosition.y };

        //Add tiles to board
        Board[(int)StartTileSettings.GridPosition.x, (int)StartTileSettings.GridPosition.y] = StartTile;
        Board[(int)EndTileSettings.GridPosition.x, (int)EndTileSettings.GridPosition.y] = EndTile;

        //Rotate tiles
        for (int i = 0; i < (int)StartTileSettings.Rotation; i++)
            StartTile.GetComponent<Tile>().RotateTile(true);
        for (int i = 0; i < (int)EndTileSettings.Rotation; i++)
            EndTile.GetComponent<Tile>().RotateTile(true);

        #endregion

        //Loop through all tiles, retrieve prefab from level and instantiate
        for (int i = 1; i < GridDimension - 1; i++)
        {
            for (int j = 1; j < GridDimension - 1; j++)
            {
                //Get correponding node
                GameObject node = Graph[i, j];

                //Instantiate a new tile and scale to tile size
                GameObject tile = Instantiate(level.GetTileForGridPosition(i, j), node.transform.position, node.transform.rotation, node.transform);
                tile.transform.localScale = new Vector3(0.1f * tileLengthX, transform.localScale.y, 0.1f * tileLengthZ);

                //Add tile to the board
                tile.GetComponent<Tile>().TileGrid = this;
                tile.GetComponent<Tile>().GridPosition = new int[2] { i, j };
                Board[i, j] = tile;

                //Apply random rotation to tile
                for (int r = 0; r < Random.Range(0, 4); r++)
                    tile.GetComponent<Tile>().RotateTile(true);
            }
        }

        //Populate list of Hazard Tiles
        HazardTiles.AddRange(GameObject.FindGameObjectsWithTag("HazardCable").Select(t => t.GetComponent<Tile>()));

        #endregion
    }

    //Destroys all board tiles
    public void DestroyBoard()
    {
        //Path-clearing
        StartTile = null;
        EndTile = null;
        HazardTiles = new List<Tile>();
        OpenList = new List<GridNode>();
        ClosedList = new List<GridNode>();
        GeneratedPath = new List<GridNode>();
        DetectedPath = new List<GridNode>();

        //Loop over all Graph elements, and destroy each object found
        for (int i = 0; i < Graph.GetLength(0); i++)
            for (int j = 0; j < Graph.GetLength(1); j++)
                if (Graph[i, j])
                    Destroy(Graph[i, j]);

        //Loop over all Board elements, and destroy each object found
        for (int i = 0; i < Board.GetLength(0); i++)
            for (int j = 0; j < Board.GetLength(1); j++)
                if (Board[i, j])
                    Destroy(Board[i, j]);
    }

    //Detect whether or not the circuit contains a vaild path
    public bool DetectCompleteCircuit()
    {
        //Reconstruct graph from tile connections
        ResetNodes();
        WriteTileConnectionsToNodes();

        //Perform A* search
        DetectedPath = AStarSearch(GetNodeFromTile(StartTile.GetComponent<Tile>()), GetNodeFromTile(EndTile.GetComponent<Tile>()));

        if (DetectedPath == null) return false;
        if (DetectedPath.Count == 0) return false;
        if (DetectHazardConnections()) return false;
        return true;
    }

    //Search for a path from the start to any of the hazard tiles
    private bool DetectHazardConnections()
    {
        //Initialise lists
        List<Tile> visited = new List<Tile>();
        List<Tile> frontier = new List<Tile>();
        frontier.Add(StartTile.GetComponent<Tile>());

        //Repeat until frontier exhausted
        while (frontier.Count > 0)
        {
            //Get next tile and it's connected tiles
            Tile tile = frontier[0];
            List<Tile> connectedTiles = tile.GetConnectedTiles();

            //If connected to any hazard tile, return true (circuit failed)
            if (connectedTiles.Any(ct => HazardTiles.Contains(ct)))
                return true;

            //Add connected tiles to frontier
            frontier.AddRange(connectedTiles.Where(ct => !frontier.Contains(ct) && !visited.Contains(ct)));

            //Remove tile from frontier
            visited.Add(tile);
            frontier.Remove(tile);
        }

        //If no hazard tiles found, return false (circuit success)
        return false;
    }

    //Checks if the game has finished animating all objects
    public bool FinishedAnimating()
    {
        //Loop through all tiles and check if animating
        for (int i = 0; i < GridDimension; i++)
            for (int j = 0; j < GridDimension; j++)
                if (Board[i, j] != null)
                    if (Board[i, j].GetComponent<Tile>().Animating) return false;

        //If none are animating, return true
        return true;
    }

    //Resets the connection data of all nodes
    private void ResetNodes()
    {
        //Loop over all nodes & reset each
        for (int i = 0; i < GridDimension; i++)
            for (int j = 0; j < GridDimension; j++)
                if (Graph[i, j] != null)
                    Graph[i, j].GetComponent<GridNode>().ResetNode();
    }

    //Writes the connection data of the tiles to the nodes to create a new graph
    private void WriteTileConnectionsToNodes()
    {
        //Grid geometry calculations
        float gridLengthX = GridMaxBounds[0] - GridMinBounds[0];
        float gridLengthZ = GridMaxBounds[1] - GridMinBounds[1];

        //Tile geometry calculations
        float tileLengthX = gridLengthX / GridDimension;
        float tileLengthZ = gridLengthZ / GridDimension;

        //Loop over all tiles
        for (int i = 0; i < GridDimension; i++)
        {
            for (int j = 0; j < GridDimension; j++)
            {
                //Check if tile exists
                if (Board[i, j] != null)
                {
                    //Get the tile & node
                    Tile tile = Board[i, j].GetComponent<Tile>();
                    GridNode node = Graph[i, j].GetComponent<GridNode>();

                    //Tile above
                    if (tile.ConnectedSides[0])
                    {
                        node.connectedNodes[0] = Graph[i, j + 1].GetComponent<GridNode>();
                        node.connectionCosts[0] = tileLengthZ;
                    }

                    //Tile right
                    if (tile.ConnectedSides[1])
                    {
                        node.connectedNodes[1] = Graph[i + 1, j].GetComponent<GridNode>();
                        node.connectionCosts[1] = tileLengthX;
                    }

                    //Tile below
                    if (tile.ConnectedSides[2])
                    {
                        node.connectedNodes[2] = Graph[i, j - 1].GetComponent<GridNode>();
                        node.connectionCosts[2] = tileLengthZ;
                    }

                    //Tile left
                    if (tile.ConnectedSides[3])
                    {
                        node.connectedNodes[3] = Graph[i - 1, j].GetComponent<GridNode>();
                        node.connectionCosts[3] = tileLengthX;
                    }
                }
            }
        }
    }

    //Draw the generated path
    private void DebugDrawGeneratedPath()
    {
        for (int i = 0; i < GeneratedPath.Count - 1; i++)
        {
            Vector3 posStart = GeneratedPath[i].transform.position;
            posStart.x -= 0.1f;
            posStart.z -= 0.1f;
            Vector3 posEnd = GeneratedPath[i + 1].transform.position;
            posEnd.x -= 0.1f;
            posEnd.z -= 0.1f;

            Debug.DrawLine(posStart, posEnd, GeneratedPathColor);
        }
    }

    //Draw the detected path
    private void DebugDrawDetectedPath()
    {
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
    //Partially constructed from: https://www.codeproject.com/Articles/1221034/Pathfinding-Algorithms-in-Csharp (16/06/2023)
    private List<GridNode> AStarSearch(GridNode start, GridNode end)
    {
        start.MinCostToStart = 0f;
        
        //Clear Lists/Dictionaries at start
        OpenList.Clear();
        ClosedList.Clear();
        OpenList.Add(start);

        //While there are open nodes remaining
        while (OpenList.Count > 0)
        {
            //Sort OpenList by best candidate and open that node
            OpenList = OpenList.OrderBy(n => n.MinCostToStart + n.StraightLineDistanceToEnd).ToList();
            GridNode currentNode = OpenList.First();
            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);

            //Found the end, reconstruct entire path and return
            if (currentNode == end)
            {
                List<GridNode> foundPath = new List<GridNode>() { end };
                BuildShortestPath(foundPath, end);
                foundPath.Reverse();
                return foundPath;
            }

            //Look for neighbouring nodes
            for (int i = 0; i < 4; i++)
            {
                //Get the i-th neighbour (if exists)
                GridNode neighbourNode = currentNode.connectedNodes[i];
                if (neighbourNode == null) continue;

                //Skip if neighbour has already been visited
                if (ClosedList.Contains(neighbourNode))
                    continue;

                //Check if this connection to the neighbour node is shorter than it's existing record & update it
                if (neighbourNode.MinCostToStart  < 0f || currentNode.MinCostToStart + currentNode.connectionCosts[i] < neighbourNode.MinCostToStart)
                {
                    //Update the neighbour node's records
                    neighbourNode.MinCostToStart = currentNode.MinCostToStart + neighbourNode.connectionCosts[i];
                    neighbourNode.NearestToStart = currentNode;

                    //Add neighbour node to OpenList
                    if (!OpenList.Contains(neighbourNode))
                        OpenList.Add(neighbourNode);
                }
            }
        }

        //Return null if not path exists
        return null;
    }

    //Recursive method to construct the shortest path from the given node to the start
    private void BuildShortestPath(List<GridNode> list, GridNode node)
    {
        if (node.NearestToStart == null)
            return;

        list.Add(node.NearestToStart);
        BuildShortestPath(list, node.NearestToStart);
    }

    //Check whether a tile exists at the input grid position
    public bool TileExists(int[] gridPosition)
    {
        if (gridPosition[0] < 0 || gridPosition[0] >= Board.GetLength(0) || gridPosition[1] < 0 || gridPosition[1] >= Board.GetLength(1))
            return false;
        return (Board[gridPosition[0], gridPosition[1]] != null);
    }

    //Retrieve the tile associated with a given node
    private Tile GetTileFromNode(GridNode node)
    {
        return Board[node.GridPosition[0], node.GridPosition[1]].GetComponent<Tile>();
    }

    //Retrieve the node associated with a given tile
    private GridNode GetNodeFromTile(Tile tile)
    {
        return Graph[tile.GridPosition[0], tile.GridPosition[1]].GetComponent<GridNode>();
    }
}

[System.Serializable]
public class TileSetting
{
    public enum TileRotation
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    public GameObject TilePrefab;
    public Vector2 GridPosition;
    public TileRotation Rotation;
}
