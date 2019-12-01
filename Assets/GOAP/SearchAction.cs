using Complete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAction : AbstractGOAPAction
{

    private bool enemySeen = false;
    GraphNode targetNode = null;

    //Approx size of map
    float maxX = 41, minX = -41, maxZ = 20, minZ = -20;

    //Set effects and precondtions for executing action
    public SearchAction()
    {
       addPrecondition("hasEnoughHealth", true);
       addPrecondition("canSeeEnemy", false);
       addEffect("canSeeEnemy", true);
       addEffect("searchForTank", true);
       targetNode = null;
       enemySeen = false;
    }

    //Resetting the imporant variables needed to execute the action
    public override void reset()
    {
        Debug.Log("ResetSearch");
        targetNode = null;
        enemySeen = false;
    }

    //Check if we have last known position, if not take it random walk
    public override bool checkPrecondtion(GameObject agent)
    {

        TankHealth currentH = agent.GetComponent<TankHealth>();
        Tank currentA = agent.GetComponent<Tank>();

        if (currentH.m_CurrentHealth <= 10 || currentA.canSeeEnemy)
        {
            return false;
        }

        return true;
    }

    //Action is done when we see enemy tank
    public override bool isDone()
    {
        return enemySeen;
    }

    //Walk towards node, choose node until enemy is seen
    public override bool perform(GameObject agent)
    {
        Tank currentA = agent.GetComponent<Tank>();

        //Find a new node when we reached target node or when we don't have a target node
        if (targetNode == null || currentA.findNodeCloseToPosition(transform.position) == targetNode)
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 targetPos = new Vector3(randomX, 0, randomZ);
            targetNode = currentA.CalculatePath(targetPos);
        }


        if (currentA.canSeeEnemy)
        {
            enemySeen = true;
        }
        else enemySeen = false;

        return true;

    }

    public override bool requiresInRange()
    {
        return false;
    }

  

}
