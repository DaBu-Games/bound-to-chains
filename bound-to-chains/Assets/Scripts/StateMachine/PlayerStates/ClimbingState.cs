using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbingState : State
{
    [SerializeField] private RisingState risingState;
    [SerializeField] private FallingState fallingState;
    [SerializeField] private BallBehaviour ballBehaviour;

    [SerializeField] private LayerMask excludeLayers;

    [SerializeField] private float maxMovingClimb = 4f;
    [SerializeField] private float movingAccelerationRate = 4f;
    [SerializeField] private float movingDecelerationRate = 4f;

    [SerializeField] private float climbSpeed = 4f;
    [SerializeField] private float climbEndJump = 20f;
    [SerializeField] private float playerBellowBallDifference = 2f;

    private List<Transform> chainsInHitbox = new List<Transform>(); // Chains in the trigger zone
    private Transform currentChain; // The chain the player is climbing towards

    public override void EnterState()
    {
        playerInput.SetPlayerGravity( 0f ); 
        playerInput.SetExcludeLayers( excludeLayers );

        playerInput.rb2d.linearVelocity = new Vector2(playerInput.rb2d.linearVelocity.x, 0);
    }

    public override void ExitState()
    {
        playerInput.SetPlayerGravity( playerInput.variables.defaultGravity );
    }

    public override void FixedUpdateState()
    {
        UpdateHighestChain(); // Continuously find the highest chain
        ClimbChain();         // Move towards the current chain
        MovingWhileClimbing();
    }

    public override void UpdateState()
    {
        // Exit the climbe state if the player is not holding the climbe button
        if ( !playerInput.isHoldingClimbe || chainsInHitbox.Count == 0 )
        {
            if( risingState.CanPlayerRise() )
            {
                stateMachine.SwitchState( risingState );
            }
            else if( fallingState.CanPlayerFall() ) 
            {
                stateMachine.SwitchState( fallingState );
            }
        }
    }

    private void OnTriggerEnter2D( Collider2D collision )
    {
        // Add chain to the list when it enters the hitbox
        if (collision.CompareTag("Chain"))
        {
            chainsInHitbox.Add(collision.transform);
        }
        else if ( collision.CompareTag("Ball") && stateMachine.GetCurrentState() is ClimbingState )
        {
            FinishClimb();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Remove chain from the list when it exits the hitbox
        if (collision.CompareTag("Chain"))
        {
            chainsInHitbox.Remove(collision.transform);

        }
    }

    private void MovingWhileClimbing()
    {
        // Calculate the target speed based on player input and max movement speed
        float targetSpeed = playerInput.moveInput.x * maxMovingClimb;

        // check if the player is moving if is so accel if not deccel 
        float accelRate = (Mathf.Abs(playerInput.moveInput.x) > 0.01f) ? movingAccelerationRate : movingDecelerationRate;

        // Calculate the difference between the target speed and the current velocity
        float speedDif = targetSpeed - playerInput.rb2d.linearVelocity.x;

        // Calculate the force to be applied based on the acceleration rate and speed difference
        float movement = speedDif * accelRate;

        // Apply the calculated force to the Rigidbody2D
        playerInput.rb2d.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void UpdateHighestChain()
    {
        if (chainsInHitbox.Count == 0)
        {
            currentChain = null; // No chains in hitbox
            return;
        }

        // Find the highest chain above the player
        Transform highestChain = null;
        float highestY = float.MinValue;
        Vector2 playerPosition = playerInput.player.transform.position;

        foreach (Transform chain in chainsInHitbox)
        {
            if (chain.position.y > playerPosition.y && chain.position.y > highestY)
            {
                highestY = chain.position.y;
                highestChain = chain;
            }
        }

        if (highestChain.transform.position.y >= playerInput.player.transform.position.y)
        {
            // Update the current chain to the highest one found
            currentChain = highestChain;
        }
        
    }

    // Get the position of the hinge joined ancher point and convert it to world space
    private Vector2 GetAnchorPoint(HingeJoint2D hingeJoint)
    {
        // Get the anchor point in local space
        Vector2 localAnchor = hingeJoint.anchor;

        // Convert it to world space
        Vector2 worldAnchor = hingeJoint.transform.TransformPoint(localAnchor);

        return worldAnchor;
    }

    private void ClimbChain()
    {
        if ( currentChain == null ) return;

        // Get the target position of where the player is going to 
        Vector2 targetPosition = GetAnchorPoint( currentChain.GetComponent<HingeJoint2D>() );

        Vector2 direction = (targetPosition - (Vector2)playerInput.player.transform.position).normalized;

        playerInput.rb2d.AddForce(direction * climbSpeed, ForceMode2D.Force);

    }

    private void FinishClimb()
    {

        // Slightly dampen movement before applying jump force
        playerInput.rb2d.linearVelocity = new Vector2( playerInput.rb2d.linearVelocity.x, 0 );

        // Apply upward force to finish the climb
        playerInput.rb2d.AddForce( Vector2.up * climbEndJump, ForceMode2D.Impulse );

        // clear the list of chains
        chainsInHitbox.Clear();

        // Switch to the risingState
        stateMachine.SwitchState(risingState);
    }

    // check if the player is holding the climbe button has more then 0 chains and the player is bellow the ball
    public bool CanPlayerClimbe()
    {
        return playerInput.isHoldingClimbe && chainsInHitbox.Count > 0 && ballBehaviour.IsTransformBellowBall( playerInput.player.transform.position.y, playerBellowBallDifference ) && ballBehaviour.isGrounded;
    }
}

