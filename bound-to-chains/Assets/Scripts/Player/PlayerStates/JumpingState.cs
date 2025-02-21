using Unity.VisualScripting;
using UnityEngine;

public class JumpingState : BaseState<Player>
{
    public JumpingState(Player stateMachine) : base(stateMachine) { }

    public override void OnUpdate() {}

    public override void OnFixedUpdate()
    {
        stateMachine.Jump();
    }

    public override void OnEnterState()
    {
        stateMachine.CanJump();
    }

    public override void OnExitState() {}
}
