using UnityEngine;

public class BallBehaviour : MonoBehaviour
{

    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;
    [SerializeField] private float raycastRange;

    private Rigidbody2D rb2d;
    private CircleCollider2D circleCollider2D;
    private HingeJoint2D hingeJoint2D;
    private LayerMask originalExcludeLayers;

    public bool isGrounded {  get; private set; }

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        hingeJoint2D = GetComponent<HingeJoint2D>();

        originalExcludeLayers = circleCollider2D.excludeLayers;
    }

    private void FixedUpdate()
    {
        CheckExcludeLayers();
        CheckGroundedStatus();
    }

    private void CheckGroundedStatus()
    {

        bool oldGroundState = isGrounded;

        isGrounded = Physics2D.Raycast( transform.position, Vector2.down, raycastRange, groundLayerMask ) && circleCollider2D.excludeLayers == originalExcludeLayers;

        if ( oldGroundState != isGrounded )
        {
            SetAngularDrag();
        }

        Debug.DrawRay( transform.position, Vector2.down * raycastRange, isGrounded ? Color.green : Color.red );

    }

    private void SetAngularDrag()
    {
        rb2d.linearDamping = isGrounded ? groundDrag : airDrag;
    }

    private void CheckExcludeLayers()
    {

        if ( rb2d.linearVelocity.y <= 0 && circleCollider2D.excludeLayers != originalExcludeLayers )
        {
            SetExcludeLayers(originalExcludeLayers);
        }

    }

    public void SetAirDrag()
    {
        rb2d.linearDamping = airDrag; 
    }

    public void SetExcludeLayers( LayerMask excludeLayers )
    {
        circleCollider2D.excludeLayers = excludeLayers;
    }

    public float GetForceOnBall()
    {
        return hingeJoint2D.reactionForce.magnitude; 
    }

    public bool IsTransformBellowBall( float transformYCheck, float diffrence )
    {
        return this.transform.position.y - transformYCheck >= diffrence;
    }

    public bool IsTransformAboveBall( float transformYCheck, float diffrence )
    {
        return transformYCheck - this.transform.position.y >= diffrence;
    }

}
