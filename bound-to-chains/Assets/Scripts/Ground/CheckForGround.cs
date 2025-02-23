using UnityEngine;

public class CheckForGround : MonoBehaviour
{

    [SerializeField] private Transform groundCheck;
    [SerializeField] private PlayerValues variables;

    public bool isGrounded { get; private set; }
    public float lastOnGroundTime { get; private set; }

    private void Update()
    {
        LastTimeOnGround();
    }

    // Check if object is on the ground or not
    private void LastTimeOnGround()
    {

        // Checking if the character is grounded
        isGrounded = Physics2D.OverlapBox(
        groundCheck.position,
        new Vector2( groundCheck.localScale.x * variables.ScaleXtimes, groundCheck.localScale.y * variables.ScaleYtimes ),
        0,
        variables.whatIsGround
        );

        // if grounded is true update the lastOngroudTime to the current time 
        if (isGrounded)
        {
            lastOnGroundTime = Time.time;
        }

    }
}
