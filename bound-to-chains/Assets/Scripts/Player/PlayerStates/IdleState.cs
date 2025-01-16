using UnityEngine;

public class IdleState : State
{
    [SerializeField] private CheckForGround playerGroundCheck;

    [SerializeField] private CrouchingState crouchingState;
    [SerializeField] private WalkingState walkingState;
    [SerializeField] private JumpingState jumpingState;
    [SerializeField] private PullingState pullingState;
    [SerializeField] private ClimbingState climbingState;
    [SerializeField] private RisingState risingState;
    [SerializeField] private FallingState fallingState;

    public override void EnterState()
    {
        playerAnimator.Play("IdleAnimation");
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
        if ( jumpingState.CanPlayerJump() ) 
        {
            stateMachine.SwitchState( jumpingState );
        }
        // check if the player is holding the climbe button and is bellow the ball if so enter the climbe state
        else if ( climbingState.CanPlayerClimbe() )
        {
            stateMachine.SwitchState( climbingState );
        }
        // check if the player has x axis input if so enter walk state
        else if ( walkingState.CanPlayerWalk() )
        {
            stateMachine.SwitchState( walkingState );
        }
        else if ( crouchingState.CanPlayerCrouch() )
        {
            stateMachine.SwitchState( crouchingState );
        }
        else if ( pullingState.CanPlayerPull() )
        {
            stateMachine.SwitchState( pullingState );
        }
        else if ( fallingState.CanPlayerFall() )
        {
            stateMachine.SwitchState( fallingState) ;
        }
        else if ( risingState.CanPlayerRise())
        {
            stateMachine.SwitchState( risingState );
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


    // Check if the player has no x input and is grounded
    public bool CanPlayerIdle()
    {
        return playerInput.moveInput.x == 0 && playerGroundCheck.isGrounded; 
    }
}
