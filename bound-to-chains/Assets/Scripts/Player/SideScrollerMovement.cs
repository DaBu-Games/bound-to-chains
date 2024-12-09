using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class SideScrollerMovement : MonoBehaviour
{

    [SerializeField] private SidescrollerVariables variables;
    [SerializeField] private Rigidbody2D rb2d; 
    [SerializeField] private Transform groundCheck;

    private Vector2 moveInput;
    private bool facingRight = true;
    public bool isGrounded;
    private bool isJumping;
    private bool canJump;
    private bool isHoldingJump;

    private float lastOnGroundTime;
    private float lastPressedJumpTime; 
    private float lastPressedParryTime;

    // Update is called once per frame
    private void Update()
    {

        UpdateTimers();

        LastTimeOnGround();

        HandelJump();
        
    }

    public void Move( InputAction.CallbackContext context )
    {

        moveInput.x = context.ReadValue<Vector2>().x;

        if ( !facingRight && moveInput.x > 0 || facingRight && moveInput.x < 0 )
        {

            FlipCharachter();

        }

    }


    public void Jump( InputAction.CallbackContext context )
    {

        // Check if the button is pressed 
        if( context.performed )
        {

            lastPressedJumpTime = variables.jumpInputBufferTime; 
            lastPressedParryTime = variables.parryBufferTime; 
            isHoldingJump = true; 

        }
        // Check if the button is let go
        else if ( context.canceled )
        {

            isHoldingJump = false; 

        }

    }

    // all the physics-based code / movement  
    private void FixedUpdate()
    {
        
        Run();
        Gravity();
        WhileJumping();

        // Jumping
        if ( canJump )
        {

            canJump = false; 
            Jump();

        }

    }

    private void UpdateTimers()
    {
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;
        lastPressedParryTime -= Time.deltaTime;
    }

    private void LastTimeOnGround()
    {

        // Checking if the character is grounded
        isGrounded = Physics2D.OverlapBox( groundCheck.position, new Vector2( transform.localScale.x * variables.ScaleXtimes, transform.localScale.y * variables.ScaleYtimes ), 0, variables.whatIsGround );

        if ( isGrounded )
        {
            lastOnGroundTime = variables.leaveGroundBufferTime;
        }

    }

    private void HandelJump()
    {

        // check if the player is grounded or if the player has touched te ground in time of 'variables.leaveGroundBufferTime'
        // and check if the player is not already jumping and if the player has pressed the jump butten in the time of 'variables.jumpInputBufferTime'
        if( ( isGrounded || lastOnGroundTime  > 0 ) && !isJumping && lastPressedJumpTime > 0 )
        {

            canJump = true; 

        }
        // check if the player is grounded has no y velocity and isjumping is equal to true
        else if ( isGrounded && rb2d.linearVelocity.y <= 0 && isJumping )
        {

            isJumping = false; 

        }
        
    }

    private void Jump()
    {

        // reset times; 
        lastOnGroundTime = 0; 
        lastPressedJumpTime = 0;
        lastPressedParryTime = 0;

        isJumping = true;

        float force = variables.jumpForce;


        // check if there is any down force if so reset it 
		if ( rb2d.linearVelocity.y < 0 )
        {

            force -= rb2d.linearVelocity.y;

        }

        // add postive y force to the player 
        rb2d.AddForce( Vector2.up * force, ForceMode2D.Impulse);

        // Clamp the upward velocity to the maximum jump force
        rb2d.linearVelocity = new Vector2( rb2d.linearVelocity.x, Mathf.Clamp( rb2d.linearVelocity.y, -Mathf.Infinity, variables.maxJumpForce ) );

    }

    private void WhileJumping()
    {

        // check if the jump button is let go while jumping
        // if so increase the gravity of the player
        if( rb2d.linearVelocity.y > 0 && isJumping && !isHoldingJump )
        {

            rb2d.gravityScale = variables.jumpCutGravity; 

        }

    }

    private void Run()
    {
        // Calculate the target speed based on player input and max movement speed
        float targetSpeed = moveInput.x * variables.maxMoveSpeed;

        float accelRate;

        // Determine if the player is on the ground or not
        if( isGrounded )
        {

            // check if the player is moving if is so accel if not deccel 
            accelRate = (Mathf.Abs(moveInput.x) > 0.01f) ? variables.moveSpeedAccelGround : variables.moveSpeedDeccelGround;

        }
        else 
        {

            accelRate = (Mathf.Abs(moveInput.x) > 0.01f) ? variables.moveSpeedAccelAir : variables.moveSpeedDeccelAir;

        }
        

        // Calculate the difference between the target speed and the current velocity
        float speedDif = targetSpeed - rb2d.linearVelocity.x;

        // Calculate the force to be applied based on the acceleration rate and speed difference
        float movement = speedDif * accelRate;

        // Apply the calculated force to the Rigidbody2D
        rb2d.AddForce( movement * Vector2.right, ForceMode2D.Force );

    }

    private void Gravity()
    {

        // check if the player is falling
        if ( rb2d.linearVelocity.y < 0 )
        {

            rb2d.gravityScale = variables.fallingGravity; 

        }
        else
        {

            rb2d.gravityScale = variables.defaultGravity; 

        }

    }

    private void FlipCharachter()
    {

        // reverse the bool value
        facingRight = !facingRight;

        // rotate the player 180 degerees 
        this.transform.Rotate(0f, 180f, 0f);

    }

}
