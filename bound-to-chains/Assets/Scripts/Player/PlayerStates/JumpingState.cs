using Unity.VisualScripting;
using UnityEngine;

public class JumpingState : State
{
    [SerializeField] private RisingState risingState;
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

    }

    public override void UpdateState()
    {

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

        playerInput.ResetJumpTime();

        stateMachine.SwitchState( risingState ); 

    }

    private bool CanBufferJump()
    {
        return Time.time - playerGroundCheck.lastOnGroundTime <= playerInput.variables.leaveGroundBufferTime;
    }

    public bool IsJumpBufferd()
    {
        return Time.time - playerInput.lastPressedJumpTime <= playerInput.variables.jumpInputBufferTime && Time.time > playerInput.variables.jumpInputBufferTime;
    }

    public void CancelJump()
    {
        isJumping = false;
    }

    // Check if the player is grounded or if the player has touched te ground in time of 'variables.leaveGroundBufferTime'
    // And check if the player is not already jumping and if the player has pressed the jump butten in the time of 'variables.jumpInputBufferTime'
    public bool CanPlayerJump()
    {

        return (playerGroundCheck.isGrounded || CanBufferJump()) && !isJumping && IsJumpBufferd();

    }

}
