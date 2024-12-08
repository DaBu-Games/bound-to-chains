using UnityEngine;

public class ThrowBall : MonoBehaviour
{

    [SerializeField] private float raycastRange = 50f;  // Max distance to throw the ball
    [SerializeField] private Rigidbody2D ballRb;
    private bool isInRange = false;

    // Update is called once per frame
    void Update()
    {
        CheckForBall();
    }

    private void CheckForBall()
    {
        // make raycast for the player
    }
}
