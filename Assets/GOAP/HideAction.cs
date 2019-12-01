using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//When the tank need to hide
public class HideAction : AbstractGOAPAction
{
    GraphNode targetNode = null;
    bool nodeReached;

    //Approx size of map
    float maxX = 41, minX = -41, maxZ = 20, minZ = -20;

    //Set effects and precondtions for executing action
    public HideAction()
    {
        addPrecondition("hasEnoughHealth", false);
        addEffect("stayAlive", true);
    }

    //Check if there is any procedural conditions that needs to be checked
    public override bool checkPrecondtion(GameObject agent)
    {
        Tank tank = GetComponent<Tank>();
        return tank.healthOver10;
    }

    //Resetting the imporant variables needed to execute the action
    public override void reset()
    {
        nodeReached = false;
        targetNode = null;
        Debug.Log("Reset Hiding");
    }

    //The action is done when node is reached (Never happens since we don't want it to finsih hiding ever)
    public override bool isDone()
    {
        return nodeReached;
    }

    public override bool perform(GameObject agent)
    {

        Tank currentA = agent.GetComponent<Tank>();

        //Fidning a hiding spot, if we don't have one or if we can see the enemy tank
        if (targetNode == null || currentA.canSeeEnemy)
        {
            Vector3 targetPos = transform.position;
            bool isInView = true;
            RaycastHit hit;

            //Super not good but works
            while (isInView)
            {
                float randomX = Random.Range(minX, maxX);
                float randomZ = Random.Range(minZ, maxZ);
                targetPos = new Vector3(randomX, 0, randomZ);
                isInView = !Physics.Linecast(targetPos, currentA.EnemyTank.transform.position, out hit, ~(1 << gameObject.layer));
            }

            Vector3 targetDir = targetPos - transform.position;
            Vector3 enemyDir = currentA.EnemyTank.transform.position - transform.position;

            //Check if the target node is towards the enemy tank
            if (Vector3.Dot(enemyDir, targetDir) < 0)
            {
                targetNode = currentA.CalculatePath(targetPos);
            }

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
