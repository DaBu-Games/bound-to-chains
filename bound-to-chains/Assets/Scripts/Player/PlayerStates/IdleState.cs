using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : BaseState<Player>
{
    public IdleState(Player stateMachine) : base(stateMachine) { }

    public override void OnUpdate() { }

    public override void OnFixedUpdate()
    {
        stateMachine.DecelPlayer( stateMachine.variables.moveSpeedDeccelGround ); 
    }

    public override void OnEnterState()
    {
        stateMachine.playerAnimator.Play("IdleAnimation");
        stateMachine.ResetPlayerMass();
        stateMachine.ResetPlayerDamping();
    }

    public override void OnExitState() { }
}
