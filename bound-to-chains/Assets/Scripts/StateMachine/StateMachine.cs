using UnityEngine;
using UnityEngine.Playables;

public class StateMachine : MonoBehaviour
{

    private State currentState;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform states; 

    void Start()
    {

        // Get all the states of the parent transform states
        foreach ( Transform stateTransform in states ) 
        {
            State state = stateTransform.GetComponent<State>(); 

            // Call the Initialize function and pas in the stateMachine and the playerinput
            state.Initialize( this, playerInput, playerAnimator );

            // make the start state the IdleState
            if( state is IdleState)
            {
                currentState = state;
            }
            
        }
        
        // Call the EnterState function of the IdleState
        currentState.EnterState();
    }

    void Update()
    {
        // if current state is not empty call the UpdateState function
        currentState?.UpdateState();
    }

    void FixedUpdate()
    {
        // if currentState is not empty call the FixedUpdateState function
        currentState?.FixedUpdateState();
    }

    // When the state gets switched call the ExitState function on the old state
    // and call the EnterState function on the newState
    public void SwitchState( State newState )
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    public State GetCurrentState()
    {
        return currentState;
    }
}
