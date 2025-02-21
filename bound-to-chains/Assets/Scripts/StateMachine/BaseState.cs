using UnityEngine;

public abstract class BaseState<T> : IState
{

    protected T stateMachine;

    public void Initialize( T stateMachine )
    {
        this.stateMachine = stateMachine;
    }

    public abstract void OnUpdate();

    public abstract void OnFixedUpdate();

    public abstract void OnEnterState();

    public abstract void OnExitState();
}

public interface IState
{
    void OnUpdate();
    void OnFixedUpdate();
    void OnEnterState();
    void OnExitState();
}
