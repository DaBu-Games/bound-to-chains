using Unity.VisualScripting;
using UnityEngine;

public class JumpingState : State
{

    [SerializeField] private Transform groundCheck;
    [SerializeField] private IdleState idleState;

    public bool isGrounded {  get; private set; }
    private bool canJump;
    private bool isJumping;

    private float lastPressedJumpTime;
    private float lastOnGroundTime;

    public override void EnterState()
    {
        lastPressedJumpTime = Time.time;
    }

    public override void ExitState()
    {

    }

    public override void FixedUpdateState()
    {

        if ( canJump )
        {

            canJump = false;
            Jump();

        }

        Gravity();
        WhileJumping();
        MovingAir(); 
    }

    public override void UpdateState()
    {
        LastTimeOnGround();
        HandelJump();
    }

    private void Jump()
    {

        isJumping = true;

        float force = playerInput.variables.jumpForce;


        // check if there is any down force if so reset it 
        if ( playerInput.rb2d.linearVelocity.y < 0)
        {

            force -= playerInput.rb2d.linearVelocity.y;

        }

        // add postive y force to the player 
        playerInput.rb2d.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        // Clamp the upward velocity to the maximum jump force
        playerInput.rb2d.linearVelocity = new Vector2( 
            playerInput.rb2d.linearVelocity.x, 
            Mathf.Clamp( 
                playerInput.rb2d.linearVelocity.y, 
                -Mathf.Infinity, playerInput.variables.maxJumpForce 
                ) 
            );

    }

    private void Gravity()
    {

        // check if the player is falling
        if (playerInput.rb2d.linearVelocity.y < 0)
        {

            playerInput.rb2d.gravityScale = playerInput.variables.fallingGravity;

        }
        else
        {

            playerInput.rb2d.gravityScale = playerInput.variables.defaultGravity;

        }

    }

    private void WhileJumping()
    {

        // check if the jump button is let go while jumping
        // if so increase the gravity of the player
        if ( playerInput.rb2d.linearVelocity.y > 0 && isJumping && !playerInput.isHoldingJump )
        {

            playerInput.rb2d.gravityScale = playerInput.variables.jumpCutGravity;

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

    private void LastTimeOnGround()
    {

        Transform player = playerInput.player.transform;

        // Checking if the character is grounded
        isGrounded = Physics2D.OverlapBox( 
            groundCheck.position, 
            new Vector2(player.localScale.x * playerInput.variables.ScaleXtimes, player.localScale.y * playerInput.variables.ScaleYtimes), 
            0, 
            playerInput.variables.whatIsGround
        );

        if (isGrounded)
        {
            lastOnGroundTime = Time.time;
        }

    }

    private void HandelJump()
    {

        // check if the player is grounded or if the player has touched te ground in time of 'variables.leaveGroundBufferTime'
        // and check if the player is not already jumping and if the player has pressed the jump butten in the time of 'variables.jumpInputBufferTime'
        if ( ( isGrounded || Time.time - lastOnGroundTime <= playerInput.variables.leaveGroundBufferTime ) && !isJumping && Time.time - lastPressedJumpTime <= playerInput.variables.jumpInputBufferTime )
        {

            canJump = true;

        }
        // check if the player is grounded has no y velocity and isjumping is equal to true
        else if ( isGrounded && playerInput.rb2d.linearVelocity.y <= 0 && isJumping )
        {

            isJumping = false;
            stateMachine.SwitchState( idleState );

        }

    }

}
