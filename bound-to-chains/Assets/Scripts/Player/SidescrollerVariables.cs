using System.Collections;
using System.Collections.Generic; 
using UnityEngine;


[CreateAssetMenu(fileName = "SidescrollerVariables", menuName = "Movement/Side scroller values")]

public class SidescrollerVariables : ScriptableObject
{
    [Header("Hitbox ground check")]
    [SerializeField] public float ScaleXtimes;
    [SerializeField]  public float ScaleYtimes;
    [SerializeField] public LayerMask whatIsGround;

    [Header("Gravity" )]
    [SerializeField] public float defaultGravity;
    [SerializeField] public float fallingGravity;
    [SerializeField] public float jumpCutGravity;

    [Header ("Forces")]
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

    [Header ("Buffer time")]
    [SerializeField] public float leaveGroundBufferTime;
    [SerializeField] public float jumpInputBufferTime;

    //[Header ("Projectile")]
    //[SerializeField] public LayerMask whatIsProjectile;
    //[SerializeField] public float dmgProjectileBufferTime;
    //[SerializeField] public float parryBufferTime;

}
