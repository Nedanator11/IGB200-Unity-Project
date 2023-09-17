using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public GameObject[,] Graph;
    public GameObject[,] Board;

    //Path-Finding
    private GameObject StartTile;
    private GameObject EndTile;
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
        float tileLengthX = gridLengthX / GridDimension;
        float tileLengthZ = gridLengthZ / GridDimension;
        float tileCentreX = tileLengthX / 2f;
        float tileCentreZ = tileLengthZ / 2f;

        #region Node, Graph & Path Generation

        #region Start & End Node Instantiation

        //World position calculation
        float startNodeX = GridMinBounds[0] - tileCentreX;
        float startNodeZ = GridMinBounds[1] - tileCentreZ + (tileLengthZ * StartTilePosZ);
        float endNodeX = GridMaxBounds[0] + tileCentreX;
        float endNodeZ = GridMinBounds[1] - tileCentreZ + (tileLengthZ * EndTilePosZ);

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
        startNode.GetComponent<GridNode>().GridPosition = new int[] { 0, StartTilePosZ };
        Graph[0, StartTilePosZ] = startNode;
        endNode.GetComponent<GridNode>().GridPosition = new int[] { GridDimension + 1, EndTilePosZ };
        Graph[GridDimension + 1, EndTilePosZ] = endNode;

        #endregion

        //Loop over grid positions & instantiate nodes
        float currentNodeX = GridMinBounds[0] + tileCentreX;
        float currentNodeZ = GridMinBounds[1] + tileCentreZ;
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
            currentNodeZ = GridMinBounds[1] + tileCentreZ;
        }

        //Loop over all nodes and populate node connections & costs
        startNode.GetComponent<GridNode>().connectedNodes[1] = Graph[1, StartTilePosZ].GetComponent<GridNode>();
        endNode.GetComponent<GridNode>().connectedNodes[3] = Graph[GridDimension, EndTilePosZ].GetComponent<GridNode>();
        for (int i = 1; i < GridDimension + 1; i++)
        {
            for (int j = 1; j < GridDimension + 1; j++)
            {
                GridNode node = Graph[i, j].GetComponent<GridNode>();

                //Node above
                if (j != GridDimension)
                {
                    //Connect to node above
                    node.connectedNodes[0] = Graph[i, j + 1].GetComponent<GridNode>();

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
                if (i != GridDimension || j == EndTilePosZ)
                {
                    //Connect to node right
                    node.connectedNodes[1] = Graph[i + 1, j].GetComponent<GridNode>();

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
                if (j != 1)
                {
                    //Connect to node below
                    node.connectedNodes[2] = Graph[i, j - 1].GetComponent<GridNode>();

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
                if (i != 1 || j == StartTilePosZ)
                {
                    //Connect to node left
                    node.connectedNodes[3] = Graph[i - 1, j].GetComponent<GridNode>();

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

        //Generate the least-cost path from start to end
        GeneratedPath = AStarSearch(startNode.GetComponent<GridNode>(), endNode.GetComponent<GridNode>());

        #endregion

        #region Tile Population

        //Instantiate the board
        Board = new GameObject[GridDimension + 2, GridDimension + 2];

        #region Start & End Tile Instantiation

        //Instantiate tiles & scale
        StartTile = Instantiate(StartTilePrefab, startNode.transform.position, startNode.transform.rotation, startNode.transform);
        EndTile = Instantiate(EndTilePrefab, endNode.transform.position, endNode.transform.rotation, endNode.transform);
        StartTile.transform.localScale = new Vector3(0.1f * tileLengthX, transform.localScale.y, 0.1f * tileLengthZ);
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

        //Loop over all generated path nodes (excluding start & end)
        for (int i = 1; i < GeneratedPath.Count - 1; i++)
        {
            //Get the set of three sequential nodes
            GridNode nodePrev = GeneratedPath[i - 1];
            GridNode node = GeneratedPath[i];
            GridNode nodeNext = GeneratedPath[i + 1];

            //If the three sequenced nodes lie on the same x or same y, the path is straight
            GameObject tilePrefab;
            if ((nodePrev.GridPosition[0] == node.GridPosition[0] && node.GridPosition[0] == nodeNext.GridPosition[0]) ||
                (nodePrev.GridPosition[1] == node.GridPosition[1] && node.GridPosition[1] == nodeNext.GridPosition[1]))
            {
                //Choose tile prefab
                float StraightThreshold = Turn90Weight + StraightWeight;
                float TSplitThreshold = StraightThreshold + TSplitWeight;
                float CrossSplitThreshold = TSplitThreshold + CrossSplitWeight;
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
                //Choose tile prefab
                float Turn90Threshold = Turn90Weight + StraightWeight;
                float TSplitThreshold = Turn90Threshold + TSplitWeight;
                float CrossSplitThreshold = TSplitThreshold + CrossSplitWeight;
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

                //Choose tile prefab
                float Turn90Threshold = Turn90Weight;
                float StraightThreshold = Turn90Threshold + StraightWeight;
                float TSplitThreshold = StraightThreshold + TSplitWeight;
                float CrossSplitThreshold = TSplitThreshold + CrossSplitWeight;
                float rand = Random.Range(0f, CrossSplitThreshold);
                GameObject tilePrefab;
                if (rand <= Turn90Threshold)
                    tilePrefab = Turn90TilePrefab;
                else if (rand <= StraightThreshold)
                    tilePrefab = StraightTilePrefab;
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

    //Destroys all board tiles
    public void DestroyBoard()
    {
        //Path-clearing
        StartTile = null;
        EndTile = null;
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
        return true;
    }

    //Resets the connection data of all nodes
    private void ResetNodes()
    {
        //Loop over all nodes & reset each
        for (int i = 0; i < GridDimension + 2; i++)
            for (int j = 0; j < GridDimension + 2; j++)
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
        for (int i = 0; i < GridDimension + 2; i++)
        {
            for (int j = 0; j < GridDimension + 2; j++)
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
