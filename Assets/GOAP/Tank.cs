using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour, IGoap
{

    [SerializeField] Rigidbody tankBody;
    [SerializeField] float minDist = 0.2f;
    [Range(0f, 10f)]
    [SerializeField] float speedValue = 10.0f;

    [SerializeField] GraphNode currentTargetNode;  // Current goal
    [SerializeField] GraphNode endTargetNode;       // The end node goal, will be used later

    [SerializeField]
    private GameObject tankBase;

    [SerializeField]
    private GameObject turretBase;
    Graph graph;
    IList<GraphNode> path;
    PathFinder pathFinder;
    GameObject EnemyTank;
    AbstractGOAPAction abstractGOAPAction;

    void Start()
    {
        graph = GameObject.FindGameObjectWithTag("Graph").GetComponent<Graph>();
        pathFinder = GetComponent<PathFinder>();
        EnemyTank = GameObject.FindGameObjectWithTag("green");

        //path = pathFinder.FindPath(graph.Nodes[100], graph.Nodes[500], graph);
        calculatePath(abstractGOAPAction);
        Debug.Log(path.Count);
    }

    void Update()
    {
        moveAgent(abstractGOAPAction);
    }

    public void actionsFinished()
    {
        throw new System.NotImplementedException();
    }

    public HashSet<KeyValuePair<string, object>> createGoalState()
    {
        throw new System.NotImplementedException();
    }

    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();
        goal.Add(new KeyValuePair<string, object> ("damageTank", true));
        goal.Add(new KeyValuePair<string, object> ("stayAlive", true));
        return goal;
    }

   

    public void planAborted(AbstractGOAPAction aborter)
    {
        throw new System.NotImplementedException();
    }

    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
        throw new System.NotImplementedException();
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<AbstractGOAPAction> actions)
    {
        throw new System.NotImplementedException();
    }


    public bool moveAgent(AbstractGOAPAction nextAction)
    {


        if (path != null && path.Count > 0)
        {
            currentTargetNode = path[0];
            if (Vector3.SqrMagnitude(transform.position - currentTargetNode.transform.position) <= 1.0f)
            {
                path.RemoveAt(0);
            }

        }
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentTargetNode.transform.position.x, transform.position.y,
            currentTargetNode.transform.position.z), Time.deltaTime * 2);
        
        // Gets vector direction of movement, normalize and multiply it with speed after setting the y direction to 
        Vector3 posValue = currentTargetNode.transform.position - transform.position;

        // Move towards goal
        if (currentTargetNode != null && posValue.magnitude > minDist)
        {
            posValue.y = 0;

            Vector3 newDir = Vector3.RotateTowards(transform.forward, posValue, 0.1f, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        // If a major goal is chosen, the tank is no longer moving andthe tank have reached its major goal
        else if (endTargetNode != null && (endTargetNode.transform.position - transform.position).magnitude < minDist)
        {
            Debug.Log("Goal " + currentTargetNode.name + " reached");
            currentTargetNode = endTargetNode = null;
        }
        RaycastHit hit;
        if (!Physics.Linecast(transform.position, EnemyTank.transform.position, out hit, ~(1 << gameObject.layer)) || hit.collider.transform == EnemyTank.transform)
        {
            PointTurretAtTarget();
        } return true;
    }


    void PointTurretAtTarget()
    {
        var targetDistance = EnemyTank.gameObject.transform.position - transform.position;
        var targetDirection = targetDistance;

        targetDistance.Normalize();

        turretBase.transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);
    }


    public void calculatePath(AbstractGOAPAction nextAction)
    {
        path = pathFinder.FindPath(findNodeCloseToPosition(transform.position), findNodeCloseToPosition(new Vector3(41, 0, 17)), graph);
    }

    GraphNode findNodeCloseToPosition(Vector3 wantedPosition)
    {
        GraphNode nearNode = null;
        float closestDist = Mathf.Infinity;
        
        foreach(GraphNode potenitalNode in graph.Nodes)
        {
            Debug.Log(potenitalNode.transform);
            Vector3 directionToTarget = potenitalNode.transform.position - wantedPosition;
            float sqrToTarget = directionToTarget.sqrMagnitude;
            if (sqrToTarget < closestDist)
            {
                closestDist = sqrToTarget;
                nearNode = potenitalNode;
            }
        }
        Debug.Log(nearNode.transform.position);
        return nearNode;

    }

}
