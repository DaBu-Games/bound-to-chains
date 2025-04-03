using UnityEngine;

public class CheckForBall : MonoBehaviour
{
    [SerializeField] private LayerMask ballLayerMask;
    [SerializeField] private float raycastRange = 1f;

    // Check if the player is close enough to the metal ball
    public bool RayCastCheck(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction, raycastRange, ballLayerMask);

        bool isInRangeBall = hit.collider != null;

        Debug.DrawRay(this.transform.position, direction * raycastRange, isInRangeBall ? Color.green : Color.red);

        return isInRangeBall;
    }


}
