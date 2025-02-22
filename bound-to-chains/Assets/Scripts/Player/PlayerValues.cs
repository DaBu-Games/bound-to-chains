using UnityEngine;


[CreateAssetMenu(fileName = "PlayerValues", menuName = "Player/Player values")]
public class PlayerValues : ScriptableObject
{
    [Header("Hitbox ground check")]
    [SerializeField] public float ScaleXtimes;
    [SerializeField] public float ScaleYtimes;
    [SerializeField] public LayerMask whatIsGround;

    [Header("Gravity")]
    [SerializeField] public float defaultGravity;
    [SerializeField] public float fallingGravity;
    [SerializeField] public float jumpCutGravity;
    [SerializeField] public float maxVelocity;

    [Header("Forces")]
    [SerializeField] public float moveSpeedAccelGround;
    [SerializeField] public float moveSpeedDeccelGround;
    [SerializeField] public float moveSpeedAccelCrouching;
    [SerializeField] public float moveSpeedDeccelCrouching;
    [SerializeField] public float moveSpeedAccelAir;
    [SerializeField] public float moveSpeedDeccelAir;
    [SerializeField] public float maxMoveSpeed;
    [SerializeField] public float jumpForce;
    [SerializeField] public float doubleJumpForce;
    [SerializeField] public float maxJumpForce;

    [Header("Buffer time")]
    [SerializeField] public float leaveGroundBufferTime;
    [SerializeField] public float jumpInputBufferTime;

    [Header("Throwing")]
    [SerializeField] public float maxThrowChargeTime = 3f;
    [SerializeField] public float throwChargeTime = 2f;
    [SerializeField] public float upWordsScaleThrow;
    [SerializeField] public float maxThrowForce;
    [SerializeField] public float minThrowForce;

    [Header("Pulling")]
    [SerializeField] public float maxPullChargeTime = 3f;
    [SerializeField] public float pullChargeTime = 2f;
    [SerializeField] public float upWordsScalePull;
    [SerializeField] public float maxPullForce;
    [SerializeField] public float minPullForce;

    [Header("Charge")]
    [SerializeField] private Color targetColor;

    [Header("Climbing")]
    [SerializeField] private LayerMask climbExcludeLayers;
    [SerializeField] public float maxMovingClimb;
    [SerializeField] public float movingAccelerationRate;
    [SerializeField] public float movingDecelerationRate;
    [SerializeField] public float climbSpeed;
    [SerializeField] public float climbEndJump;

}
