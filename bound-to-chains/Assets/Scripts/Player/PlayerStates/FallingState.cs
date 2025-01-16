using UnityEngine;

public class FallingState : RisingState
{
    public override void EnterState()
    {
        playerInput.rb2d.gravityScale = playerInput.variables.fallingGravity;
        playerAnimator.Play("FallingAnimation");
    }

    public override void ExitState()
    {
        playerInput.rb2d.gravityScale = playerInput.variables.defaultGravity;
    }

    // Return nothing because jumping will always be false in this state
    protected override void WhileJumping()
    {

    }

    // Return nothing because you are already in the falling state
    protected override void HandleAirborneSpecific()
    {
        
    }

    protected override void LimitVelocity()
    {

        playerInput.rb2d.linearVelocityY = Mathf.Clamp( playerInput.rb2d.linearVelocity.y, -playerInput.variables.maxVelocity, float.MaxValue );
    }

    // Check if the player has no upward velocity and the player is not grounded
    public bool CanPlayerFall()
    {
        return playerInput.rb2d.linearVelocity.y < 0 && !playerGroundCheck.isGrounded;
    }

}
