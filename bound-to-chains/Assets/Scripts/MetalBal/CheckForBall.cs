using UnityEngine;

public class CheckForBall : MonoBehaviour
{
    [SerializeField] private LayerMask ballLayerMask;
    [SerializeField] private float raycastRange = 1f;

    // Check if the player is close enough to the metal ball
    public bool BallCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, this.transform.right, raycastRange, ballLayerMask);

        bool isInRangeBall = hit.collider != null;

        Debug.DrawRay(this.transform.position, this.transform.right * raycastRange, isInRangeBall ? Color.green : Color.red);

        return isInRangeBall;

    }


}
