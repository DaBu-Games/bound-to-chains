using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class StateMachine
{
    public IState currentState { get; private set; }
    private Dictionary<System.Type, IState> allStates = new Dictionary<System.Type, IState>();
    private List<Transition> transitions = new List<Transition>();
    private List<Transition> currentTransitions = new List<Transition>();

    public void OnUpdate()
    {

        foreach ( var transition in currentTransitions ) 
        {
           if( transition.CheckCondition())
           {
                SwitchState( transition.toState );
           }
        }

        Debug.Log(currentState.ToString());
        
        currentState?.OnUpdate();
    }

    public void OnFixedUpdate()
    {
        currentState?.OnFixedUpdate();
    }

    public void AddState(IState state)
    {
        allStates.TryAdd(state.GetType(), state);
    }

    public void RemoveState(System.Type type) 
    {
        if ( allStates.ContainsKey(type) )
            allStates.Remove(type);
    }

    public void SwitchState(IState state)
    {
        currentState?.OnExitState();
        currentState = state;
        if (currentState == null)
            return;

        currentTransitions = transitions.FindAll(x => x.fromState == currentState || x.fromState == null);
        currentState.OnEnterState();
    }

    public void AddTransition(Transition transition)
    {
        transitions.Add(transition);
    }
}
