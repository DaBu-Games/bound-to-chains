using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbingState : BaseState<Player>
{
    public ClimbingState(Player stateMachine) : base(stateMachine) { }

    public override void OnUpdate() { }

    public override void OnFixedUpdate()
    {
        stateMachine.ClimbChain();
        stateMachine.MovePlayer(stateMachine.variables.moveSpeedAccelClimbing, stateMachine.variables.moveSpeedAccelClimbing);
    }

    public override void OnEnterState()
    {
        stateMachine.generateChain.SetAutoLength(false);
        stateMachine.generateChain.SetCurrentChainLength();
        stateMachine.SetIsTrigger(true);
        stateMachine.ResetCharachterRotation();
        stateMachine.SetPlayerGravity(0f);
        stateMachine.playerAnimator.Play("ClimbingAnimation");
    }

    public override void OnExitState()
    {
        stateMachine.SetPlayerGravity(stateMachine.variables.defaultGravity);
        stateMachine.generateChain.SetAutoLength(true);

        if (!stateMachine.playerCollisionCheck.isColliding)
            stateMachine.SetIsTrigger(false);
    }
}

