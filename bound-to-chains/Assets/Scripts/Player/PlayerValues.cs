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
    [SerializeField] public float upWordsScaleThrow;
    [SerializeField] public float maxThrowForce;
    [SerializeField] public float minThrowForce;

    [Header("Pulling")]
    [SerializeField] public float upWordsScalePull;
    [SerializeField] public float maxPullForce;
    [SerializeField] public float minPullForce;

    [Header("Charge")]
    [SerializeField] public Color targetColor;
    [SerializeField] public float maxChargeTime;
    [SerializeField] public float chargeTime;

    [Header("Climbing")]
    [SerializeField] public LayerMask climbExcludeLayers;
    [SerializeField] public float climbSpeed;
    [SerializeField] public float climbEndJump;

    [Header("Hanging")]
    [SerializeField] public float hangingMargin;

}
