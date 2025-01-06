using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{

    public SidescrollerVariables variables; 
    public GameObject player;

    public Vector2 moveInput { get; private set; }
    public Rigidbody2D rb2d { get; private set; }
    public bool isHoldingJump {  get; private set; }
    public float lastPressedJumpTime { get; private set; }
    public bool isHoldingCharge { get; private set; }
    public bool isHoldingClimbe { get; private set; }

    private bool facingRight = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = player.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        // Get the move input and put it in a vector 2 
        moveInput = context.ReadValue<Vector2>();

        if ( !facingRight && moveInput.x > 0 || facingRight && moveInput.x < 0 )
        {

            FlipCharachter();

        }

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


    private void FlipCharachter()
    {

        // reverse the bool value
        facingRight = !facingRight;

        // rotate the player 180 degerees 
        player.transform.Rotate( 0f, 180f, 0f );

    }
}
