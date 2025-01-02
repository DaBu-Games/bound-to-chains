using UnityEngine;
using UnityEngine.Playables;

public class StateMachine : MonoBehaviour
{

    private State currentState;

    void Start()
    {
        //currentState = ;
        currentState.EnterState();
    }

    void Update()
    {
        currentState.UpdateState();
    }

    public void SwitchState( State newState )
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
}
