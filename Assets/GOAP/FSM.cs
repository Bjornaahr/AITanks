using System.Collections.Generic;
using UnityEngine;
/**
 * Stack-based Finite State Machine.
 * Push and pop states to the FSM.
 * 
 * States should push other states onto the stack 
 * and pop themselves off.
 */


public class FSM
{

    private Stack<FSMState> stateStack = new Stack<FSMState>();

    public delegate void FSMState(FSM fsm, GameObject gameObject);

    //Checks if there is something in the stack and invokes the action on top
    public void Update(GameObject gameObject)
    {
        if (stateStack.Peek() != null)
            stateStack.Peek().Invoke(this, gameObject);
    }

    public void pushState(FSMState state)
    {
        stateStack.Push(state);
    }

    public void popState()
    {
        stateStack.Pop();
    }
}
