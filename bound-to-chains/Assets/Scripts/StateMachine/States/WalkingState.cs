using Unity.VisualScripting;
using UnityEngine;

public class WalkingState : State
{

    [SerializeField] private IdleState idleState;
    [SerializeField] private JumpingState jumpingState;

    public override void EnterState()
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override void FixedUpdateState()
    {
        Walking(); 
    }

    public override void UpdateState()
    {
        if (playerInput.isHoldingJump)
        {
            stateMachine.SwitchState(jumpingState);
        }
        else if ( playerInput.moveInput.x == 0 )
        {
            stateMachine.SwitchState(idleState);
        }
    }

    private void Walking()
    {
        // Calculate the target speed based on player input and max movement speed
        float targetSpeed = playerInput.moveInput.x * playerInput.variables.maxMoveSpeed;

        // check if the player is moving if is so accel if not deccel 
        float accelRate = (Mathf.Abs( playerInput.moveInput.x) > 0.01f) ? playerInput.variables.moveSpeedAccelGround : playerInput.variables.moveSpeedDeccelGround;

        // Calculate the difference between the target speed and the current velocity
        float speedDif = targetSpeed - playerInput.rb2d.linearVelocity.x;

        // Calculate the force to be applied based on the acceleration rate and speed difference
        float movement = speedDif * accelRate;

        // Apply the calculated force to the Rigidbody2D
        playerInput.rb2d.AddForce(movement * Vector2.right, ForceMode2D.Force);

    }
}
