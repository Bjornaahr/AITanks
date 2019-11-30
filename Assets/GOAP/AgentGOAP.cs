using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentGOAP : MonoBehaviour
{


    private FSM stateMachine;
    private FSM.FSMState idleState;
    private FSM.FSMState moveToState;
    private FSM.FSMState performActionState;

    private HashSet<AbstractGOAPAction> availableActions;
    private Queue<AbstractGOAPAction> currentActions;
    private IGoap dataProvider;
    private ActionPlanner planner;


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

            HashSet<KeyValuePair<string, object>> worldState = dataProvider.getWorldState();
            HashSet<KeyValuePair<string, object>> goal = dataProvider.createGoalState();

            Queue<AbstractGOAPAction> plan = planner.plan(gameObject, availableActions, worldState, goal);
            if (plan != null)
            {
                currentActions = plan;
                dataProvider.planFound(goal, plan);

                fsm.popState();
                fsm.pushState(performActionState);
            }
            else
            {
                dataProvider.planFailed(goal);
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
                fsm.popState();
                fsm.pushState(idleState);
                dataProvider.actionsFinished();
                return;
            }

            AbstractGOAPAction action = currentActions.Peek();
            if (action.isDone())
            {
                currentActions.Dequeue();
            }

            if (hasActionPlan())
            {
                action = currentActions.Peek();
                bool inRange = action.requiresInRange() ? action.isInRange() : true;

                if (inRange)
                {
                    bool success = action.perform(obj);
                    if (!success)
                    {
                        fsm.popState();
                        fsm.pushState(idleState);
                        createIdleState();
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
            foreach (AbstractGOAPAction ac in availableActions)
            {
                ac.reset();
            }
        }
    }



}
