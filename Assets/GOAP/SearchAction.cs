using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAction : AbstractGOAPAction
{

    private bool enemySeen = false;

    public SearchAction()
    {
        addPrecondition("canSeeEnemy", false);
        addPrecondition("hasEnoughHealth", true);
        addEffect("shootCannon", true);
    }

    public override void reset()
    {
        enemySeen = false;
    }

    //Check if we have last known position, if not take it random walk
    public override bool checkPrecondtion(GameObject agent)
    {
        return true;
    }

    public override bool isDone()
    {
        throw new System.NotImplementedException();
    }

    //Walk towards node, choose node until enemy is seen
    public override bool perform(GameObject agent)
    {
        throw new System.NotImplementedException();
    }

    public override bool requiresInRange()
    {
        return false;
    }

  

}
