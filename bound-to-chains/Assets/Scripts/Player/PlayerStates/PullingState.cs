using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PullingState : BaseState<Player>
{
    public PullingState(Player stateMachine) : base(stateMachine) { }

    private float crouchingMass = 5f;
    private float Damping = 2f;

    public override void OnUpdate()
    {
        stateMachine.CheckChargeDuration( false );
    }

    public override void OnFixedUpdate()
    {
        stateMachine.CheckForBall(); 
    }

    public override void OnEnterState()
    {
        stateMachine.playerAnimator.Play("ChargeAnimation");
        stateMachine.SetPlayerMass(crouchingMass);
        stateMachine.SetPlayerDamping(Damping);
        stateMachine.StartCharge();
    }

    public override void OnExitState()
    {
        stateMachine.ResetPlayerMass();
        stateMachine.ResetPlayerDamping();
    }
}
