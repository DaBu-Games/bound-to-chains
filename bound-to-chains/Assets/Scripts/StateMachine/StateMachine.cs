using UnityEngine;
using System;
using System.Collections.Generic;



public class StateMachine
{
    public IState currentState { get; private set; }
    private Dictionary<Type, IState> allStates = new Dictionary<Type, IState>();
    private List<Transition> transitions = new List<Transition>();
    private List<Transition> currentTransitions = new List<Transition>();

    public void OnUpdate()
    {
        foreach (var transition in currentTransitions) 
        {
           if( transition.CheckCondition())
               SwitchState( transition.toState );
        }
        
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

    public void RemoveState(Type type) 
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

        //Debug.Log(currentState.ToString());
    }

    public void AddTransition(Transition transition)
    {
        transitions.Add(transition);
    }
}
