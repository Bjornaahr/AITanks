using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [SerializeField] Rigidbody tankBody;
    [SerializeField] float minDist = 0.2f;
    [SerializeField] float speedValue = 10.0f;

    [SerializeField] GraphNode currentGoal;  // Current goal
    [SerializeField] GraphNode majorGoal;    // The end node goal, will be used later

    // Start is called before the first frame update

    // Update is called once per frame
    void FixedUpdate()
    {
        // Gets vector direction of movement, normalize and multiply it with speed after setting the y direction to 0
        Vector3 posValue = (currentGoal.transform.position - transform.position);

        // Move towards goal
        if (currentGoal != null && posValue.magnitude > minDist)
        {
            posValue.y = 0;
            tankBody.velocity = posValue.normalized * speedValue;
            
            Vector3 newDir = Vector3.RotateTowards(transform.forward, posValue, 0.1f, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        // If a major goal is chosen, the tank is no longer moving andthe tank have reached its major goal
        else if (majorGoal != null && (majorGoal.transform.position - transform.position).magnitude < minDist)
        {
            Debug.Log("Goal " + currentGoal.name + " reached");
            currentGoal = majorGoal = null;
        }
    }
}
