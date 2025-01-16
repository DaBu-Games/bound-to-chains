using UnityEngine;

public class CameraTrigger : MonoBehaviour
{

    private Camera objectCamera;

    private void Awake()
    {
        objectCamera = GetComponent<Camera>();
        objectCamera.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if ( collision.CompareTag("Player") )
        {
            objectCamera.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if ( collision.CompareTag("Player") )
        {
            objectCamera.enabled = false;
        }
    }

}
