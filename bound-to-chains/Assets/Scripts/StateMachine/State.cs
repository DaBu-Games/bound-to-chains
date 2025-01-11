using UnityEngine;

public abstract class State : MonoBehaviour
{

    protected StateMachine stateMachine;
    protected PlayerInput playerInput;
    protected Animator playerAnimator; 

    public void Initialize( StateMachine stateMachine, PlayerInput playerInput, Animator playerAnimator )
    {
        this.stateMachine = stateMachine;
        this.playerInput = playerInput;
        this.playerAnimator = playerAnimator; 
    }

    public abstract void EnterState(); // function that happends when you enter the state
    public abstract void UpdateState(); // function that happends every update 
    public abstract void FixedUpdateState(); // Use this function if you want to run physics code
    public abstract void ExitState(); // function that happends when you exit the state
}
