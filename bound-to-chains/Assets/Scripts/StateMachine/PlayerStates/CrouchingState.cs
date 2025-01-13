using UnityEngine;

public class CrouchingState : State
{
    [SerializeField] private IdleState idleState;
    [SerializeField] private PullingState pullingState;
    [SerializeField] private ThrowState throwState;

    [SerializeField] private CheckForGround playerGroundCheck;

    [SerializeField] private float crouchingMass = 5f;
    [SerializeField] private float Damping = 2f;

    public override void EnterState()
    {

        playerAnimator.Play("ChargeAnimation");
        playerInput.SetPlayerMass( crouchingMass );
        playerInput.SetPlayerDamping( Damping );
        playerInput.rb2d.linearVelocity = Vector2.zero;

    }

    public override void ExitState()
    {
       
    }

    public override void FixedUpdateState()
    {
        Crouching();
        throwState.CheckForBall(); 

        if (!playerInput.facingRight && playerInput.moveInput.x > 0 || playerInput.facingRight && playerInput.moveInput.x < 0)
        {

            playerInput.FlipCharachter();

        }
    }

    public override void UpdateState()
    {

        if (throwState.CanPlayerThrow())
        {
            stateMachine.SwitchState(throwState);
        }
        else if ( pullingState.CanPlayerPull() )
        {
            stateMachine.SwitchState(pullingState);
        }

        if ( !CanPlayerCrouch() )
        {

            playerInput.ResetPlayerMass();
            playerInput.ResetPlayerDamping();

            stateMachine.SwitchState( idleState );

        }
    }

    private void Crouching()
    {
        // Calculate the target speed based on player input and max movement speed
        float targetSpeed = playerInput.moveInput.x * playerInput.variables.maxMoveSpeed;

        // check if the player is moving if is so accel if not deccel 
        float accelRate = (Mathf.Abs(playerInput.moveInput.x) > 0.01f) ? playerInput.variables.moveSpeedAccelCrouching : playerInput.variables.moveSpeedDeccelCrouching;

        // Calculate the difference between the target speed and the current velocity
        float speedDif = targetSpeed - playerInput.rb2d.linearVelocity.x;

        // Calculate the force to be applied based on the acceleration rate and speed difference
        float movement = speedDif * accelRate;

        // Apply the calculated force to the Rigidbody2D
        playerInput.rb2d.AddForce(movement * Vector2.right, ForceMode2D.Force);

    }

    public bool CanPlayerCrouch()
    {
        throwState.CheckForBall();

        return playerGroundCheck.isGrounded && ( playerInput.isHoldingCrouch || throwState.CanCharge() ); 
    }

}
