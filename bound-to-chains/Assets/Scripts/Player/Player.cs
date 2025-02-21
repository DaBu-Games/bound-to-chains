using UnityEngine;
using System.Collections;
using UnityEditor.Animations;

public class Player : MonoBehaviour
{
    [SerializeField] private SidescrollerVariables chained;
    [SerializeField] private SidescrollerVariables unchained;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private HingeJoint2D lastHinge;
    [SerializeField] private CheckForGround playerGroundCheck;
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private BallBehaviour ballBehaviour;
    [SerializeField] private Rigidbody2D ballrb2d;
    [SerializeField] private LayerMask ballLayerMask;
    [SerializeField] public Animator playerAnimator;
    [SerializeField] public CheckForChains checkForChains;

    private bool facingRight;
    private StateMachine stateMachine;
    private LayerMask originalExcludeLayers;
    private float startingMass;
    private float startingLinearDamping;
    private float startingAngularDamping;

    private bool canJump; 
    public bool isJumping { get; private set; }
    private float chargeStartTime;
    [SerializeField] private float maxChargeTime = 3f;
    [SerializeField] private float chargeTime = 2f;
    [SerializeField] private float raycastRange = 1f;
    [SerializeField] private float upWordsScale = 1.2f;
    [SerializeField] private float maxThrowForce = 2000f;
    [SerializeField] private float minThrowForce = 2000f;
    [SerializeField] private float playerAboveBallDifference = 0.55f;
    [SerializeField] private LayerMask ballExcludeLayers;
    [SerializeField] private Color targetColor;
    private bool isInRangeBall;

    [SerializeField] private float maxMovingClimb = 4f;
    [SerializeField] private float movingAccelerationRate = 4f;
    [SerializeField] private float movingDecelerationRate = 4f;
    [SerializeField] private float climbSpeed = 4f;
    [SerializeField] private float climbEndJump = 20f;
    [SerializeField] private float playerBellowBallDifference = 2f;

    private float playerFromBallDifference = 2f;
    private float minForceOnHinge = 40f;
    private float minForceOnBall = 40f; 

    public SidescrollerVariables variables { get; private set; }

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
        stateMachine.AddTransition(new Transition(climbingState, risingState, () => CanPlayerRise()));
        stateMachine.AddTransition(new Transition(climbingState, fallingState, () => CanPlayerFall()));

        //Crouching
        stateMachine.AddTransition(new Transition(climbingState, throwState, () => CanPlayerThrow()));
        stateMachine.AddTransition(new Transition(climbingState, pullingState, () => CanPlayerPull()));
        stateMachine.AddTransition(new Transition(climbingState, idleState, () => CanPlayerIdle()));

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
        stateMachine.AddTransition(new Transition(idleState, pullingState, () => CanPlayerPull()));
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

    // check if the player is holding the climbe button has more then 0 chains and the player is bellow the ball
    public bool CanPlayerClimbe()
    {
        return playerInput.isHoldingClimbe && checkForChains.chainsInHitbox.Count > 0 && 
                ballBehaviour.IsTransformBellowBall( this.transform.position.y, playerBellowBallDifference) && 
                    ballBehaviour.isGrounded;
    }
    public bool CanPlayerCrouch()
    {
        CheckForBall();

        return playerGroundCheck.isGrounded && ( playerInput.isHoldingCrouch || CanChargePull() );
    }

    // Check if the player has no upward velocity and the player is not grounded
    public bool CanPlayerFall()
    {
        return rb2d.linearVelocity.y < 0 && !playerGroundCheck.isGrounded;
    }

    private bool IsPlayerHanging()
    {

        return lastHinge.reactionForce.magnitude > minForceOnHinge && ballBehaviour.isGrounded && 
                ballBehaviour.IsTransformBellowBall(this.transform.position.y, playerBellowBallDifference);

    }

    public bool CanPlayerHang()
    {
        return ballBehaviour.CheckDistanceFromBall(this.transform.position, playerFromBallDifference) && 
                !playerGroundCheck.isGrounded && IsPlayerHanging();
    }

