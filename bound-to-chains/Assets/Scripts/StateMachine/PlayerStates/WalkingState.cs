using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WalkingState : State
{

    [SerializeField] private CheckForGround playerGroundCheck;

    [SerializeField] private IdleState idleState;
    [SerializeField] private JumpingState jumpingState;
    [SerializeField] private FallingState fallingState;
    [SerializeField] private HangingState hangingState;

    public override void EnterState()
    {
        playerAnimator.Play("WalkAnimation");
    }

    public override void ExitState()
    {
        
    }

    public override void FixedUpdateState()
    {
        Walking();

        if (!playerInput.facingRight && playerInput.moveInput.x > 0 || playerInput.facingRight && playerInput.moveInput.x < 0)
        {

            playerInput.FlipCharachter();

        }
    }

    public override void UpdateState()
    {
        // check if the player can jump if so enter the jump state
        if ( jumpingState.CanPlayerJump() )
        {
            stateMachine.SwitchState(jumpingState);
        }
        // check if the player has no x axis input if so enter the idle state
        else if ( idleState.CanPlayerIdle() )
        {
            stateMachine.SwitchState(idleState);
        }
        else if ( hangingState.CanPlayerHang() )
        {
            stateMachine.SwitchState(hangingState);
        }
        else if ( fallingState.CanPlayerFall() )
        {
            stateMachine.SwitchState( fallingState );
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

    // Check if the player input x is not equal to 0 and if the player is grounded
    public bool CanPlayerWalk()
    {
        return playerInput.moveInput.x != 0 && playerGroundCheck.isGrounded; 
    }
}
