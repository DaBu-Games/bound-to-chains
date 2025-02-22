using UnityEngine;
using UnityEngine.InputSystem;

public class FallingState : BaseState<Player>
{
    public FallingState(Player stateMachine) : base(stateMachine) { }

    public override void OnUpdate() { }

    public override void OnFixedUpdate()
    {
        stateMachine.WhileJumping();
        stateMachine.MovingAir();
        stateMachine.FlipCharachter();
    }

    public override void OnEnterState()
    {
        stateMachine.SetPlayerGravity(stateMachine.variables.fallingGravity);
        stateMachine.playerAnimator.Play("FallingAnimation");
    }

    public override void OnExitState()
    {
        stateMachine.SetPlayerGravity(stateMachine.variables.defaultGravity);
    }
}
