using UnityEngine;

public class BallBehaviour : MonoBehaviour
{

    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;
    [SerializeField] private float raycastRange;

    private Rigidbody2D rb2d;
    public bool isGrounded {  get; private set; }

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        CheckGroundedStatus();
    }

    private void CheckGroundedStatus()
    {

        bool oldGroundState = isGrounded;

        isGrounded = Physics2D.Raycast( transform.position, Vector2.down, raycastRange, groundLayerMask );

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

    public bool IsTransformBellowBall( float transformYCheck, float diffrence )
    {
        return this.transform.position.y - transformYCheck >= diffrence;
    }

}
