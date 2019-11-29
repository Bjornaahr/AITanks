using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [SerializeField] Rigidbody tankBody;
    [SerializeField] float minDist = 0.2f;
    [Range(0f, 10f)]
    [SerializeField] float speedValue = 10.0f;
    [Range(0.01f, 0.2f)]
    [SerializeField] float rotationValue = 0.12f;

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

    Vector3 knownEnemyPosition;                     // Last known position of tank

    void Start() 
    {
        graph = GameObject.FindGameObjectWithTag("Graph").GetComponent<Graph>();
        pathFinder = GetComponent<PathFinder>();
        EnemyTank = GameObject.FindGameObjectWithTag("green");

        path = pathFinder.FindPath(graph.Nodes[100], graph.Nodes[500], graph);
        Debug.Log(path.Count);
        
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (path != null && path.Count > 0) {
            currentTargetNode = path[0];
            if (Vector3.SqrMagnitude(transform.position - currentTargetNode.transform.position) <= 1.0f)
            {
                path.RemoveAt(0);
            }
            
        }
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentTargetNode.transform.position.x, transform.position.y, 
            currentTargetNode.transform.position.z), Time.deltaTime * 2);
        // Gets vector direction of movement, normalize and multiply it with speed after setting the y direction to 0
        
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
        
        // Sends a raytrace to check if the enemy tank is in view
        RaycastHit hit;
        if (!Physics.Linecast(transform.position, EnemyTank.transform.position, out hit, ~(1 << gameObject.layer)) || hit.collider.transform == EnemyTank.transform)
        {
            knownEnemyPosition = EnemyTank.transform.position;
        }

        // Points the turrent towards the last known position of the tank as long as one such position is known
        if (knownEnemyPosition != null)
        {
            PointTurretAtTarget();
        }
        

        
    }

    void PointTurretAtTarget()
    {
        var targetDistance = EnemyTank.gameObject.transform.position - transform.position;
        var targetDirection = targetDistance;

        targetDistance.Normalize();

       
        // turretBase.transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        
        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(turretBase.transform.forward, targetDirection, rotationValue, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(turretBase.transform.position, newDirection, Color.blue);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        turretBase.transform.rotation = Quaternion.LookRotation(newDirection);

    }



    void MoveForward()
    {
        float deltaTime = Time.deltaTime;

        var newPosition = gameObject.transform.position + gameObject.transform.forward * speedValue * deltaTime;

        gameObject.transform.position = newPosition;
        tankBody.MovePosition(newPosition);
    }
}
