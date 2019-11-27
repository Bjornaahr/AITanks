using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
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
    

    // Update is called once per frame
    void FixedUpdate()
    {
        // Gets vector direction of movement, normalize and multiply it with speed after setting the y direction to 0
        Vector3 posValue = /*currentTargetNode.transform.position - transform.position;*/ new Vector3(0, 0, 0);

        // Move towards goal
        if (currentTargetNode != null && posValue.magnitude > minDist)
        {
            posValue.y = 0;
            tankBody.velocity = posValue.normalized * speedValue;

            Vector3 newDir = Vector3.RotateTowards(transform.forward, posValue, 0.1f, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        // If a major goal is chosen, the tank is no longer moving andthe tank have reached its major goal
        else if (endTargetNode != null && (endTargetNode.transform.position - transform.position).magnitude < minDist)
        {
            Debug.Log("Goal " + currentTargetNode.name + " reached");
            currentTargetNode = endTargetNode = null;
        }

        if (currentTargetNode != null)
        {
            PointTurretAtTarget();

        }
    }

    void PointTurretAtTarget()
    {
        var targetDistance = currentTargetNode.gameObject.transform.position - transform.position;
        var targetDirection = targetDistance;

        targetDistance.Normalize();

        turretBase.transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);
    }

    void MoveForward()
    {
        float deltaTime = Time.deltaTime;

        var newPosition = gameObject.transform.position + gameObject.transform.forward * speedValue * deltaTime;

        gameObject.transform.position = newPosition;
        tankBody.MovePosition(newPosition);
    }
}
