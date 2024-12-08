using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    [SerializeField] private float pushForce = 5f;

    private void OnCollisionEnter2D( Collision2D collision )
    {

       // Check if the ball collides on top of the player
        if ( collision.gameObject.CompareTag("Player") && IsBallOnTopOfPlayer( collision.transform.position ) )
        {
            // Push the player back when the ball collides with the player
            PushPlayerBack( collision );
        }
    }


    private bool IsBallOnTopOfPlayer( Vector2 playerPosition )
    {
        Vector2 ballPosition = transform.position;

        // Check if the ball is within a small distance above the player
        if ( ballPosition.y > playerPosition.y && Mathf.Abs( ballPosition.x - playerPosition.x ) < 1f )
        {
            return true;
        }

        return false;
    }


    void PushPlayerBack(Collision2D collision)
    {
        // Get the direction from the ball to the player
        Vector2 direction = collision.transform.position - transform.position;

        // Normalize the direction so the force is always applied in the same manner
        direction.Normalize();

        // Get the player's Rigidbody2D component and apply the force
        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            // Apply the force to the player, pushing them away from the ball
            playerRb.AddForce( direction * pushForce, ForceMode2D.Impulse );
        }
    }

}
