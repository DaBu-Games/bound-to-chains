using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WalkingState : BaseState<Player>
{
    public WalkingState(Player stateMachine) : base(stateMachine) { }

    public override void OnUpdate() { }

    public override void OnFixedUpdate()
    {
        stateMachine.Walking();
        stateMachine.FlipCharachter();
    }

    public override void OnEnterState()
    {
        stateMachine.playerAnimator.Play("WalkAnimation");
    }

    public override void OnExitState() { }
}
