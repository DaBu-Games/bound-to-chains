using UnityEngine;

public class HangingState : State
{

    [SerializeField] private HingeJoint2D lastHinge;
    [SerializeField] private BallBehaviour ballBehaviour;
    [SerializeField] private CheckForGround playerGroundCheck;

    [SerializeField] private ClimbingState climbingState;
    [SerializeField] private IdleState idleState;
    [SerializeField] private JumpingState jumpingState;
    [SerializeField] private WalkingState walkingState;
    [SerializeField] private RisingState risingState;
    [SerializeField] private FallingState fallingState;

    [SerializeField] private float playerBellowBallDifference = 2f;
    [SerializeField] private float minForceOnHinge = 40f;
    [SerializeField] private float decelHanging = 1f; 

    public override void EnterState()
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override void FixedUpdateState()
    {

        DecelPlayer();

    }

    public override void UpdateState()
    {
         if ( climbingState.CanPlayerClimbe() )
        {
            stateMachine.SwitchState(climbingState);
            return;
        }

        if ( !CanPlayerHang() ) 
        {
            if ( risingState.CanPlayerRise())
            {
                stateMachine.SwitchState(risingState);
            }
            else if ( fallingState.CanPlayerFall())
            {
                stateMachine.SwitchState(fallingState);
            }
            else if (idleState.CanPlayerIdle())
            {
                stateMachine.SwitchState(idleState);
            }
            else if (jumpingState.CanPlayerJump())
            {
                stateMachine.SwitchState(jumpingState);
            }
            else if (walkingState.CanPlayerWalk())
            {
                stateMachine.SwitchState(walkingState);
            }
        }
    }
    private void DecelPlayer()
    {
        // Calculate the target speed based on player input and max movement speed
        float targetSpeed = playerInput.moveInput.x * playerInput.variables.maxMoveSpeed;

        // Calculate the difference between the target speed and the current velocity
        float speedDif = targetSpeed - playerInput.rb2d.linearVelocity.x;

        // Calculate the deceleration force to apply
        float movement = speedDif * decelHanging;

        // Apply the calculated force to decelerate the Rigidbody2D
        playerInput.rb2d.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private bool IsPlayerHanging()
    {

        return lastHinge.reactionForce.magnitude > minForceOnHinge;

    }

    public bool CanPlayerHang()
    {
        return ballBehaviour.IsTransformBellowBall( playerInput.player.transform.position.y, playerBellowBallDifference ) && !playerGroundCheck.isGrounded && IsPlayerHanging(); 
    }
}
