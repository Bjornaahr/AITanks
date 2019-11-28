using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Graph;


/*
Requires the following changes to GraphNodes.cs

Add:

public bool walkable;
// Properties for A* calculations
public GraphNode parent;
public int gcost;
public int hcost;
public int Fcost()
{
    return gcost + hcost;
}

*/


public class PathFinder : MonoBehaviour
{
    public List<GraphNode> openSet = new List<GraphNode>();
    public HashSet<GraphNode> closedSet = new HashSet<GraphNode>();

    // TODO choose input data format from tank

    // Current: build to get a graph-object from tank with
    // a single Edge-object in the Edges (list).
    // The Edge-object should contain the tanks start position
    // as StartNode and the target as EndNode. This can of course
    // be changed, but I am really not that familiar with Unity
    // and C# yet.

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // TODO currently takes one grap-object from tank
        // with one Edge-object in list with StartNode and
        // TargetNode
        Graph analyze = gameObject.AddComponent<Graph>();


        // Add the tanks StartNode = current position
        openSet.Add(analyze.Edges[0].StartNode);

        // New nodes are rearanged until parent is
        // lower than new. Then compare with children and swop to get
        // the lowest left child. Subtract 1 divide by to (integer division)
        // always (n-1)/2. So you can calculate left child and the right
        // class by a function. 
        while (openSet.Count > 0)
        {
            // Set the currentNode to first node in openSet
            GraphNode currentNode = openSet[0];

            // Loop through via neighbour-list
            // TODO if access to lines of sight and not only distance
            // weights are available we could implement Theta* variant
            // of the A* algorithm.
            // NB! This code does not take into account weights on edges
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].Fcost() < currentNode.Fcost() || openSet[i].Fcost() == currentNode.Fcost() && openSet[i].hcost < currentNode.hcost)
                {
                    currentNode = openSet[i];
                }
            }
            // Move currentNode to closedSet (as visited)
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // We have reached the target!
            // TODO please help me to check if it is possible to reference
            // to graph-object like this
            if (currentNode == analyze.Edges[0].EndNode)
            {
                // TODO find the best way to return the node-list to tank
                // I do not know if this use of graph-object data will work
                // or if I have to use other strategies. I tried to retract
                // the path directly in to the graph-objects graphNodes list
                // if this works the graph can be send back to the tank.
                // You guys have to help me choosing the best way to do this!
                ReversePath(analyze.Edges[0].StartNode, analyze.Edges[0].EndNode);
                return;
            }

            // Loop through the node mesh via all nodes adjectents
            foreach (GraphNode neighbour in currentNode.Adjacent)
            {
                // TODO please inform me if I should choose another approach for
                // nodes that are not walkable!
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }
                int newMovementCostToNeighbour = currentNode.gcost + Costs(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gcost || !openSet.Contains(neighbour))
                {
                    neighbour.gcost = newMovementCostToNeighbour;
                    neighbour.hcost = Costs(neighbour, analyze.Edges[0].EndNode);
                    // Set parent for the neighbour to current node
                    neighbour.parent = currentNode;
                    if (!neighbour.walkable)
                    {
                        continue;
                    }
                }
            }
        }


        // TODO It would be better using the grap-objects property
        // list Nodes to contain the finished lowest cost path
        // directly.

        // Extracts the shortest path nodes by backtracing the
        // nodes through their parents and reverse the list
        // Result: a list of GraphNodes with sequential
        // shortest path from start node to target
        void ReversePath(GraphNode startNode, GraphNode endNode)
        {
            List<GraphNode> path = new List<GraphNode>();
            GraphNode currentNode = endNode;
            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();
        }


        // TODO - how should we best do the calculations? Currently we are
        // not regarding weights on the games nodes.
        int Costs(GraphNode a, GraphNode b)
        {
            /*
            // How many diagonal moves do we have to make
            // to get on the level where node b is
            // This is a preliminary model for using positions
            // to calculate the costs. Nodes are not equiped with
            // coordinates?
            {
                int distX = Mathf.Abs(a.gridx - b.gridx);
                int distY = Mathf.Abs(a.gridy - b.gridy);

                // TODO - exchange number with constants given
                // by the nodes positions
                if (distX > distY)
                {
                    return (some constant) * distY + (some constant) * distX;
                }
                return (some constant) * distX + (some constant) * (distY - distX);
            }
            */


            // Dummy return not to break the code
            return 10;
        }
    }
}

