using Unity.VisualScripting;
using UnityEngine;

public class JumpingState : State
{
    [SerializeField] private IdleState idleState;
    [SerializeField] private CheckForGround playerGroundCheck;

    private bool canJump;
    public bool isJumping {  get; private set; }

    public override void EnterState()
    {
        canJump = true; 
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

        //Gravity();
        //WhileJumping();
        //MovingAir(); 
    }

    public override void UpdateState()
    {
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

    //private void Gravity()
    //{

    //    // check if the player is falling
    //    if (playerInput.rb2d.linearVelocity.y < 0)
    //    {

    //        playerInput.rb2d.gravityScale = playerInput.variables.fallingGravity;

    //    }
    //    else
    //    {

    //        playerInput.rb2d.gravityScale = playerInput.variables.defaultGravity;

    //    }

    //}

    //private void WhileJumping()
    //{

    //    // check if the jump button is let go while jumping
    //    // if so increase the gravity of the player
    //    if ( playerInput.rb2d.linearVelocity.y > 0 && isJumping && !playerInput.isHoldingJump )
    //    {

    //        playerInput.rb2d.gravityScale = playerInput.variables.jumpCutGravity;

    //    }

    //}

    //private void MovingAir()
    //{
    //    // Calculate the target speed based on player input and max movement speed
    //    float targetSpeed = playerInput.moveInput.x * playerInput.variables.maxMoveSpeed;

    //    // check if the player is moving if is so accel if not deccel 
    //    float accelRate = (Mathf.Abs(playerInput.moveInput.x) > 0.01f) ? playerInput.variables.moveSpeedAccelAir : playerInput.variables.moveSpeedDeccelAir;

    //    // Calculate the difference between the target speed and the current velocity
    //    float speedDif = targetSpeed - playerInput.rb2d.linearVelocity.x;

    //    // Calculate the force to be applied based on the acceleration rate and speed difference
    //    float movement = speedDif * accelRate;

    //    // Apply the calculated force to the Rigidbody2D
    //    playerInput.rb2d.AddForce(movement * Vector2.right, ForceMode2D.Force);
    //}

    private bool CanBufferJump()
    {
        return Time.time - playerGroundCheck.lastOnGroundTime <= playerInput.variables.leaveGroundBufferTime;
    }

    public bool IsJumpBufferd()
    {
        return Time.time - playerInput.lastPressedJumpTime <= playerInput.variables.jumpInputBufferTime && Time.time > playerInput.variables.jumpInputBufferTime;
    }

    private void HandelJump()
    {

        // check if the player is grounded or if the player has touched te ground in time of 'variables.leaveGroundBufferTime'
        // and check if the player is not already jumping and if the player has pressed the jump butten in the time of 'variables.jumpInputBufferTime'
        if ( ( playerGroundCheck.isGrounded || CanBufferJump() ) && !isJumping && IsJumpBufferd() )
        {
            canJump = true;
        }
        // check if the player is grounded has no y velocity and isjumping is equal to true
        else if ( playerGroundCheck.isGrounded && playerInput.rb2d.linearVelocity.y <= 0 && isJumping )
        {

            isJumping = false;
            stateMachine.SwitchState( idleState );
            
        }

    }

}
