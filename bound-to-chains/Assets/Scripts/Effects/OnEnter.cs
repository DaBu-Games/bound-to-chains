using UnityEngine;

public class OnEnter : MonoBehaviour
{

    [SerializeField] FadeIn screen;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.gameObject.CompareTag( "Player" ) )
        {
            screen.StartFadeIn();
        }
    }
}
