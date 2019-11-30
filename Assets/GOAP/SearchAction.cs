using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAction : AbstractGOAPAction
{

    private bool enemySeen = false;
    GraphNode targetNode = null;

    //Approx size of map
    float maxX = 41, minX = -41, maxZ = 20, minZ = -20;


    public SearchAction()
    {
       addPrecondition("canSeeEnemy", false);
       addPrecondition("hasEnoughHealth", true);
       addEffect("damageTank", true);
       targetNode = null;
    }

    public override void reset()
    {
        targetNode = null;
        enemySeen = false;
    }

    //Check if we have last known position, if not take it random walk
    public override bool checkPrecondtion(GameObject agent)
    {
        return true;
    }

    public override bool isDone()
    {
        return enemySeen;
    }

    //Walk towards node, choose node until enemy is seen
    public override bool perform(GameObject agent)
    {
        Tank currentA = agent.GetComponent<Tank>();


        if (targetNode == null || currentA.findNodeCloseToPosition(transform.position) == targetNode)
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 targetPos = new Vector3(randomX, 0, randomZ);
            targetNode = currentA.CalculatePath(targetPos);
            Debug.Log("Targetnode Pis: " + targetNode);
            Debug.Log(targetPos);
        }
        return true;

    }

    public override bool requiresInRange()
    {
        return false;
    }

  

}
