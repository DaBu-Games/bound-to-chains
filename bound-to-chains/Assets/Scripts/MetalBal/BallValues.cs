using UnityEngine;

[CreateAssetMenu(fileName = "BallValues", menuName = "Ball/Ball values")]
public class BallValues : ScriptableObject
{
    [Header("LayerMask")]
    [SerializeField] public LayerMask groundLayerMask;

    [Header("Velocity")]
    [SerializeField] public float maxVelocity;

    [Header("Drag")]
    [SerializeField] public float groundDrag;
    [SerializeField] public float airDrag;

    [Header("LayerMask")]
    [SerializeField] public LayerMask ballExcludeLayers;

    [Header("Forces")]
    [SerializeField] public float minForceOnHinge;
    [SerializeField] public float minForceOnBall;

    [Header("Range")]
    [SerializeField] public float playerAboveBallDifference;
    [SerializeField] public float playerBellowBallDifference;
    [SerializeField] public float playerFromBallDifference;
    [SerializeField] public float raycastRange;
}
