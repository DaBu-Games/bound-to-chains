using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    [Header("The diffrent Player / Ball Values")]
    [SerializeField] private PlayerValues chained;
    [SerializeField] private PlayerValues unchained;
    [SerializeField] private BallValues ballValues;

    [Header("Player components")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CheckForGround playerGroundCheck;
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider2D;

    [Header("Ball components")]
    [SerializeField] private BallBehaviour ballBehaviour;
    [SerializeField] private CheckForBall checkForBall;
    [SerializeField] private Rigidbody2D ballrb2d;

    [Header("Chain component")]
    [SerializeField] private GenerateChain generateChain; 

    [Header("Player animator")]
    [SerializeField] public Animator playerAnimator;

    public PlayerValues variables { get; private set; }
    private bool facingRight;
    private StateMachine stateMachine;
    private LayerMask originalExcludeLayers;
    private float startingMass;
    private float startingLinearDamping;
    private float startingAngularDamping;
    private bool canJump;
    public bool isJumping { get; private set; }
    private float chargeStartTime;

    private IState risingState;
    private IState idleState;
    private void Start()
    {
        variables = chained;

        originalExcludeLayers = boxCollider2D.excludeLayers;
        rb2d.gravityScale = variables.defaultGravity;
        startingMass = rb2d.mass;
        startingLinearDamping = rb2d.linearDamping;
        startingAngularDamping = rb2d.angularDamping;

        facingRight = true;

        stateMachine = new StateMachine();

        IState climbingState = new ClimbingState(this);
        IState crouchingState = new CrouchingState(this);
        IState fallingState = new FallingState(this);
        IState hangingState = new HangingState(this);
        idleState = new IdleState(this);
        IState jumpingState = new JumpingState(this); // 
        IState pullingState = new PullingState(this);
        risingState = new RisingState(this);
        IState throwState = new ThrowState(this);
        IState walkingState = new WalkingState(this);

        //Climbing 
        stateMachine.AddTransition(new Transition(climbingState, risingState, () => CanPlayerRise() && !CanPlayerClimbe()));
        stateMachine.AddTransition(new Transition(climbingState, fallingState, () => CanPlayerFall() && !CanPlayerClimbe()));

        //Crouching
        stateMachine.AddTransition(new Transition(crouchingState, throwState, () => CanPlayerThrow() && playerInput.isHoldingCharge));
        stateMachine.AddTransition(new Transition(crouchingState, pullingState, () => CanPlayerPull() && playerInput.isHoldingCharge));
        stateMachine.AddTransition(new Transition(crouchingState, idleState, () => CanPlayerIdle() && !checkForBall.BallCheck() && !playerInput.isHoldingCrouch ));
        stateMachine.AddTransition(new Transition(crouchingState, walkingState, () => CanPlayerWalk() && !checkForBall.BallCheck() && !playerInput.isHoldingCrouch));

        //Falling
        stateMachine.AddTransition(new Transition(fallingState, climbingState, () => CanPlayerClimbe()));
        stateMachine.AddTransition(new Transition(fallingState, hangingState, () => CanPlayerHang()));
        stateMachine.AddTransition(new Transition(fallingState, jumpingState, () => CanPlayerJump() && !isJumping && HasNoExcludeLayers() ));
        stateMachine.AddTransition(new Transition(fallingState, walkingState, () => CanPlayerWalk() && !isJumping && HasNoExcludeLayers()));
        stateMachine.AddTransition(new Transition(fallingState, idleState, () => CanPlayerIdle() && !isJumping && HasNoExcludeLayers()));

        //Hanging
        stateMachine.AddTransition(new Transition(hangingState, climbingState, () => CanPlayerClimbe()));
        stateMachine.AddTransition(new Transition(hangingState, idleState, () => CanPlayerIdle()));
        stateMachine.AddTransition(new Transition(hangingState, jumpingState, () => CanPlayerJump()));
        stateMachine.AddTransition(new Transition(hangingState, walkingState, () => CanPlayerWalk()));

        //Idle
        stateMachine.AddTransition(new Transition(idleState, jumpingState, () => CanPlayerJump()));
        stateMachine.AddTransition(new Transition(idleState, climbingState, () => CanPlayerClimbe()));
        stateMachine.AddTransition(new Transition(idleState, walkingState, () => CanPlayerWalk()));
        stateMachine.AddTransition(new Transition(idleState, crouchingState, () => CanPlayerCrouch()));
        stateMachine.AddTransition(new Transition(idleState, pullingState, () => CanPlayerPull() && playerInput.isHoldingCharge));
        stateMachine.AddTransition(new Transition(idleState, fallingState, () => CanPlayerFall()));
        stateMachine.AddTransition(new Transition(idleState, risingState, () => CanPlayerRise()));

        //Rising
        stateMachine.AddTransition(new Transition(risingState, fallingState, () => CanPlayerFall()));
        stateMachine.AddTransition(new Transition(risingState, climbingState, () => CanPlayerClimbe()));
        stateMachine.AddTransition(new Transition(risingState, hangingState, () => CanPlayerHang()));
        stateMachine.AddTransition(new Transition(risingState, jumpingState, () => CanPlayerJump() && !isJumping && HasNoExcludeLayers()));
        stateMachine.AddTransition(new Transition(risingState, walkingState, () => CanPlayerWalk() && !isJumping && HasNoExcludeLayers()));
        stateMachine.AddTransition(new Transition(risingState, idleState, () => CanPlayerIdle() && !isJumping && HasNoExcludeLayers()));

        //Walking
        stateMachine.AddTransition(new Transition(walkingState, jumpingState, () => CanPlayerJump()));
        stateMachine.AddTransition(new Transition(walkingState, idleState, () => CanPlayerIdle()));
        stateMachine.AddTransition(new Transition(walkingState, hangingState, () => CanPlayerHang()));
        stateMachine.AddTransition(new Transition(walkingState, fallingState, () => CanPlayerFall()));

        stateMachine.SwitchState(idleState); 
    }

    private void Update()
    {
        stateMachine.OnUpdate();
    }

    private void FixedUpdate()
    {
        stateMachine.OnFixedUpdate();
    }

    // All conditions for transtions 
    #region 
    private bool CanPlayerClimbe()
    {
        return playerInput.isHoldingClimbe && ballBehaviour.isGrounded &&
                ballBehaviour.IsTransformBellowBall(this.transform.position.y, ballValues.playerBellowBallDifference);
    }
    private bool CanPlayerCrouch()
    {
        return playerGroundCheck.isGrounded && (playerInput.isHoldingCrouch || ballBehaviour.isGrounded && checkForBall.BallCheck());
    }

    private bool CanPlayerFall()
    {
        return rb2d.linearVelocity.y < 0 && !playerGroundCheck.isGrounded;
    }

    private bool CanPlayerHang()
    {
        return generateChain.IsChainMaxLength( variables.hangingMargin ) && !playerGroundCheck.isGrounded && ballBehaviour.isGrounded &&
                ballBehaviour.IsTransformBellowBall(this.transform.position.y, ballValues.playerBellowBallDifference);
    }

    private bool CanPlayerIdle()
    {
        return playerInput.moveInput.x == 0 && playerGroundCheck.isGrounded;
    }

    private bool CanBufferJump()
    {
        return Time.time - playerGroundCheck.lastOnGroundTime <= variables.leaveGroundBufferTime;
    }

    private bool IsJumpBufferd()
    {
        return Time.time - playerInput.lastPressedJumpTime <= variables.jumpInputBufferTime && Time.time > variables.jumpInputBufferTime;
    }

    private bool CanPlayerJump()
    {

        return (playerGroundCheck.isGrounded || CanBufferJump()) && !isJumping && IsJumpBufferd();

    }

    private bool CanPlayerPull()
    {
        return playerGroundCheck.isGrounded && !checkForBall.BallCheck()
                && ballBehaviour.isGrounded;
    }

    private bool CanPlayerThrow()
    {
        return playerGroundCheck.isGrounded && checkForBall.BallCheck() && ballBehaviour.isGrounded;
    }

    private bool CanPlayerRise()
    {
        return rb2d.linearVelocity.y > 0 && !playerGroundCheck.isGrounded;
    }

    private bool CanPlayerWalk()
    {
        return playerInput.moveInput.x != 0 && playerGroundCheck.isGrounded;
    }
    #endregion

    // All functions to change or get player value's
    #region 
    public IState GetPlayerState()
    {
        return stateMachine.currentState;
    }
    public void SetPlayerGravity(float gravity)
    {
        rb2d.gravityScale = gravity;
    }

    public void SetPlayerMass(float mass)
    {
        rb2d.mass = mass;
    }

    public void ResetPlayerMass()
    {
        SetPlayerMass(startingMass);
    }

    public void SetPlayerDamping(float damping)
    {
        rb2d.linearDamping = damping;
        rb2d.angularDamping = damping;
    }

    public void ResetPlayerDamping()
    {
        rb2d.linearDamping = startingLinearDamping;
        rb2d.angularDamping = startingAngularDamping;
    }

    public void SetLinearVelocity(Vector2 velocity)
    {
        rb2d.linearVelocity = velocity;
    }

    public void ResetLinearVelocity()
    {
        rb2d.linearVelocity = Vector2.zero;
    }

    public Vector2 GetLinearVelocity()
    {
        return rb2d.linearVelocity;
    }

    public void SetExcludeLayers(LayerMask excludeLayers)
    {
        boxCollider2D.excludeLayers = excludeLayers;
    }

    public void ResetExludeLayers()
    {
        Debug.Log("reset");
        SetExcludeLayers(originalExcludeLayers);
    }

    public bool HasNoExcludeLayers()
    {
        return boxCollider2D.excludeLayers == originalExcludeLayers;
    }
    #endregion

    // All functions that influnce the player it zelf
    #region
    public void ResetJumpTime()
    {
        playerInput.SetJumpTime(variables.jumpInputBufferTime);
    }

    public void FlipCharachter()
    {
        if ( !facingRight && playerInput.moveInput.x > 0 || facingRight && playerInput.moveInput.x < 0)
        {
            facingRight = !facingRight;

            this.transform.Rotate(0f, 180f, 0f);
        }
    }

    public void ResetCharachterRotation()
    {
        facingRight = true;
        this.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// Check if the player is holding the jump button
    /// </summary>
    public void WhileJumping()
    {
        if ( isJumping && !playerInput.isHoldingJump )
        {

            rb2d.gravityScale = variables.jumpCutGravity;
            CancelJump();

        }

    }

    public void CancelJump()
    {
        isJumping = false;
    }

    public void CanJump()
    {
        canJump = true;
    }

    /// <summary>
    /// Adds a upwards force to the player but clamps the force to the maxJumpForce
    /// </summary>
    public void Jump()
    {
        if (!canJump)
            return;

        canJump = false;
        isJumping = true;
        float force = variables.jumpForce;

        if (rb2d.linearVelocity.y < 0)
        {

            force -= rb2d.linearVelocity.y;

        }

        rb2d.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        rb2d.linearVelocity = new Vector2(
            rb2d.linearVelocity.x,
            Mathf.Clamp(
                rb2d.linearVelocity.y,
                -Mathf.Infinity, variables.maxJumpForce
                )
            );

        ResetJumpTime();

        stateMachine.SwitchState(risingState);

    }

    public void LimitVelocity()
    {
        rb2d.linearVelocityY = Mathf.Clamp(rb2d.linearVelocity.y, -variables.maxVelocity, float.MaxValue);
    }

    public void StartCharge()
    {
        chargeStartTime = Time.time;
    }

    /// <summary>
    /// Check the charge duration of the player and throw or pull the ball using the chargeDuration
    /// </summary>
    /// <param name="isThrowing"></param>
    public void CheckChargeDuration( bool isThrowing )
    {

        if ( ( isThrowing ? CanPlayerThrow() : CanPlayerPull() ) && chargeStartTime != 0f)
        {

            float chargeDuration = Time.time - chargeStartTime;

            float chargeRatio = chargeDuration / variables.chargeTime;
            chargeRatio = Mathf.Clamp01(chargeRatio);

            Color newColor = Color.Lerp(spriteRenderer.material.color, variables.targetColor, chargeRatio);

            spriteRenderer.color = newColor;

            if (!playerInput.isHoldingCharge)
            {
                if (!isThrowing) 
                {
                    PullBallWithForce(chargeDuration);
                }
                else
                {
                    ThrowBallWithForce(chargeDuration);
                }
            }
            else if (chargeDuration >= variables.maxChargeTime )
            {
                if (!isThrowing)
                {
                    PullBallWithForce(variables.maxChargeTime);
                }
                else
                {
                    ThrowBallWithForce(variables.maxChargeTime);
                }
            }
        }
        else
        {
            StartCoroutine(ResetChargeState());
        }

    }

    /// <summary>
    /// Add force to the bal in the direction the player is facing. Makes the force stronger the higher chargeDuration is
    /// </summary>
    /// <param name="chargeDuration"></param>
    public void ThrowBallWithForce(float chargeDuration)
    {
        playerAnimator.Play("ThrowingAnimation");

        float chargeFactor = Mathf.Min(chargeDuration / variables.chargeTime, 1f);
        float throwForce = Mathf.Lerp(variables.minThrowForce, variables.maxThrowForce, chargeFactor);

        ballBehaviour.SetAirDrag();

        ballrb2d.AddForce(throwForce * this.transform.right, ForceMode2D.Impulse);
        ballrb2d.AddForce(throwForce * variables.upWordsScaleThrow * this.transform.up, ForceMode2D.Impulse);

        StartCoroutine(ResetChargeState());
    }

    /// <summary>
    /// Add force to the bal in towards the player. Makes the force stronger the higher chargeDuration is
    /// </summary>
    /// <param name="chargeDuration"></param>
    private void PullBallWithForce(float chargeDuration)
    {
        playerAnimator.Play("ThrowingAnimation");

        Vector2 directionToPlayer = (this.transform.position - ballrb2d.transform.position).normalized;

        float chargeFactor = Mathf.Min(chargeDuration / variables.chargeTime, 1f);
        float pullForce = Mathf.Lerp(variables.minThrowForce, variables.maxThrowForce, chargeFactor);

        ballBehaviour.SetAirDrag();

        ballrb2d.AddForce(pullForce * directionToPlayer, ForceMode2D.Impulse);
        ballrb2d.AddForce(pullForce * variables.upWordsScalePull * this.transform.up, ForceMode2D.Impulse);

        if (ballBehaviour.IsTransformAboveBall(this.transform.position.y, ballValues.playerAboveBallDifference))
        {
            ballBehaviour.SetExcludeLayers(ballValues.ballExcludeLayers);
        }

        StartCoroutine(ResetChargeState());
    }

    public IEnumerator ResetChargeState()
    {
        chargeStartTime = 0f;
        spriteRenderer.color = spriteRenderer.material.color;

        yield return new WaitForSeconds(0.25f);

        stateMachine.SwitchState(idleState);
    }

    /// <summary>
    /// Adds a accel or deccel force to the player depending on the x move input.
    /// </summary>
    /// <param name="accel"></param>
    /// <param name="deccel"></param>
    public void MovePlayer(float accel, float deccel)
    {
        float targetSpeed = playerInput.moveInput.x * variables.maxMoveSpeed;
        float accelRate = (Mathf.Abs(playerInput.moveInput.x) > 0.01f) ? accel: deccel;
        float speedDif = targetSpeed - rb2d.linearVelocity.x;
        float force = speedDif * accelRate;

        rb2d.AddForce(force * Vector2.right, ForceMode2D.Force);
    }

    /// <summary>
    /// Adds a deccel force over time
    /// </summary>
    /// <param name="deccel"></param>
    public void DecelPlayer(float deccel)
    {
        float speedDif = -rb2d.linearVelocity.x;
        float decelerationForce = speedDif * deccel;

        rb2d.AddForce(decelerationForce * Vector2.right, ForceMode2D.Force);
    }

    /// <summary>
    /// Add a updwards force slowly to the player towards the ball
    /// </summary>
    public void ClimbChain()
    {
        Vector2 direction = (ballBehaviour.GetPosition() - this.transform.position).normalized;

        rb2d.AddForce(direction * variables.climbSpeed, ForceMode2D.Force);
    }

    /// <summary>
    /// Add a upwards force to the player and switch to the risingstate
    /// </summary>
    public void FinishClimb()
    {
        SetLinearVelocity( new Vector2(rb2d.linearVelocity.x, 0) );
        rb2d.AddForce(Vector2.up * variables.climbEndJump, ForceMode2D.Impulse);

        stateMachine.SwitchState(risingState);
    }
    #endregion
}
