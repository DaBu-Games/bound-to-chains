using UnityEngine;
using UnityEngine.InputSystem;

public class HangingState : BaseState<Player>
{
    public HangingState(Player stateMachine) : base(stateMachine) { }

    public override void OnUpdate() { }

    public override void OnFixedUpdate()
    {
        stateMachine.DecelPlayer();
        stateMachine.FlipCharachter();
    }
    public override void OnEnterState()
    {
        stateMachine.playerAnimator.Play("HangingAnimation");
    }

    public override void OnExitState() { }
}
