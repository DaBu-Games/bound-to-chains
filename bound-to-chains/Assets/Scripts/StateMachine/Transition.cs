using UnityEngine;

public class Transition
{
    public readonly IState fromState;
    public readonly IState toState;
    private System.Func<bool> condition;

    public Transition(IState fromState, IState toState, System.Func<bool> condition)
    {
        this.fromState = fromState;
        this.toState = toState;
        this.condition = condition;
    }

    public bool CheckCondition()
    {
        return condition();
    }

}
