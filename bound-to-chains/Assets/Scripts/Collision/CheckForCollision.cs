using UnityEngine;

public class CheckForCollision : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsCollision; 
    public bool isColliding {  get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsCollision) != 0)
            isColliding = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsCollision) != 0)
            isColliding = false;
    }
}
