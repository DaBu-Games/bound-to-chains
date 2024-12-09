using UnityEngine;

public class BallBehaviour : MonoBehaviour
{

    [SerializeField] private LayerMask whatIsGround;
    private CircleCollider2D circleCollider;
    public bool isGrounded {  get; private set; }

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        CheckGroundedStatus(); 
    }

    private void CheckGroundedStatus()
    {

        isGrounded = Physics2D.OverlapCircle(transform.position, circleCollider.radius, whatIsGround);

    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the overlap circle in the editor
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, circleCollider != null ? circleCollider.radius : 0f);
    }

}
