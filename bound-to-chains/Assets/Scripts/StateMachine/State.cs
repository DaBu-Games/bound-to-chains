using UnityEngine;

public abstract class State
{

    protected StateMachine stateMachine;

    public State( StateMachine stateMachine )
    {
        this.stateMachine = stateMachine;
    }

    public abstract void EnterState();
    public abstract void UpdateState(); 
    public abstract void FixedUpdateState(); // Use this function if you want to run physics code
    public abstract void ExitState();
}
