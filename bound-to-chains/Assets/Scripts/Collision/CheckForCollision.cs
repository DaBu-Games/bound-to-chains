using UnityEngine;

public class CheckForCollision : MonoBehaviour
{
    private LayerMask collisionLayers;
    private int collidingCount = 0; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & collisionLayers) != 0)
            collidingCount++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & collisionLayers) != 0)
            collidingCount--;
    }

    public void SetCollisionLayers(LayerMask collisionLayers)
    {
        this.collisionLayers = collisionLayers;
    }

    public bool IsColliding()
    {
        return collidingCount > 0; 
    }
}
