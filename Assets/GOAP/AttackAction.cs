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

    public AttackAction()
    {
        addPrecondition("hasEnoughHealth", true);
        addPrecondition("canSeeEnemy", true);
        addEffect("damageTank", true);
    }

    public override bool checkPrecondtion(GameObject agent)
    {
        return true;
    }

    public override void reset()
    {
        reachedNode = false;
        targetNode = null;
        Debug.Log("ResetAttack");

    }

    public override bool isDone()
    {
        Debug.Log("Done");
        return reachedNode;
    }

    public override bool perform(GameObject agent)
    {
        Tank currentA = agent.GetComponent<Tank>();


        Debug.Log("Attack");

        if (targetNode == null)
        {
            var radius = 3;
            var vector2 = Random.insideUnitCircle.normalized * radius;
            var targetPos = currentA.EnemyTank.transform.position + new Vector3(vector2.x, 0, vector2.y);
            targetNode = currentA.CalculatePath(targetPos);
            Debug.Log(targetNode.transform.position);
        } else if (currentA.findNodeCloseToPosition(transform.position) == targetNode)
        {
            reachedNode = true;
        }

        return true;

    }

    public override bool requiresInRange()
    {
        return false;
    }

  
}
