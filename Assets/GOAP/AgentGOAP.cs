using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Entire GOAP system is heavly inspired by
https://gamedevelopment.tutsplus.com/tutorials/goal-oriented-action-planning-for-a-smarter-ai--cms-20793
*/


public class AgentGOAP : MonoBehaviour
{


    private FSM stateMachine;
    private FSM.FSMState idleState;
    private FSM.FSMState moveToState;
    private FSM.FSMState performActionState;
    private FSM.FSMState resetActionState;

    private HashSet<AbstractGOAPAction> availableActions;
    private Queue<AbstractGOAPAction> currentActions;
    private IGoap dataProvider;
    private ActionPlanner planner;
    [SerializeField]
    private AbstractGOAPAction[] actions;

    // Use this for initialization
    void Start()
    {
        stateMachine = new FSM();
        availableActions = new HashSet<AbstractGOAPAction>();
        currentActions = new Queue<AbstractGOAPAction>();
        planner = new ActionPlanner();
        findDataProvider();
        createIdleState();
        createMoveToState();
        createPerformActionState();
        stateMachine.pushState(idleState);
        loadActions();

        actions = GetComponents<AbstractGOAPAction>();

    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update(this.gameObject);
    }

    public void addAction(AbstractGOAPAction action)
    {
        availableActions.Add(action);
    }

    public AbstractGOAPAction getAction(Type action)
    {
        foreach (AbstractGOAPAction currAction in availableActions)
        {
            if (currAction.GetType().Equals(action))
            {
                return currAction;
            }
        }

        return null;
    }

    public void removeAction(AbstractGOAPAction action)
    {
        availableActions.Remove(action);
    }

    private bool hasActionPlan()
    {
        return currentActions.Count > 0;
    }

    private void createIdleState()
    {
        idleState = (fsm, obj) => {
            //Get the world state and goals
            HashSet<KeyValuePair<string, object>> worldState = dataProvider.getWorldState();
            HashSet<KeyValuePair<string, object>> goal = dataProvider.createGoalState();
            //Get the plan
            Queue<AbstractGOAPAction> plan = planner.plan(gameObject, availableActions, worldState, goal);
            if (plan != null)
            {
                //Set the actions to the actions in the plan
                currentActions = plan;
                //Tell agent it found a plan
                dataProvider.planFound(goal, plan);

                //Pop the idlestate
                fsm.popState();
                //Start performing actions
                fsm.pushState(performActionState);
            }
            else { 
                //Tell agent plan failed
                dataProvider.planFailed(goal);
                //Try to find a new plan
                fsm.popState();
                fsm.pushState(idleState);
            }
        };
    }

    private void createMoveToState()
    {
        moveToState = (fsm, gameObject) => {

            AbstractGOAPAction action = currentActions.Peek();
            if (action.requiresInRange() && action.target == null)
            {
                fsm.popState();
                fsm.popState();
                fsm.pushState(idleState);
                return;
            }

            if (dataProvider.moveAgent(action))
            {
                fsm.popState();
            }

        };
    }

    private void createPerformActionState()
    {

        performActionState = (fsm, obj) => {

            if (!hasActionPlan())
            {
                //Go find new plan and tell the agent it is finished
                fsm.popState();
                fsm.pushState(idleState);
                dataProvider.actionsFinished();
                return;
            }

            AbstractGOAPAction action = currentActions.Peek();
            if (action.isDone())
            {
                //Take action out of action queue
                currentActions.Dequeue();
            }

            if (hasActionPlan())
            {
                //Set action to the action on top of the queue
                action = currentActions.Peek();
                //Check if you need to be in range
                bool inRange = action.requiresInRange() ? action.isInRange() : true;


                if (inRange)
                {
                    //Check if we could perfrom action if not go to idle state and find a new plan
                    bool success = action.perform(obj);
                    if (!success)
                    {
                        fsm.popState();
                        fsm.pushState(idleState);
                        //ABORT
                        dataProvider.planAborted(action);
                    }
                }
                else
                {
                    fsm.pushState(moveToState);
                }
            }
            else
            {
                //I don't have a plan and need to find one
                fsm.popState();
                fsm.pushState(idleState);
                dataProvider.actionsFinished();
            }
        };
    }

    private void findDataProvider()
    {
        foreach (Component comp in gameObject.GetComponents(typeof(Component)))
        {
            if (typeof(IGoap).IsAssignableFrom(comp.GetType()))
            {
                dataProvider = (IGoap)comp;
                return;
            }
        }
    }

    private void loadActions()
    {
        AbstractGOAPAction[] actions = gameObject.GetComponents<AbstractGOAPAction>();
        foreach (AbstractGOAPAction a in actions)
        {
            availableActions.Add(a);
        }
    }


    public void resetActions()
    {
        if (availableActions != null)
        {
            foreach (AbstractGOAPAction ac in actions)
            {
                ac.reset();
            }
        }
    }

    public void ResetPlans()
    {
        resetActionState = (fsm, obj) =>
        {
            fsm.popState();
            fsm.pushState(idleState);
        };
    }

}
