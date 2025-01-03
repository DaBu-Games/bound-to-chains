using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowState : State
{
    [SerializeField] private LayerMask ballLayerMask;
    [SerializeField] private BallBehaviour ballBehaviour;
    [SerializeField] private Rigidbody2D ballrb2d; 
    [SerializeField] private float upWordsScale;
    [SerializeField] private float maxThrowForce = 2000f;
    [SerializeField] private float minThrowForce = 2000f;
    [SerializeField] private float raycastRange = 0.75f;

    [SerializeField] private IdleState idleState;
    [SerializeField] private CheckForGround playerGroundCheck;

    private float chargeStartTime;
    private bool isInRange = false;

    private float maxChargeTime = 3f;
    private float chargeTime = 2f;


    public override void EnterState()
    {

        // check if the player is not in the range of the ball if so exit state
        if ( !CheckForBall() )
        {
            ResetThrowState();
        }
        else
        {
            chargeStartTime = Time.time;
        }

    }

    public override void ExitState()
    {
        
    }

    public override void FixedUpdateState()
    {
        CheckForBall(); 
    }

    public override void UpdateState()
    {
        CheckChargeDuration();
    }

    private bool CheckForBall()
    {

        // Check if the player is close enough to the metal ball
        RaycastHit2D hit = Physics2D.Raycast( playerInput.player.transform.position, playerInput.player.transform.right, raycastRange, ballLayerMask );

        isInRange = hit.collider != null;

        Debug.DrawRay( playerInput.player.transform.position, playerInput.player.transform.right * raycastRange, isInRange ? Color.green : Color.red );

        return isInRange;

    }

    private void CheckChargeDuration()
    {

        if ( CanCharge() )
        {

            float chargeDuration = Time.time - chargeStartTime;

            if ( !playerInput.isHoldingCharge )
            {

                ThrowBallWithForce( chargeDuration );

            }
            else if ( chargeDuration >= maxChargeTime )
            {
                ThrowBallWithForce( maxChargeTime );
            }

        }
        else
        {
            ResetThrowState();
        }

    }
    private bool CanCharge()
    {
        return ballBehaviour.isGrounded && playerGroundCheck.isGrounded && isInRange;
    }

    private void ThrowBallWithForce(float chargeDuration)
    {

        float chargeFactor = Mathf.Min(chargeDuration / chargeTime, 1f);
        float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargeFactor);

        Debug.Log(throwForce);

        ballrb2d.AddForce(throwForce * playerInput.player.transform.right, ForceMode2D.Impulse);
        ballrb2d.AddForce(throwForce * upWordsScale * playerInput.player.transform.up, ForceMode2D.Impulse);

        ResetThrowState();

    }

   
    private void ResetThrowState()
    {
        chargeStartTime = 0f;
        Debug.Log("reset?");
        stateMachine.SwitchState( idleState );

    }

}
