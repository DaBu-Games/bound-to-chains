using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbingState : BaseState<Player>
{
    public ClimbingState(Player stateMachine) : base(stateMachine) { }

    public override void OnUpdate() 
    {
        stateMachine.RotateCharachterToDirection(stateMachine.playerTransform.position, stateMachine.ballTransform.position);
    }

    public override void OnFixedUpdate()
    {
        stateMachine.ClimbChain();
        stateMachine.MovePlayer(stateMachine.variables.moveSpeedAccelClimbing, stateMachine.variables.moveSpeedAccelClimbing);
    }

    public override void OnEnterState()
    {
        stateMachine.generateChain.SetLengthDistanceJoined(stateMachine.generateChain.GetDistance()); 
        stateMachine.generateChain.SetIsShortening(true);
        stateMachine.SetExcludeLayers(stateMachine.variables.climbExcludeLayers);
        stateMachine.ResetCharachterRotation();
        stateMachine.playerAnimator.Play("ClimbingAnimation");
    }

    public override void OnExitState()
    {
        stateMachine.generateChain.SetIsShortening(false);
        stateMachine.ResetCharachterRotation();

        if (!stateMachine.playerCollisionCheck.IsColliding())
            stateMachine.ResetExludeLayers();
    }
}

