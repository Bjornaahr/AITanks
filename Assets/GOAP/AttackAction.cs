using Complete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : AbstractGOAPAction
{
    int segments = 50;
    float xradius = 5;
    float yradius = 5;
    LineRenderer line;
    bool reachedNode;
    GraphNode targetNode = null;
    bool canSee;
    int searchCount;

    //Set effects and precondtions for executing action
    public AttackAction()
    {
        addPrecondition("hasEnoughHealth", true);
        addPrecondition("canSeeEnemy", true);
        addEffect("damageTank", true);
        canSee = true;
    }

    //Check if there is any procedural conditions that needs to be checked
    public override bool checkPrecondtion(GameObject agent)
    {
        return true;
    }


    //Resetting the imporant variables needed to execute the action
    public override void reset()
    {
        reachedNode = false;
        targetNode = null;
        Debug.Log("ResetAttack");

    }

    //Action is done when we reach the node we wanted to
    public override bool isDone()
    {
        Debug.Log("Done");
        return reachedNode;
    }

    public override bool perform(GameObject agent)
    {
        Tank currentA = agent.GetComponent<Tank>();
        AgentGOAP currentGOAP = agent.GetComponent<AgentGOAP>();

        Debug.Log("Attack");

        //Find a node if we don't have one and get the path to it
        if (targetNode == null)
        {
            //Finding a node in circle around the enemy tank
            var radius = 3;
            var vector2 = Random.insideUnitCircle.normalized * radius;
            var targetPos = currentA.EnemyTank.transform.position + new Vector3(vector2.x, 0, vector2.y);
            targetNode = currentA.CalculatePath(targetPos);


            Debug.Log(targetNode.transform.position);

        //If you can't see the enemy and we know his last position, search in the area around last know pos
        } else if (currentA.knownEnemyPosition != null && !currentA.canSeeEnemy && currentA.findNodeCloseToPosition(transform.position) == targetNode)
        {
            Debug.Log("Last position");
            float randomX = Random.Range(-10, 10);
            float randomZ = Random.Range(-10, 10);

            Vector3 targetPos = currentA.knownEnemyPosition + new Vector3(randomX, 0, randomZ);
            targetNode = currentA.CalculatePath(targetPos);
            searchCount++;
        //If we don't have the last positon and can't see the enemy abort plan
        } else if (currentA.knownEnemyPosition == Vector3.zero && !currentA.canSeeEnemy)
        {
            return false;
        }
        
        
        else if (currentA.findNodeCloseToPosition(transform.position) == targetNode)
        {
            reachedNode = true;
        }

        //If we have searched 5 locations abort plan
        if(searchCount > 5)
        {
            return false;
        } 

        //If health is under accepted level abort plan (bad variable name)
        return currentA.healthOver10;
        


    }

    public override bool requiresInRange()
    {
        return false;
    }

  
}
