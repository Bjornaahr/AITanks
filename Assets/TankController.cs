using Complete;
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
    GameObject[] Tanks;
    public Color Friendly;

    Vector3 knownEnemyPosition;                     // Last known position of tank
    [SerializeField] Transform barrelDirection;     // Direction of barrel
    [SerializeField] TankShooting shoot;            // Shooting script

    void Start() 
    {
        graph = GameObject.FindGameObjectWithTag("Graph").GetComponent<Graph>();
        pathFinder = GetComponent<PathFinder>();
        Tanks = GameObject.FindGameObjectsWithTag("gray");
        shoot = gameObject.GetComponent<TankShooting>();
        
        path = pathFinder.FindPath(graph.Nodes[100], graph.Nodes[500], graph);
        foreach(var Tank in Tanks) {
            if (Tank.GetComponent<TankController>().Friendly != this.Friendly) {
                EnemyTank = Tank;
            }
        }
        
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
        bool tankVisible = !Physics.Linecast(transform.position, new Vector3(EnemyTank.transform.position.x, 0, EnemyTank.transform.position.z), out hit, ~(1 << gameObject.layer));

        Debug.DrawRay(barrelDirection.position, barrelDirection.forward * 100, Color.blue);    // DEBUG

        if (tankVisible || hit.collider.transform == EnemyTank.transform)
        {
            knownEnemyPosition = new Vector3(EnemyTank.transform.position.x, 0, EnemyTank.transform.position.z);
            // check if barrel points towards target
            RaycastHit barrelHit;
            
            // Only shoots if the enemy is in sights from the turret
            if (Physics.Linecast(transform.position, transform.position + barrelDirection.forward * 100, out barrelHit, (1 << gameObject.layer))) //|| barrelHit.collider.transform == EnemyTank.transform)
            {
                shoot.Fire();
            }
        }

        // Points the turrent towards the last known position of the tank as long as one such position is known
        if (knownEnemyPosition != null)
        {
            PointTurretAtTarget();
        }
    }

    void PointTurretAtTarget()
    {
        var targetDistance = knownEnemyPosition - transform.position;
        var targetDirection = targetDistance;
        targetDirection.y = 0;
        targetDistance.Normalize();
        
        // turretBase.transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        
        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(turretBase.transform.forward, targetDirection, rotationValue, 0.0f);
        
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

    public void getColor(Color c)
    {
        Friendly = c;
    }
}
