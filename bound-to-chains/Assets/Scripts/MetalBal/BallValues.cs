using UnityEngine;

[CreateAssetMenu(fileName = "PlayerValues", menuName = "Player/Player values")]
public class BallValues : ScriptableObject
{
    [SerializeField] private CheckForGround playerGroundCheck;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;
    [SerializeField] private float raycastRange;
    [SerializeField] private float maxVelocity;

    [Header("LayerMask")]
    [SerializeField] private LayerMask excludeLayersThrow;

    [Header("Forces")]
    [SerializeField] private float minForceOnHinge = 40f;
    [SerializeField] private float minForceOnBall = 40f;

    [Header("Range")]
    [SerializeField] private float playerAboveBallDifference;
    [SerializeField] public float playerBellowBallDifference;
    [SerializeField] public float playerFromBallDifference;
}
