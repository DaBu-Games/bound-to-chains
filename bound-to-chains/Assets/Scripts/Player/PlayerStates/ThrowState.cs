using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ThrowState : BaseState<Player>
{
    public ThrowState(Player stateMachine) : base(stateMachine) { }

    public override void OnUpdate()
    {
        stateMachine.CheckChargeDuration( true );
    }

    public override void OnFixedUpdate()
    {
        stateMachine.CheckForBall();
    }

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
