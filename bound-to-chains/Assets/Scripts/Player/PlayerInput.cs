using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private SidescrollerVariables chained;
    [SerializeField] private SidescrollerVariables unchained;
    [SerializeField] private HingeJoint2D lastHinge;

    public SidescrollerVariables variables { get; private set; }
    public GameObject player;

    public Vector2 moveInput { get; private set; }
    public Rigidbody2D rb2d { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public BoxCollider2D boxCollider2D { get; private set; }
    public bool isHoldingJump {  get; private set; }
    public float lastPressedJumpTime { get; private set; }
    public bool isHoldingCharge { get; private set; }
    public bool isHoldingClimbe { get; private set; }
    public bool isHoldingCrouch { get; private set; }

    private LayerMask originalExcludeLayers;
    private float startingMass;
    private float startingLinearDamping;
    private float startingAngularDamping;

    public bool facingRight { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        variables = chained; 
        rb2d = player.GetComponent<Rigidbody2D>();

        spriteRenderer = player.GetComponent<SpriteRenderer>();

        boxCollider2D = player.GetComponent<BoxCollider2D>();

        originalExcludeLayers = boxCollider2D.excludeLayers;

        // Set the default gravity of the player
        rb2d.gravityScale = variables.defaultGravity;
        startingMass = rb2d.mass;

        startingLinearDamping = rb2d.linearDamping;
        startingAngularDamping = rb2d.angularDamping;

        facingRight = true;
    }

    public void SetPlayerGravity( float gravity )
    {
        rb2d.gravityScale = gravity;
    }

    public void SetPlayerMass(float mass)
    {
        rb2d.mass = mass;
    }

    public void ResetPlayerMass()
    {
        SetPlayerMass( startingMass );
    }

    public void SetPlayerDamping( float damping)
    {
        rb2d.linearDamping = damping;
        rb2d.angularDamping = damping;
    }

    public void ResetPlayerDamping()
    {
        rb2d.linearDamping = startingLinearDamping;
        rb2d.angularDamping = startingAngularDamping;
    }

    public void SetExcludeLayers( LayerMask excludeLayers )
    {
        boxCollider2D.excludeLayers = excludeLayers;
    }

    public void ResetExludeLayers()
    {
        SetExcludeLayers( originalExcludeLayers );
    }

    public bool HasNoExcludeLayers()
    {
        return boxCollider2D.excludeLayers == originalExcludeLayers;
    }

    public void UnChainePlayer()
    {
        variables = unchained; 
        lastHinge.connectedBody = null;
        lastHinge.useConnectedAnchor = false;
        SetPlayerGravity( variables.defaultGravity );
    }

    public void Move(InputAction.CallbackContext context)
    {
        // Get the move input and put it in a vector 2 
        moveInput = context.ReadValue<Vector2>();

    }
    public void Jump( InputAction.CallbackContext context ) 
    {

        // Check if the button is pressed 
        if ( context.started )
        {

            isHoldingJump = true;
            lastPressedJumpTime = Time.time;

        }
        // Check if the button is let go
        else if (context.canceled)
        {

            isHoldingJump = false;

        }

    }

    public void ResetJumpTime()
    {
        lastPressedJumpTime = lastPressedJumpTime - variables.jumpInputBufferTime; 
    }

    public void ThrowInput( InputAction.CallbackContext context )
    {

        // Check if the button is pressed 
        if ( context.performed )
        {

            isHoldingCharge = true; 

        }
        // Check if the button is let go
        else if ( context.canceled )
        {
            isHoldingCharge = false;

        }

    }
    public void Climbing( InputAction.CallbackContext context )
    {

        // Check if the button is pressed 
        if ( context.performed )
        {

            isHoldingClimbe = true;

        }
        // Check if the button is let go
        else if ( context.canceled )
        {

            isHoldingClimbe = false;

        }

    }

    public void Crouching(InputAction.CallbackContext context)
    {

        // Check if the button is pressed 
        if ( context.performed )
        {

            isHoldingCrouch = true;

        }
        // Check if the button is let go
        else if ( context.canceled )
        {

            isHoldingCrouch = false;

        }

    }


    public void FlipCharachter()
    {

        // reverse the bool value
        facingRight = !facingRight;

        // rotate the player 180 degerees 
        player.transform.Rotate( 0f, 180f, 0f );

    }

    public void ResetCharachterRotation()
    {

        facingRight = true; 

        player.transform.rotation = Quaternion.identity;
    }
}
