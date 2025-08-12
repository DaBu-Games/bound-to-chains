using UnityEngine;

public class BoltCutters : MonoBehaviour
{

    private void OnCollisionEnter2D( Collision2D collision )
    {
        if( collision.gameObject.CompareTag( "Player" ) )
        {
            collision.gameObject.GetComponent<Player>().UnChainePlayer();
            this.gameObject.SetActive( false );
            collision.gameObject.GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = true;
        }
    }
}
