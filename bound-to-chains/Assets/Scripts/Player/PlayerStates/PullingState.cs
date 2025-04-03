using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PullingState : BaseState<Player>
{
    public PullingState(Player stateMachine) : base(stateMachine) { }

    public override void OnUpdate()
    {
        if (!stateMachine.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("ThrowingAnimation"))
        {
            stateMachine.CheckChargeDuration(false);
        }
    }

    public override void OnFixedUpdate() { }
    public override void OnEnterState()
    {
        stateMachine.playerAnimator.Play("ChargeAnimation");
        stateMachine.SetPlayerMass(stateMachine.variables.crouchingMass);
        stateMachine.SetPlayerDamping(stateMachine.variables.crouchingDamping);
        stateMachine.StartCharge();
    }

    public override void OnExitState()
    {
        stateMachine.ResetPlayerMass();
        stateMachine.ResetPlayerDamping();
    }
}
