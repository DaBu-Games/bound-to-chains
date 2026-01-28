using UnityEngine;
using UnityEngine.InputSystem;

public class FallingState : BaseState<Player>
{
    public FallingState(Player stateMachine) : base(stateMachine) { }

    public override void OnUpdate() { }

    public override void OnFixedUpdate()
    {
        stateMachine.WhileJumping();
        stateMachine.MovePlayer( stateMachine.variables.moveSpeedAccelAir, stateMachine.variables.moveSpeedDeccelAir );
        stateMachine.FlipCharachterOnInput();
        if (!stateMachine.playerCollisionCheck.IsColliding())
            stateMachine.ResetExludeLayers();
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
