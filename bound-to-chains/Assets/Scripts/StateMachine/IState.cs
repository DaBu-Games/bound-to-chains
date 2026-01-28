using UnityEngine;

public interface IState
{
    void OnUpdate();
    void OnFixedUpdate();
    void OnEnterState();
    void OnExitState();
}