    // Check if the player has no x input and is grounded
    public bool CanPlayerIdle()
    {
        return playerInput.moveInput.x == 0 && playerGroundCheck.isGrounded;
    }

    public bool CanBufferJump()
    {
        return Time.time - playerGroundCheck.lastOnGroundTime <= variables.leaveGroundBufferTime;
    }

    public bool IsJumpBufferd()
    {
        return Time.time - playerInput.lastPressedJumpTime <= variables.jumpInputBufferTime && Time.time > variables.jumpInputBufferTime;
    }

    // Check if the player is grounded or if the player has touched te ground in time of 'variables.leaveGroundBufferTime'
    // And check if the player is not already jumping and if the player has pressed the jump butten in the time of 'variables.jumpInputBufferTime'
    public bool CanPlayerJump()
    {

        return (playerGroundCheck.isGrounded || CanBufferJump()) && !isJumping && IsJumpBufferd();

    }

    private bool CanChargePull()
    {
        return playerGroundCheck.isGrounded && isInRangeBall;
    }

    // Check if the player can charge and is holding the charge button
    public bool CanPlayerPull()
    {
        CheckForBall();

        return CanChargePull() && playerInput.isHoldingCharge && (ballBehaviour.GetForceOnBall() > minForceOnBall || ballBehaviour.isGrounded);
    }

    // Check if the player has a upward velocity above 0 and the player is not grounded
    public bool CanPlayerRise()
    {
        return rb2d.linearVelocity.y > 0 && !playerGroundCheck.isGrounded;
    }

    // check if the ball and player are grounded and the ball is inrange of the player
    public bool CanChargeThrow()
    {
        return ballBehaviour.isGrounded && CanChargePull();
    }

    // Check if the player can charge and is holding the charge button
    public bool CanPlayerThrow()
    {
        CheckForBall();

        return CanChargeThrow() && playerInput.isHoldingCharge;
    }

