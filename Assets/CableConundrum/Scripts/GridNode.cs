using UnityEngine;

public class GridNode : MonoBehaviour
{
    public GridNode[] connectedNodes = new GridNode[] { null, null, null, null };
    public float[] connectionCosts = new float[] { -1f, -1f, -1f, -1f };
    public float StraightLineDistanceToEnd;
    public float MinCostToStart = -1f;
    public GridNode NearestToStart = null;
    public int[] GridPosition;

    //Gets the straight-line distance from this node to the specified node
    public float StraightLineDistanceTo(GridNode node)
    {
        return Vector3.Distance(gameObject.transform.position, node.gameObject.transform.position);
    }

    //Reset the variables of this node
    public void ResetNode()
    {
        connectedNodes = new GridNode[] { null, null, null, null };
        connectionCosts = new float[] { -1f, -1f, -1f, -1f };
        MinCostToStart = -1f;
        NearestToStart = null;
    }
}
