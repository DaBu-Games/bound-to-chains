using UnityEngine;

public abstract class State : MonoBehaviour
{

    protected StateMachine stateMachine;
    protected PlayerInput playerInput;

    public void Initialize( StateMachine stateMachine, PlayerInput playerInput )
    {
        this.stateMachine = stateMachine;
        this.playerInput = playerInput;
    }

    public abstract void EnterState();
    public abstract void UpdateState(); 
    public abstract void FixedUpdateState(); // Use this function if you want to run physics code
    public abstract void ExitState();
}
