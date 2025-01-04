using UnityEngine;

public class IdleState : State
{

    [SerializeField] private WalkingState walkingState;
    [SerializeField] private JumpingState jumpingState;
    [SerializeField] private ThrowState throwState;
    [SerializeField] private ClimbingState climbingState;

    public override void EnterState()
    {
      
    }

    public override void ExitState()
    {

    }

    public override void FixedUpdateState()
    {
        DecelPlayer(); 
    }

    public override void UpdateState()
    {
        // check if the player is holding the jump button if so enter the jump state
        if ( jumpingState.IsJumpBufferd() ) 
        {
            stateMachine.SwitchState( jumpingState );
        }
        // check if the player is holding the climbe button if so enter the climbe state
        else if ( playerInput.isHoldingClimbe && climbingState.IsPlayerBelowBall() )
        {
            stateMachine.SwitchState( climbingState );
        }
        // check if the player has x axis input if so enter walk state
        else if ( playerInput.moveInput.x != 0 )
        {
            stateMachine.SwitchState( walkingState );
        }
        // check if the player is pressing the throw button if so enter the throw state
        else if ( playerInput.isHoldingCharge && throwState.CheckForBall() )
        {
            stateMachine.SwitchState(throwState);
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
        playerInput.rb2d.AddForce ( movement * Vector2.right, ForceMode2D.Force );
    }
}
