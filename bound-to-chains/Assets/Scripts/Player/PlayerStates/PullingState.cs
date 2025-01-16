using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PullingState : State
{
    [SerializeField] private LayerMask ballLayerMask;
    [SerializeField] private BallBehaviour ballBehaviour;
    [SerializeField] private Rigidbody2D ballrb2d;
    [SerializeField] private float upWordsScale;
    [SerializeField] private float maxThrowForce = 2000f;
    [SerializeField] private float minThrowForce = 2000f;
    [SerializeField] private float playerAboveBallDifference = 0.55f;

    [SerializeField] private LayerMask ballExcludeLayers;
    [SerializeField] private float minForceOnBall = 40f;

    [SerializeField] private IdleState idleState;
    [SerializeField] private ThrowState throwState;
    [SerializeField] private CheckForGround playerGroundCheck;

    [SerializeField] private Color targetColor;

    private float chargeStartTime;

    [SerializeField] private float crouchingMass = 5f;
    [SerializeField] private float Damping = 2f;

    [SerializeField] private float maxChargeTime = 3f;
    [SerializeField] private float chargeTime = 2f;


    public override void EnterState()
    {
        playerAnimator.Play("ChargeAnimation");
        playerInput.SetPlayerMass(crouchingMass);
        playerInput.SetPlayerDamping(Damping);
        chargeStartTime = Time.time;

    }

    public override void ExitState()
    {
        playerInput.ResetPlayerMass();
        playerInput.ResetPlayerDamping();
    }

    public override void FixedUpdateState()
    {
        throwState.CheckForBall();
    }

    public override void UpdateState()
    {
        CheckChargeDuration();
    }

    private void CheckChargeDuration()
    {

        if ( CanCharge() && chargeStartTime != 0f )
        {

            // Calculate how long the player has been charging
            float chargeDuration = Time.time - chargeStartTime;

            // Calculate the charge ratio
            float chargeRatio = chargeDuration / chargeTime;
            chargeRatio = Mathf.Clamp01(chargeRatio); // Ensure it's between 0 and 1


            Color newColor = Color.Lerp( playerInput.spriteRenderer.material.color, targetColor, chargeRatio);

            playerInput.spriteRenderer.color = newColor;

            // if the player has let go the charge button
            if (!playerInput.isHoldingCharge)
            {

                PullBallWithForce(chargeDuration);

            }
            // Set the max chargeduration to maxChargeTime
            else if (chargeDuration >= maxChargeTime)
            {

                PullBallWithForce(maxChargeTime);

            }

        }
        else
        {
            StartCoroutine( ResetThrowState() );
        }

    }

    private void PullBallWithForce( float chargeDuration )
    {

        playerAnimator.Play("ThrowingAnimation");

        Vector2 directionToPlayer = ( playerInput.player.transform.position - ballrb2d.transform.position ).normalized;

        // Calculate how hard the player can throw the ball
        float chargeFactor = Mathf.Min(chargeDuration / chargeTime, 1f);
        float pullForce = Mathf.Lerp( minThrowForce, maxThrowForce, chargeFactor );

        ballBehaviour.SetAirDrag();

        ballrb2d.AddForce( pullForce * directionToPlayer, ForceMode2D.Impulse );
        ballrb2d.AddForce( pullForce * upWordsScale * Vector2.up, ForceMode2D.Impulse );

        if ( ballBehaviour.IsTransformAboveBall( playerInput.player.transform.position.y, playerAboveBallDifference ) )
        {
            ballBehaviour.SetExcludeLayers(ballExcludeLayers);
        }

        StartCoroutine(ResetThrowState());

    }


    // Reset the ChargeStartTime and switch state
    private IEnumerator ResetThrowState()
    {

        chargeStartTime = 0f;

        playerInput.spriteRenderer.color = playerInput.spriteRenderer.material.color;

        yield return new WaitForSeconds(0.25f);

        stateMachine.SwitchState(idleState);

    }

    // check if the player is grounded and the ball is not inrange of the player
    private bool CanCharge()
    {
        return playerGroundCheck.isGrounded && !throwState.isInRange ;
    }

    // Check if the player can charge and is holding the charge button
    public bool CanPlayerPull()
    {
        throwState.CheckForBall();

        return CanCharge() && playerInput.isHoldingCharge && ( ballBehaviour.GetForceOnBall() > minForceOnBall || ballBehaviour.isGrounded );
    }
}
