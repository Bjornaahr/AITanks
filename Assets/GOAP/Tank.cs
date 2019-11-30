using Complete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour, IGoap
{

    [SerializeField] Rigidbody tankBody;
    [SerializeField] float minDist = 0.2f;
    [Range(0f, 10f)]
    [SerializeField] float speedValue = 10.0f;
    [Range(0.2f, 10.0f)]
    [SerializeField] float rotationValue = 1.0f;

    [SerializeField] GraphNode currentTargetNode;  // Current goal
    [SerializeField] GraphNode endTargetNode;       // The end node goal, will be used later

    [SerializeField]
    private GameObject tankBase;

    [SerializeField]
    private GameObject turretBase;
    Graph graph;
    IList<GraphNode> path;
    PathFinder pathFinder;
    public GameObject EnemyTank;
    GameObject[] Tanks;
    public Color Friendly;

    Vector3 knownEnemyPosition;                     // Last known position of tank
    [SerializeField] Transform barrelDirection;     // Direction of barrel
    [SerializeField] TankShooting shoot;            // Shooting script

    AbstractGOAPAction abstractGOAPAction;
    [SerializeField]
    bool healthOver10;
    TankHealth tankHealth;
    public bool canSeeEnemy;

    void Start()
    {
        graph = GameObject.FindGameObjectWithTag("Graph").GetComponent<Graph>();
        pathFinder = GetComponent<PathFinder>();
        Tanks = GameObject.FindGameObjectsWithTag("gray");
        shoot = gameObject.GetComponent<TankShooting>();
        tankHealth = GetComponent<TankHealth>();
        healthOver10 = true;
       // path = pathFinder.FindPath(graph.Nodes[198], graph.Nodes[212], graph);

        
       foreach (var Tank in Tanks)
       {
            if (Tank.GetComponent<Tank>().Friendly != this.Friendly)
            {
                EnemyTank = Tank;
            }
       }

    }

    void FixedUpdate()
    {
        moveAgent(abstractGOAPAction);

        if (tankHealth.m_CurrentHealth <= 10)
        {
            healthOver10 = false;
        }

    }

    public void actionsFinished()
    {
        Debug.Log("Action Done");
    }

    public HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();
        goal.Add(new KeyValuePair<string, object>("damageTank", true));
        goal.Add(new KeyValuePair<string, object>("stayAlive", true));
        return goal;
    }

    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();
        worldData.Add(new KeyValuePair<string, object>("canSeeEnemy", canSeeEnemy)); 
        worldData.Add(new KeyValuePair<string, object>("hasEnoughHealth", healthOver10));
        return worldData;
    }

   

    public void planAborted(AbstractGOAPAction aborter)
    {
        throw new System.NotImplementedException();
    }

    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
        foreach (var n in failedGoal)
        {
            Debug.LogWarning(n);
        }
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<AbstractGOAPAction> actions)
    {
        foreach(var n in actions)
        Debug.Log(n);
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
        if (currentTargetNode != null)
        {
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

        }
        // Sends a raytrace to check if the enemy tank is in view
        RaycastHit hit;
        bool tankVisible = !Physics.Linecast(transform.position, EnemyTank.transform.position, out hit, ~(1 << gameObject.layer));

        canSeeEnemy = tankVisible;
        Debug.DrawLine(transform.position, EnemyTank.transform.position, Color.green);
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
        return true;
    }


    void PointTurretAtTarget()
    {
        var targetDistance = knownEnemyPosition - transform.position;
        var targetDirection = targetDistance;
        targetDirection.y = 0;
        targetDistance.Normalize();

        // turretBase.transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(turretBase.transform.forward, targetDirection, rotationValue * Time.deltaTime, 0.0f);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        turretBase.transform.rotation = Quaternion.LookRotation(newDirection);
    }


    public GraphNode CalculatePath(Vector3 target)
    {
        GraphNode targetNode = findNodeCloseToPosition(target);
        GraphNode posNode = findNodeCloseToPosition(transform.position);

       
        path = pathFinder.FindPath(posNode, targetNode, graph);
        return targetNode;
    }

    public GraphNode findNodeCloseToPosition(Vector3 wantedPosition)
    {
        GraphNode nearNode = null;
        float closestDist = Mathf.Infinity;
        
        foreach(GraphNode potenitalNode in graph.Nodes)
        {
            Vector3 directionToTarget = potenitalNode.transform.position - wantedPosition;
            float sqrToTarget = directionToTarget.sqrMagnitude;
            if (sqrToTarget < closestDist)
            {
                closestDist = sqrToTarget;
                nearNode = potenitalNode;
            } 
        }

        return nearNode;

    }


    public void getColor(Color c)
    {
        Friendly = c;
    }

}
