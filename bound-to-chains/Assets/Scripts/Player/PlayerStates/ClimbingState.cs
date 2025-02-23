using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbingState : BaseState<Player>
{
    public ClimbingState(Player stateMachine) : base(stateMachine) { }

    public override void OnUpdate() { }

    public override void OnFixedUpdate()
    {
        stateMachine.UpdateHighestChain();
        stateMachine.ClimbChain();
        stateMachine.MovingWhileClimbing();
    }

    public override void OnEnterState()
    {
        stateMachine.SetPlayerGravity(0f);
        stateMachine.SetExcludeLayers( stateMachine.variables.climbExcludeLayers);

        stateMachine.ResetCharachterRotation();

        stateMachine.SetLinearVelocity( new Vector2( stateMachine.GetLinearVelocity().x, 0) );

        stateMachine.playerAnimator.Play("ClimbingAnimation");
    }

    public override void OnExitState()
    {
        stateMachine.SetPlayerGravity(stateMachine.variables.defaultGravity);
        if (!stateMachine.checkForChains.isColliding)
            stateMachine.ResetExludeLayers();
    }
}

