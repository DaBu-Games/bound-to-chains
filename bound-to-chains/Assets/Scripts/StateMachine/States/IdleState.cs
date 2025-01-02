using UnityEngine;

public class IdleState : State
{

    [SerializeField] private WalkingState walkingState;
    [SerializeField] private JumpingState jumpingState;

    public override void EnterState()
    {
        Debug.Log("Start idle state");
    }

    public override void ExitState()
    {
        Debug.Log("Stop idle state");
    }

    public override void FixedUpdateState()
    {
        DecelPlayer(); 
    }

    public override void UpdateState()
    {

        if ( playerInput.isHoldingJump ) 
        {
            stateMachine.SwitchState( jumpingState );
        }

        else if ( playerInput.moveInput.x != 0 )
        {
            stateMachine.SwitchState( walkingState );
        }

    }

    private void DecelPlayer()
    {
        // Calculate the target speed based on player input and max movement speed
        float targetSpeed = playerInput.moveInput.x * playerInput.variables.maxMoveSpeed;

        // Calculate the difference between the target speed and the current velocity
        float speedDif = targetSpeed - playerInput.rb2d.linearVelocity.x;

        // Calculate the deceleration force to apply
        float movement = speedDif * playerInput.variables.moveSpeedDeccelGround;

        // Apply the calculated force to decelerate the Rigidbody2D
        playerInput.rb2d.AddForce (movement * Vector2.right, ForceMode2D.Force );
    }
}
