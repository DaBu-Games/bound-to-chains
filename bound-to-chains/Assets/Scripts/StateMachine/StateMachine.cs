using UnityEngine;
using UnityEngine.Playables;

public class StateMachine : MonoBehaviour
{

    private State currentState;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform states; 

    void Start()
    {

        foreach ( Transform stateTransform in states ) 
        {
            State state = stateTransform.GetComponent<State>(); 

            state.Initialize( this, playerInput );

            if( state is IdleState)
            {
                currentState = state;
            }
            
        }
        
        currentState.EnterState();
    }

    void Update()
    {
        currentState?.UpdateState();
    }

    void FixedUpdate()
    {
        currentState?.FixedUpdateState();
    }

    public void SwitchState( State newState )
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
}
