using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAction : AbstractGOAPAction
{

    public SearchAction()
    {
        addPrecondition("canSeeEnemy", false);
        addPrecondition("hasEnoughHealth", true);
    }


    //Check if we have last known position, if not take it random walk
    public override bool checkPrecondtion(GameObject agent)
    {
        throw new System.NotImplementedException();
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

    public override void reset()
    {
        
    }

}
