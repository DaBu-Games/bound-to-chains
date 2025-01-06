using UnityEngine;

public abstract class RisingState : State
{
    [SerializeField] private ClimbingState climbingState;
    [SerializeField] private JumpingState jumpingState;
    [SerializeField] private WalkingState walkingState;
    [SerializeField] private IdleState idleState;
    [SerializeField] private FallingState fallingState;

    [SerializeField] private CheckForGround playerGroundCheck;

    public override void EnterState()
    {
        
    }

    public override void ExitState()
    {

    }

    public override void FixedUpdateState()
    {
        WhileJumping();
        MovingAir();
    }

    public override void UpdateState()
    {

        // check if the player is holding the climbe button and is under the ball if so enter the climbe state
        if (playerInput.isHoldingClimbe && climbingState.IsPlayerBelowBall())
        {
            stateMachine.SwitchState(climbingState);
        }
        // all the states that can happen when the player is grounded
        else if (playerGroundCheck.isGrounded)
        {

            // check if the player is holding the jump button if so enter the jump state
            if (jumpingState.IsJumpBufferd())
            {
                stateMachine.SwitchState(jumpingState);
            }
            // check if the player has x axis input if so enter walk state
            else if (playerInput.moveInput.x != 0)
            {
                stateMachine.SwitchState(walkingState);
            }
            // if there are no inputs go to the idle state
            else
            {
                stateMachine.SwitchState(idleState);
            }

        }
        // the state that can happen when the player is still in the air
        else
        {

            HandleAirborneSpecific();

        }

    }

    private void MovingAir()
    {
        // Calculate the target speed based on player input and max movement speed
        float targetSpeed = playerInput.moveInput.x * playerInput.variables.maxMoveSpeed;

        // check if the player is moving if is so accel if not deccel 
        float accelRate = (Mathf.Abs(playerInput.moveInput.x) > 0.01f) ? playerInput.variables.moveSpeedAccelAir : playerInput.variables.moveSpeedDeccelAir;

        // Calculate the difference between the target speed and the current velocity
        float speedDif = targetSpeed - playerInput.rb2d.linearVelocity.x;

        // Calculate the force to be applied based on the acceleration rate and speed difference
        float movement = speedDif * accelRate;

        // Apply the calculated force to the Rigidbody2D
        playerInput.rb2d.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    protected virtual void WhileJumping()
    {

        // check if the jump button is let go while jumping
        // if so increase the gravity of the player
        if ( jumpingState.isJumping && !playerInput.isHoldingJump )
        {

            playerInput.rb2d.gravityScale = playerInput.variables.jumpCutGravity;

        }

    }

    protected virtual void HandleAirborneSpecific()
    {
        // check if the player is falling if so switch to the falling state
        if ( playerInput.rb2d.linearVelocity.y < 0 )
        {
            stateMachine.SwitchState( fallingState );
        }
    }

}
