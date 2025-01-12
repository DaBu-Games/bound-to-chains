using UnityEngine;
using System.Collections;
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

        chargeStartTime = Time.time;
        playerAnimator.Play("ChargeAnimation");

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

    public bool CheckForBall()
    {

        // Check if the player is close enough to the metal ball
        RaycastHit2D hit = Physics2D.Raycast( this.transform.position, this.transform.right, raycastRange, ballLayerMask );

        isInRange = hit.collider != null;

        Debug.DrawRay( this.transform.position, this.transform.right * raycastRange, isInRange ? Color.green : Color.red );

        return isInRange;

    }

    private void CheckChargeDuration()
    {

        if ( CanCharge() && chargeStartTime != 0f )
        {

            // Calculate how long the player has been charging
            float chargeDuration = Time.time - chargeStartTime;

            // if the player has let go the charge button
            if ( !playerInput.isHoldingCharge )
            {

                ThrowBallWithForce( chargeDuration );

            }
            // Set the max chargeduration to maxChargeTime
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

    // check if the ball and player are grounded and the ball is inrange of the player
    private bool CanCharge()
    {
        return ballBehaviour.isGrounded && playerGroundCheck.isGrounded && isInRange;
    }

    private void ThrowBallWithForce(float chargeDuration)
    {

        playerAnimator.Play( "ThrowingAnimation" );

        // Calculate how hard the player can throw the ball
        float chargeFactor = Mathf.Min(chargeDuration / chargeTime, 1f);
        float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargeFactor);

        ballBehaviour.SetAirDrag();

        ballrb2d.AddForce(throwForce * playerInput.player.transform.right, ForceMode2D.Impulse);
        ballrb2d.AddForce(throwForce * upWordsScale * playerInput.player.transform.up, ForceMode2D.Impulse);

        StartCoroutine( ResetThrowState() );

    }

   
    // Reset the ChargeStartTime and switch state
    private IEnumerator ResetThrowState()
    {

        chargeStartTime = 0f;

        yield return new WaitForSeconds( 0.25f );

        stateMachine.SwitchState( idleState );

    }

    // Check if the player can charge and is holding the charge button
    public bool CanPlayerThrow()
    {
        CheckForBall();

        return CanCharge() && playerInput.isHoldingCharge; 
    }

}
