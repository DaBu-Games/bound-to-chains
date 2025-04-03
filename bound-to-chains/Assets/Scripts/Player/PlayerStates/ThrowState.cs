using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ThrowState : BaseState<Player>
{
    public ThrowState(Player stateMachine) : base(stateMachine) { }

    public override void OnUpdate()
    {
        if (!stateMachine.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("ThrowingAnimation"))
        {
            stateMachine.CheckChargeDuration(true);
        }
    }

    public override void OnFixedUpdate() { }

    public override void OnEnterState()
    {
        stateMachine.StartCharge(); 
    }

    public override void OnExitState()
    {
        stateMachine.ResetPlayerMass();
        stateMachine.ResetPlayerDamping();
    }
}