    // Check if the player input x is not equal to 0 and if the player is grounded
    public bool CanPlayerWalk()
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
        SetExcludeLayers(originalExcludeLayers);
    }

    public bool HasNoExcludeLayers()
    {
        return boxCollider2D.excludeLayers == originalExcludeLayers;
    }
    #endregion // 

    // All functions that influnce the player it zelf
    #region
    public void ResetJumpTime()
    {
        playerInput.SetJumpTime(variables.jumpInputBufferTime);
    }

    public void UnChainePlayer()
    {
        variables = unchained;
        lastHinge.connectedBody = null;
        lastHinge.useConnectedAnchor = false;
        SetPlayerGravity(variables.defaultGravity);
    }
    public void FlipCharachter()
    {
        if ( !facingRight && playerInput.moveInput.x > 0 || facingRight && playerInput.moveInput.x < 0)
        {
            // reverse the bool value
            facingRight = !facingRight;

            // rotate the player 180 degerees 
            this.transform.Rotate(0f, 180f, 0f);
        }
    }

    public void ResetCharachterRotation()
    {
        facingRight = true;
        this.transform.rotation = Quaternion.identity;
    }

    public void DecelPlayer()
    {
        // Calculate the target speed based on player input and max movement speed
        float targetSpeed = playerInput.moveInput.x * variables.maxMoveSpeed;

        // Calculate the difference between the target speed and the current velocity
        float speedDif = targetSpeed - rb2d.linearVelocity.x;

        // Calculate the deceleration force to apply
        float movement = speedDif * ( playerGroundCheck.isGrounded ? variables.moveSpeedDeccelGround : variables.moveSpeedDeccelAir );

        // Apply the calculated force to decelerate the Rigidbody2D
        rb2d.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    public void Crouching()
    {
        // Calculate the target speed based on player input and max movement speed
        float targetSpeed = playerInput.moveInput.x * variables.maxMoveSpeed;

        // check if the player is moving if is so accel if not deccel 
        float accelRate = (Mathf.Abs(playerInput.moveInput.x) > 0.01f) ? variables.moveSpeedAccelCrouching : variables.moveSpeedDeccelCrouching;

        // Calculate the difference between the target speed and the current velocity
        float speedDif = targetSpeed - rb2d.linearVelocity.x;

        // Calculate the force to be applied based on the acceleration rate and speed difference
        float movement = speedDif * accelRate;

        // Apply the calculated force to the Rigidbody2D
        rb2d.AddForce(movement * Vector2.right, ForceMode2D.Force);

    }

    public void MovingAir()
    {
        // Calculate the target speed based on player input and max movement speed
        float targetSpeed = playerInput.moveInput.x * variables.maxMoveSpeed;

        // check if the player is moving if is so accel if not deccel 
        float accelRate = (Mathf.Abs(playerInput.moveInput.x) > 0.01f) ? variables.moveSpeedAccelAir : variables.moveSpeedDeccelAir;

        // Calculate the difference between the target speed and the current velocity
        float speedDif = targetSpeed - rb2d.linearVelocity.x;

        // Calculate the force to be applied based on the acceleration rate and speed difference
        float movement = speedDif * accelRate;

        // Apply the calculated force to the Rigidbody2D
        rb2d.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    public void WhileJumping()
    {

        // check if the jump button is let go while jumping
        // if so increase the gravity of the player and "cancel" the jump
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

    public void Jump()
    {
        if (!canJump)
            return;

        canJump = false;
        isJumping = true;

        float force = variables.jumpForce;


        // check if there is any down force if so reset it 
        if (rb2d.linearVelocity.y < 0)
        {

            force -= rb2d.linearVelocity.y;

        }

        // add postive y force to the player 
        rb2d.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        // Clamp the upward velocity to the maximum jump force
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

    public bool CheckForBall()
    {

        // Check if the player is close enough to the metal ball
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, this.transform.right, raycastRange, ballLayerMask);

        isInRangeBall = hit.collider != null;

        Debug.DrawRay(this.transform.position, this.transform.right * raycastRange, isInRangeBall ? Color.green : Color.red);

        return isInRangeBall;

    }

    public void CheckChargeDuration( bool isThrowing )
    {

        if (isThrowing ? CanChargeThrow() : CanChargePull() && chargeStartTime != 0f)
        {

            // Calculate how long the player has been charging
            float chargeDuration = Time.time - chargeStartTime;

            // Calculate the charge ratio
            float chargeRatio = chargeDuration / chargeTime;
            chargeRatio = Mathf.Clamp01(chargeRatio); // Ensure it's between 0 and 1

            Color newColor = Color.Lerp(spriteRenderer.material.color, targetColor, chargeRatio);

            spriteRenderer.color = newColor;

            // if the player has let go the charge button
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
            // Set the max chargeduration to maxChargeTime
            else if (chargeDuration >= maxChargeTime)
            {
                if (!isThrowing)
                {
                    PullBallWithForce(maxChargeTime);
                }
                else
                {
                    ThrowBallWithForce(maxChargeTime);
                }
            }
        }
        else
        {
            StartCoroutine(ResetChargeState());
        }

    }

    public void ThrowBallWithForce(float chargeDuration)
    {

        playerAnimator.Play("ThrowingAnimation");

        // Calculate how hard the player can throw the ball
        float chargeFactor = Mathf.Min(chargeDuration / chargeTime, 1f);
        float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargeFactor);

        ballBehaviour.SetAirDrag();

        ballrb2d.AddForce(throwForce * this.transform.right, ForceMode2D.Impulse);
        ballrb2d.AddForce(throwForce * upWordsScale * this.transform.up, ForceMode2D.Impulse);

        StartCoroutine(ResetChargeState());

    }

    private void PullBallWithForce(float chargeDuration)
    {

        playerAnimator.Play("ThrowingAnimation");

        Vector2 directionToPlayer = (this.transform.position - ballrb2d.transform.position).normalized;

        // Calculate how hard the player can throw the ball
        float chargeFactor = Mathf.Min(chargeDuration / chargeTime, 1f);
        float pullForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargeFactor);

        ballBehaviour.SetAirDrag();

        ballrb2d.AddForce(pullForce * directionToPlayer, ForceMode2D.Impulse);
        ballrb2d.AddForce(pullForce * upWordsScale * Vector2.up, ForceMode2D.Impulse);

        if (ballBehaviour.IsTransformAboveBall(this.transform.position.y, playerAboveBallDifference))
        {
            ballBehaviour.SetExcludeLayers(ballExcludeLayers);
        }

        StartCoroutine(ResetChargeState());

    }


    // Reset the ChargeStartTime and switch state
    public IEnumerator ResetChargeState()
    {

        chargeStartTime = 0f;

        spriteRenderer.color = spriteRenderer.material.color;

        yield return new WaitForSeconds(0.25f);

        stateMachine.SwitchState(idleState);

    }

    public void Walking()
    {
        // Calculate the target speed based on player input and max movement speed
        float targetSpeed = playerInput.moveInput.x * variables.maxMoveSpeed;

        // check if the player is moving if is so accel if not deccel 
        float accelRate = (Mathf.Abs(playerInput.moveInput.x) > 0.01f) ? variables.moveSpeedAccelGround : variables.moveSpeedDeccelGround;

        // Calculate the difference between the target speed and the current velocity
        float speedDif = targetSpeed - rb2d.linearVelocity.x;

        // Calculate the force to be applied based on the acceleration rate and speed difference
        float movement = speedDif * accelRate;

        // Apply the calculated force to the Rigidbody2D
        rb2d.AddForce(movement * Vector2.right, ForceMode2D.Force);

    }

    public void MovingWhileClimbing()
    {
        // Calculate the target speed based on player input and max movement speed
        float targetSpeed = playerInput.moveInput.x * maxMovingClimb;

        // check if the player is moving if is so accel if not deccel 
        float accelRate = (Mathf.Abs(playerInput.moveInput.x) > 0.01f) ? movingAccelerationRate : movingDecelerationRate;

        // Calculate the difference between the target speed and the current velocity
        float speedDif = targetSpeed - rb2d.linearVelocity.x;

        // Calculate the force to be applied based on the acceleration rate and speed difference
        float movement = speedDif * accelRate;

        // Apply the calculated force to the Rigidbody2D
        rb2d.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    public void UpdateHighestChain()
    {
        if (checkForChains.chainsInHitbox.Count == 0)
        {
            checkForChains.SetChain( null ); // No chains in hitbox
            return;
        }

        // Find the highest chain above the player
        Transform highestChain = null;
        float highestY = float.MinValue;
        Vector2 playerPosition = this.transform.position;

        foreach ( Transform chain in checkForChains.chainsInHitbox)
        {
            if (chain.position.y > playerPosition.y && chain.position.y > highestY)
            {
                highestY = chain.position.y;
                highestChain = chain;
            }
        }

        if (highestChain != null && highestChain.transform.position.y >= this.transform.position.y)
        {
            // Update the current chain to the highest one found
            checkForChains.SetChain(highestChain);
        }

    }

    // Get the position of the hinge joined ancher point and convert it to world space
    public Vector2 GetAnchorPoint(HingeJoint2D hingeJoint)
    {
        // Get the anchor point in local space
        Vector2 localAnchor = hingeJoint.anchor;

        // Convert it to world space
        Vector2 worldAnchor = hingeJoint.transform.TransformPoint(localAnchor);

        return worldAnchor;
    }

    public void ClimbChain()
    {
        if (checkForChains.currentChain == null) return;

        // Get the target position of where the player is going to 
        Vector2 targetPosition = GetAnchorPoint(checkForChains.currentChain.GetComponent<HingeJoint2D>());

        Vector2 direction = (targetPosition - (Vector2)this.transform.position).normalized;

        rb2d.AddForce(direction * climbSpeed, ForceMode2D.Force);

    }

    public void FinishClimb()
    {

        // Slightly dampen movement before applying jump force
        rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, 0);

        // Apply upward force to finish the climb
        rb2d.AddForce(Vector2.up * climbEndJump, ForceMode2D.Impulse);

        // clear the list of chains
        checkForChains.chainsInHitbox.Clear();

        // Switch to the risingState
        stateMachine.SwitchState(risingState);
    }
    #endregion
}
