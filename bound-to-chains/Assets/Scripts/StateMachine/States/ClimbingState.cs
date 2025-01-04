using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbingState : State
{
    [SerializeField] private IdleState idleState;
    [SerializeField] private float climbSpeed = 4;
    [SerializeField] private float playerBellowBallDifference = 2f;
    [SerializeField] private Transform ballTransform;

    private List<Transform> chainsInHitbox = new List<Transform>(); // Chains in the trigger zone
    private Transform currentChain; // The chain the player is climbing towards

    public override void EnterState()
    {
        playerInput.rb2d.gravityScale = 0; 
    }

    public override void ExitState()
    {
        playerInput.rb2d.gravityScale = playerInput.variables.defaultGravity;
    }

    public override void FixedUpdateState()
    {
        UpdateHighestChain(); // Continuously find the highest chain
        ClimbChain();         // Move towards the current chain
    }

    public override void UpdateState()
    {
        // Exit climbing state if the climb button is released or there's no chain to climb
        if ( !playerInput.isHoldingClimbe || chainsInHitbox.Count == 0 || !IsPlayerBelowBall() )
        {
            stateMachine.SwitchState(idleState);
        }
    }

    public bool IsPlayerBelowBall()
    {
        Debug.Log(ballTransform.position.y - playerInput.player.transform.position.y); 
        return ballTransform.position.y - playerInput.player.transform.position.y >= playerBellowBallDifference;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Add chain to the list when it enters the hitbox
        if (collision.CompareTag("Chain"))
        {
            chainsInHitbox.Add(collision.transform);
        }
        else if (collision.CompareTag("Ball"))
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
        if (currentChain == null) return;

        // Get the position of the hinge joined ancher point and convert it to world space
        Vector2 targetPosition = GetAnchorPoint( currentChain.GetComponent<HingeJoint2D>() );

        Vector2 direction = (targetPosition - (Vector2)playerInput.player.transform.position).normalized;

        playerInput.rb2d.AddForce(direction * climbSpeed, ForceMode2D.Force);

    }

    private void FinishClimb()
    {
        // Apply upward force to finish the climb
        playerInput.rb2d.AddForce(Vector2.up * playerInput.variables.jumpForce, ForceMode2D.Impulse);

        chainsInHitbox.Clear();

        // Switch to idle state
        stateMachine.SwitchState(idleState);
    }
}

