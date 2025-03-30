using UnityEngine;
using UnityEngine.InputSystem;

public class BallBehaviour : MonoBehaviour
{
    [SerializeField] private BallValues ballValues;

    private Rigidbody2D rb2d;
    private CircleCollider2D circleCollider2D;
    private LayerMask originalExcludeLayers;

    public bool isGrounded {  get; private set; }

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();

        originalExcludeLayers = circleCollider2D.excludeLayers;
    }

    private void FixedUpdate()
    {
        CheckExcludeLayers();
        CheckGroundedStatus();
        LimitVelocity(); 
    }
    private void LimitVelocity()
    {
        rb2d.linearVelocityY = Mathf.Clamp( rb2d.linearVelocity.y, -ballValues.maxVelocity, float.MaxValue);
    }


    private void CheckGroundedStatus()
    {

        bool oldGroundState = isGrounded;

        isGrounded = Physics2D.Raycast( transform.position, Vector2.down, ballValues.raycastRange, ballValues.groundLayerMask) && circleCollider2D.excludeLayers == originalExcludeLayers;

        if ( oldGroundState != isGrounded )
        {
            SetAngularDrag();
        }

        Debug.DrawRay( transform.position, Vector2.down * ballValues.raycastRange, isGrounded ? Color.green : Color.red );

    }

    private void SetAngularDrag()
    {
        rb2d.linearDamping = isGrounded ? ballValues.groundDrag : ballValues.airDrag;
    }

    private void CheckExcludeLayers()
    {

        if ( rb2d.linearVelocity.y <= 0 && originalExcludeLayers != circleCollider2D.excludeLayers )
        {
            // Check if the ball is still inside a tilemap or platform
            bool isStillInsideTilemap = Physics2D.OverlapCircle(transform.position, ballValues.raycastRange, LayerMask.GetMask("Platform") );

            if (!isStillInsideTilemap)
            {
                SetExcludeLayers(originalExcludeLayers);
            }
        }

    }

    public void SetAirDrag()
    {
        rb2d.linearDamping = ballValues.airDrag; 
    }

    public void SetExcludeLayers( LayerMask excludeLayers )
    {
        circleCollider2D.excludeLayers = excludeLayers;
    }

    public bool IsTransformBellowBall( float transformYCheck, float diffrence )
    {
        return this.transform.position.y - transformYCheck >= diffrence;
    }

    public bool IsTransformAboveBall( float transformYCheck, float diffrence )
    {
        return transformYCheck - this.transform.position.y >= diffrence;
    }

    public bool CheckDistanceFromBall(Vector2 objectPosition, float diffrence)
    {
        return Vector2.Distance(this.transform.position, objectPosition) >= diffrence;
    }
}
