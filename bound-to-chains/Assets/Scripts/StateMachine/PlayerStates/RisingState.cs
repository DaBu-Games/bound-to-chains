using UnityEngine;
using UnityEngine.InputSystem;

public class RisingState : State
{
    [SerializeField] private ClimbingState climbingState;
    [SerializeField] private JumpingState jumpingState;
    [SerializeField] private WalkingState walkingState;
    [SerializeField] private IdleState idleState;
    [SerializeField] private FallingState fallingState;
    [SerializeField] private HangingState hangingState;

    [SerializeField] protected CheckForGround playerGroundCheck;

    public override void EnterState()
    {
        playerAnimator.Play("RisingAnimation");
    }

    public override void ExitState()
    {
        if ( jumpingState.isJumping ) 
        { 
            jumpingState.CancelJump();
        }
        if( !playerInput.HasNoExcludeLayers() )
        {
            playerInput.ResetExludeLayers();
        }
    }

    public override void FixedUpdateState()
    {
        WhileJumping();
        MovingAir();
        LimitVelocity();

        if (!playerInput.facingRight && playerInput.moveInput.x > 0 || playerInput.facingRight && playerInput.moveInput.x < 0)
        {

            playerInput.FlipCharachter();

        }
    }

    public override void UpdateState()
    {

        // check if the player is holding the climbe button and is under the ball if so enter the climbe state
        if ( climbingState.CanPlayerClimbe() )
        {
            stateMachine.SwitchState(climbingState);
            return;
        }

        if( hangingState.CanPlayerHang() )
        {
            stateMachine.SwitchState(hangingState);
            return;
        }

        // all the states that can happen when the player is grounded
        if ( !jumpingState.isJumping && playerInput.HasNoExcludeLayers() )
        {

            // check if the player is holding the jump button if so enter the jump state
            if ( jumpingState.CanPlayerJump() )
            {
                stateMachine.SwitchState(jumpingState);
                return;
            }
            // check if the player has x axis input if so enter walk state
            else if ( walkingState.CanPlayerWalk() )
            {
                stateMachine.SwitchState(walkingState);
                return;
            }
            // if there are no inputs go to the idle state
            else if ( idleState.CanPlayerIdle() )
            {
                stateMachine.SwitchState(idleState);
                return;
            }

        }

        // the state that can happen when the player is still in the air
        HandleAirborneSpecific();

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
        // if so increase the gravity of the player and "cancel" the jump
        if ( jumpingState.isJumping && !playerInput.isHoldingJump )
        {

            playerInput.rb2d.gravityScale = playerInput.variables.jumpCutGravity;
            jumpingState.CancelJump();

        }

    }

    protected virtual void LimitVelocity()
    {
        return;
    }

    protected virtual void HandleAirborneSpecific()
    {
        // check if the player is falling if so switch to the falling state
        if ( fallingState.CanPlayerFall() )
        {   
            stateMachine.SwitchState( fallingState );
        }
    }

    // Check if the player has a upward velocity above 0 and the player is not grounded
    public bool CanPlayerRise()
    {
        return playerInput.rb2d.linearVelocity.y > 0 && !playerGroundCheck.isGrounded;
    }


}
