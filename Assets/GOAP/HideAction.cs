using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAction : AbstractGOAPAction
{
    GraphNode targetNode = null;
    bool nodeReached;

    //Approx size of map
    float maxX = 41, minX = -41, maxZ = 20, minZ = -20;

    public HideAction()
    {
        addPrecondition("hasEnoughHealth", false);
        addEffect("stayAlive", true);
    }

    public override bool checkPrecondtion(GameObject agent)
    {
        return true;
    }

    public override void reset()
    {
        nodeReached = false;
        targetNode = null;
        Debug.Log("Reset Hiding");
    }

    public override bool isDone()
    {
        return nodeReached;
    }

    public override bool perform(GameObject agent)
    {

        Tank currentA = agent.GetComponent<Tank>();


        if (targetNode == null || currentA.canSeeEnemy)
        {
            Vector3 targetPos = transform.position;
            bool isInView = true;
            RaycastHit hit;

            while (isInView)
            {
                float randomX = Random.Range(minX, maxX);
                float randomZ = Random.Range(minZ, maxZ);
                targetPos = new Vector3(randomX, 0, randomZ);
                isInView = !Physics.Linecast(targetPos, currentA.EnemyTank.transform.position, out hit, ~(1 << gameObject.layer));
            }
            targetNode = currentA.CalculatePath(targetPos);
        } else if(currentA.findNodeCloseToPosition(transform.position) == targetNode)
        {
           // nodeReached = true;
        }

        return true;
    }

    public override bool requiresInRange()
    {
        return false;
    }

   
}
