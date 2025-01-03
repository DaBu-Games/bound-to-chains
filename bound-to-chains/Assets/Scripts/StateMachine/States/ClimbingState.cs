using UnityEngine;

public class ClimbingState : State
{
    [SerializeField] private IdleState idleState;

    [SerializeField] private float climbSpeed = 2f;
    [SerializeField] private LayerMask chainLayerMask; 
    private Transform currentChain;

    public override void EnterState()
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override void FixedUpdateState()
    {
        GetNextChain();
        ClimbeChain(); 
    }

    public override void UpdateState()
    {
        // if the player is not holding the climbe button any more exit the clime state
        if( !playerInput.isHoldingClimbe)
        {
            stateMachine.SwitchState(idleState); 
        }

    }

    private void GetNextChain()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerInput.player.transform.position, Vector2.up, Mathf.Infinity, chainLayerMask);
        if ( hit.collider != null )
        {
            currentChain = hit.transform;
        }
    }

    private void ClimbeChain()
    {
        if ( currentChain == null ) return;

        // Move the player towards the target chain segment
        playerInput.player.transform.position = Vector2.MoveTowards( playerInput.player.transform.position, currentChain.position, climbSpeed * Time.deltaTime );

        // Check if the player reached the target
        if ( Vector2.Distance( playerInput.player.transform.position, currentChain.position) < 0.1f )
        {
            // Stop climbing or update the current segment logic if necessary
            currentChain = null;
        }
    }
}
