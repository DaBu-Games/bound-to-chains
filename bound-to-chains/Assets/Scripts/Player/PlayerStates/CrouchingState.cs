using UnityEngine;
using UnityEngine.InputSystem;

public class CrouchingState : BaseState<Player>
{
    public CrouchingState(Player stateMachine) : base(stateMachine) { }

    private float crouchingMass = 5f;
    private float Damping = 2f;

    public override void OnUpdate() { }

    public override void OnFixedUpdate()
    {
        stateMachine.FlipCharachter();
    }

    public override void OnEnterState()
    {
        stateMachine.playerAnimator.Play("ChargeAnimation");
        stateMachine.SetPlayerMass(crouchingMass);
        stateMachine.SetPlayerDamping(Damping);
        stateMachine.ResetLinearVelocity();
    }

    public override void OnExitState()
    {
        stateMachine.ResetPlayerMass();
        stateMachine.ResetPlayerDamping();
    }
}
