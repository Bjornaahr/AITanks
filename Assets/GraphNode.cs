using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode : MonoBehaviour
{
    [SerializeField]
    public List<GraphNode> Adjacent;

    [SerializeField]
    private string _id;

    [Range(0, 10)]
    public int ExampleInteger;

    // Required for the PathFinder.cs
    public bool walkable;
    // Properties for A* calculations
    public GraphNode parent;
    public int gcost;
    public int hcost;
    public int Fcost()
    {
        return gcost + hcost;
    }
    // End required for the PathFinder

    private void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        foreach (var node in Adjacent)
        {
            Debug.DrawLine(node.transform.position, transform.position, Color.red);
        }
    }
}
