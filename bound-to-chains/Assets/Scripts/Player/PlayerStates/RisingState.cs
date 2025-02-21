using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class RisingState : BaseState<Player>
{
    public RisingState(Player stateMachine) : base(stateMachine) { }

    public override void OnUpdate() { }

    public override void OnFixedUpdate()
    {
        stateMachine.WhileJumping();
        stateMachine.MovingAir();
        stateMachine.FlipCharachter();
    }

    public override void OnEnterState()
    {
        stateMachine.playerAnimator.Play("RisingAnimation");
    }

    public override void OnExitState()
    {
        if (stateMachine.isJumping)
        {
            stateMachine.CancelJump();
        }
        if (!stateMachine.HasNoExcludeLayers() && !stateMachine.checkForChains.isColliding)
        {
            stateMachine.ResetExludeLayers();
        }
    }
}
