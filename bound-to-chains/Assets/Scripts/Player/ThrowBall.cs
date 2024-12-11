using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowBall : MonoBehaviour
{

    [SerializeField] private LayerMask ballLayerMask;
    [SerializeField] private GameObject metalBall;
    [SerializeField] private float upWordsScale;
    [SerializeField] private float maxThrowForce = 2000f;
    [SerializeField] private float minThrowForce = 2000f;

    private SideScrollerMovement movementScript;
    private BallBehaviour ballScript;
    private Rigidbody2D ballrb;

    private float chargeStartTime;
    private float raycastRange = 0.75f;
    private bool isInRange = false;
    private bool isCharging = false;

    private float maxChargeTime = 3f;
    private float chargeTime = 2f;
   

    
    private void Start()
    {
        movementScript = this.transform.GetComponent<SideScrollerMovement>();
        ballScript = metalBall.GetComponent<BallBehaviour>();
        ballrb = metalBall.GetComponent <Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckForBall();
    }

    private void Update()
    {
        CheckChargeDuration();
    }

    private void CheckForBall()
    {

        // Check if the player is close enough to the metal ball
        RaycastHit2D hit = Physics2D.Raycast( transform.position, transform.right, raycastRange, ballLayerMask );

        isInRange = hit.collider != null;

        Debug.DrawRay( transform.position, transform.right * raycastRange, isInRange ? Color.green : Color.red );

    }

    public void ThrowInput( InputAction.CallbackContext context )
    {

        if ( CanCharge() && context.performed && !isCharging )
        {
            chargeStartTime = Time.time;
            isCharging = true;
        }
        else if ( context.canceled && isCharging )
        {
            ThrowBallWithForce( Time.time - chargeStartTime );
        }

    }

    private bool CanCharge()
    {
        return ballScript.isGrounded && movementScript.isGrounded && isInRange;
    }

    private void CheckChargeDuration()
    {

        if ( CanCharge() && isCharging ) 
        {

            float chargeDuration = Time.time - chargeStartTime;

            if (chargeDuration >= maxChargeTime)
            {

                ThrowBallWithForce(maxChargeTime);

            }

        }
        else if( chargeStartTime != 0f )
        {
            ResetThrowState();
        }

    }

    private void ThrowBallWithForce( float chargeDuration )
    {

        float chargeFactor = Mathf.Min( chargeDuration / chargeTime, 1f );
        float throwForce = Mathf.Lerp( minThrowForce, maxThrowForce, chargeFactor );

        Debug.Log( throwForce );

        ballrb.AddForce( throwForce * transform.right, ForceMode2D.Impulse );
        ballrb.AddForce( throwForce * upWordsScale * transform.up, ForceMode2D.Impulse);

        ResetThrowState();

    }

    //Reset values
    private void ResetThrowState()
    {
        chargeStartTime = 0f;
        isCharging = false;
        Debug.Log("Reset");
    }

}
