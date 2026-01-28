using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public Vector2 moveInput { get; private set; }
    public bool isHoldingJump {  get; private set; }
    public float lastPressedJumpTime { get; private set; }
    public bool isHoldingCharge { get; private set; }
    public bool isHoldingClimbe { get; private set; }
    public bool isHoldingCrouch { get; private set; }

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

    public void SetJumpTime(float time)
    {
        lastPressedJumpTime = lastPressedJumpTime - time;
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
}
