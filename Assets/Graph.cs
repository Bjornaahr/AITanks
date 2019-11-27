using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Graph : MonoBehaviour
{
    [System.Serializable]
    public class Edge
    {
        public GraphNode StartNode;

        public GraphNode EndNode;

        [Range(0f, 10f)]
        public int Weight;
    }

    [SerializeField]
    private GameObject floor;
    [SerializeField]
    private GameObject node;

    public List<GraphNode> Nodes;
    public List<Edge> Edges; 

    // Start is called before the first frame update
    void Start()
    {

        Vector3[] floorVerticies = floor.GetComponent<MeshFilter>().sharedMesh.vertices;

        int iL = ((int)floorVerticies[0].x * (int)floor.transform.localScale.x - (int)floorVerticies[120].x * (int)floor.transform.localScale.x) / 3 + 1;
        int jL = ((int)floorVerticies[0].z * (int)floor.transform.localScale.z - (int)floorVerticies[120].z * (int)floor.transform.localScale.z) / 3 + 1;
        

        int[,] nodeList = new int[iL, jL];

        int i = 0, j = 0;

        int ID = 0;
        
        for (float x = (int)floorVerticies[120].x * (int)floor.transform.localScale.x; x < (int)floorVerticies[0].x * (int)floor.transform.localScale.x; x += 3)
        {
            j = 0;
            for (float y = (int)floorVerticies[120].z * (int)floor.transform.localScale.z; y < (int)floorVerticies[0].z * (int)floor.transform.localScale.z; y += 3)
            {
                GameObject newNode = Instantiate(node) as GameObject;
                newNode.name = ID.ToString();
                newNode.transform.parent = gameObject.transform;
                newNode.transform.position = new Vector3(x, 1, y);

                if (!Physics.CheckSphere(newNode.transform.position, 0.9f))
                {
                    newNode.transform.position = new Vector3(x, 0, y);
                    Nodes.Add(newNode.GetComponent<GraphNode>());

                    nodeList[i, j] = ID;
                    ID++;
                }
                else
                {
                    nodeList[i, j] = -1;
                    Destroy(newNode);
                }
                j++;
            }
            i++;
        }
        print(i + " " + iL);
        print(j + " " + jL);
        for (i = 0; i < iL; i++)
        {
            for (j = 0; j < jL; j++)
            {

                ID = nodeList[i, j];
                //Is there a node in this spot?
                if (ID != -1)
                {
                    if (i == 0)
                    {
                        GraphNode thisNode = Nodes[ID];
                        int adjNode = nodeList[i + 1, j];
                        if (adjNode != -1)
                        {
                            GraphNode adjacentNode = Nodes[adjNode];
                            thisNode.Adjacent.Add(adjacentNode);
                        }
                    }

                    else if (i == iL - 1)
                    {
                        GraphNode thisNode = Nodes[ID];
                        int adjNode = nodeList[i - 1, j];
                        if (adjNode != -1)
                        {
                            GraphNode adjacentNode = Nodes[adjNode];
                            thisNode.Adjacent.Add(adjacentNode);
                        }
                    }

                    else
                    {
                        GraphNode thisNode = Nodes[ID];
                        int adjNode = nodeList[i + 1, j];
                        int adjNode2 = nodeList[i - 1, j];
                        if (adjNode != -1)
                        {
                            GraphNode adjacentNode = Nodes[adjNode];
                            thisNode.Adjacent.Add(adjacentNode);
                        }
                        if (adjNode2 != -1)
                        {
                            GraphNode adjacentNode = Nodes[adjNode2];
                            thisNode.Adjacent.Add(adjacentNode);
                        }
                    }

                    if (j == 0)
                    {
                        GraphNode thisNode = Nodes[ID];
                        int adjNode = nodeList[i, j + 1];
                        if (adjNode != -1)
                        {
                            GraphNode adjacentNode = Nodes[adjNode];
                            thisNode.Adjacent.Add(adjacentNode);
                        }
                    }

                    else if (j == jL - 1)
                    {
                        GraphNode thisNode = Nodes[ID];
                        int adjNode = nodeList[i, j - 1];
                        if (adjNode != -1)
                        {
                            GraphNode adjacentNode = Nodes[adjNode];
                            thisNode.Adjacent.Add(adjacentNode);
                        }
                    }

                    else
                    {
                        GraphNode thisNode = Nodes[ID];
                        int adjNode = nodeList[i, j + 1];
                        int adjNode2 = nodeList[i, j - 1];
                        if (adjNode != -1)
                        {
                            GraphNode adjacentNode = Nodes[adjNode];
                            thisNode.Adjacent.Add(adjacentNode);
                        }
                        if (adjNode2 != -1)
                        {
                            GraphNode adjacentNode = Nodes[adjNode2];
                            thisNode.Adjacent.Add(adjacentNode);
                        }
                    }
                }
            }
        }
    }

    private void Reset()
    {
        Nodes = GetComponentsInChildren<GraphNode>(true).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
