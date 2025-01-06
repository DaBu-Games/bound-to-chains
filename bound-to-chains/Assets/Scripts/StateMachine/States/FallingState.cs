using UnityEngine;

public class FallingState : RisingState
{
    public override void EnterState()
    {
        playerInput.rb2d.gravityScale = playerInput.variables.fallingGravity;
    }

    public override void ExitState()
    {
        playerInput.rb2d.gravityScale = playerInput.variables.defaultGravity;
    }

    protected override void WhileJumping()
    {

    }

    protected override void HandleAirborneSpecific()
    {
        
    }

}
