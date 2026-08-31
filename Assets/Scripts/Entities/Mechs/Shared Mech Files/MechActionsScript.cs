using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechActionsScript : MonoBehaviour
{

    //main script for managing the player
    [Header("Components")]
    public Rigidbody2D myRigidbody2D;
    public CapsuleCollider2D myCapsuleCollider2D;
    public PlayerAnimationManagerScript playerAnimationManagerScript;
    public AudioSource jumpSound;
    public LogicScript logic;
    public GameObject DeathAnimation;

    [Header("Scripts")]
    public PlayerPhysicsScript myPlayerPhysicsScript;
    public VerticalMovementScript myVerticalMovementScript;
    public HorizontalMovementScript myHorizontalMovementScript;
    public MagnetManagerScript myMagnetManagerScript;
    public ChargeScript myChargeScript;
    public GroundCheckScript myGroundCheckScript;

    [Header("Logic")]
    protected bool playerAlive = true;
    public bool isCharging = false;

    [Header("Inputs")]
    public bool jumpPressed;
    protected bool jetpackOn;
    public bool chargePressed = false;
    public bool recoverMagnet = false;

    [Header("Horizontal Movement")]
    public float direction;
    public bool movementDisabled;
    public bool torsoFacingRight = true;

    [Header("Vertical Movement")]
    public float verticalDirection;

    [Header("Damage")]
    protected float knockbackTime = 0.25f;
    const float invincibilityTimeDefault = .5f;

    [Header("Physics")]
    public bool repelOn = false;
    public bool attractOn = false;
    public bool attractButtonHeld = false;
    public bool repelButtonHeld = false;
    public bool attractButtonMostRecent = false;
    public bool repelButtonMostRecent = false;
    public bool holdToAttract = false;

    //[Header("Orientation")]
    //public Camera virtualCamera;
    //public Vector2 mousePosition;
    //public Vector2 mouseRelativePosition;

    [Header("Input")]
    public GameObject BulletSpawner;
    public BulletSpawnerScript bulletSpawnerScript;
    public bool shootingInput = false;

    [Header("Magnet")]
    protected bool launchMagnetHeld = false;
    protected bool launchMagnet = false;

    [Header("Recent Input Timers")]
    public float lastMoveInputTime = -10f;
    public float lastJumpInputTime = -10f;
    public float lastRepelInputTime = -10f;
    public float lastAttractInputTime = -10f;
    public bool checkMovementInput = false;
    public bool checkJumpInput = false;
    protected virtual void Awake()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        repelOn = repelButtonHeld && !attractButtonMostRecent;
        attractOn = (attractButtonHeld || (holdToAttract && launchMagnetHeld)) && !repelButtonMostRecent;
        if (myMagnetManagerScript.magnetismDisabled)
        {
            attractOn = false;
            repelOn = false;
        }
        if (!playerAlive || logic.IsPaused)
        {
            return;
        }
        //Vertical
        jetpackOn = myVerticalMovementScript.handleVerticalUpdates(verticalDirection, playerAlive, jumpPressed);
        myVerticalMovementScript.SetJetpackSprites(direction, verticalDirection);
        //Magnet
        myMagnetManagerScript.SetMagnetAudio(repelOn, attractOn);
        if (shootingInput)
        {
            bulletSpawnerScript.Shoot();
        }
        if (launchMagnet)
        {
            myMagnetManagerScript.launchMagnet();
            launchMagnet = false;
        }

        //Experiment
        if (checkMovementInput)
        {
            lastMoveInputTime = Time.time;
        }
        if (checkJumpInput)
        {
            lastJumpInputTime = Time.time;
        }
    }
    protected void FixedUpdate()
    {
        handleGunOrientation();
        if (!playerAlive || movementDisabled)
        {
            //TODO set up dying stuff
            //animator.SetBool("hasDied", false);
            return;
        }
        myGroundCheckScript.UpdatePlatformFriction();
        myPlayerPhysicsScript.modifyPhysics(jetpackOn, direction, myGroundCheckScript.returnTrulyOnGround());
        myHorizontalMovementScript.handleHorizontalMovement(direction);
        myVerticalMovementScript.handleVerticalMovement();
        myVerticalMovementScript.handleRemainingFuelBar();
        myMagnetManagerScript.handleMagneticRepulsion(repelOn, attractOn);
        //isCharging = myChargeScript.handleCharging(chargePressed);
        myGroundCheckScript.CheckIfStuckInGround();
    }

    //public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    //{
    //    Vector2 v = ctx.ReadValue<Vector2>();

    //    if (ctx.performed) checkMovementInput = true;
    //    if (ctx.canceled) checkMovementInput = false;
    //    //Debug.Log("OnMove Activate");

    //}
    //public void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    //{
    //    if (ctx.performed)
    //    {
    //        jumpPressed = true;
    //        checkJumpInput = true;

    //    }
    //    if (ctx.canceled)
    //    {
    //        jumpPressed = false;
    //        checkJumpInput = false;
    //    }
    //}
    //public void OnRepel(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    //{
    //    if (ctx.ReadValueAsButton())
    //    {

    //        lastRepelInputTime = Time.time;
    //    }
    //    repelButtonHeld = ctx.ReadValueAsButton();
    //    if (ctx.started)
    //    {
    //        repelButtonMostRecent = true;
    //        attractButtonMostRecent = false;
    //    }
    //    if (ctx.canceled)
    //    {
    //        repelButtonMostRecent = false;
    //    }
    //}
    //public void OnAttract(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    //{
    //    if (ctx.ReadValueAsButton())
    //    {

    //        lastAttractInputTime = Time.time;
    //    }
    //    attractButtonHeld = ctx.ReadValueAsButton();
    //    if (ctx.started)
    //    {
    //        attractButtonMostRecent = true;
    //        repelButtonMostRecent = false;
    //    }
    //    if (ctx.canceled)
    //    {
    //        attractButtonMostRecent = false;
    //    }
    //}

    public void DisableMovement()
    {
        movementDisabled = true;
        myRigidbody2D.linearVelocity = Vector3.zero;
        playerAnimationManagerScript.setHorizontalSpeed(0);
        //animator.SetFloat("HorizontalInput", 0);
    }
    public void EnableMovement()
    {
        movementDisabled = false;
    }
    protected virtual void handleGunOrientation() { }

    public virtual void DamagePlayer(float Damage, Vector2 knockbackDirection, float knockback = 0, float invincibilityTime = invincibilityTimeDefault, bool DeathPit = false)
    {
        //healthScript.takeDamage(Damage, knockbackDirection, knockback, invincibilityTime, DeathPit);
    }
    
    public IEnumerator handleKnockback(float knockback, Vector2 knockbackDirection)
    {
        if (!playerAlive) yield break;
        //movementEnabled = false;
        myRigidbody2D.AddForce(knockbackDirection * knockback * 10, ForceMode2D.Impulse);
        playerAnimationManagerScript.setAllSpritesColor(Color.red);
        yield return new WaitForSeconds(knockbackTime);
        //movementEnabled = true;
        playerAnimationManagerScript.setAllSpritesColor(Color.white);
    }

    public void KillPlayer(bool DeathPit = false)
    {
        if (!playerAlive) return;
        playerAlive = false;
        //TODO dying stuff
        //animator.SetBool("hasDied", true);
        playerAnimationManagerScript.startDeath();
        if (DeathPit)
        {
            gameObject.SetActive(false);
            return;
        }
        DeathAnimation.SetActive(true);
        myRigidbody2D.linearVelocity = new Vector3(0, 0, 0);
        myRigidbody2D.gravityScale = 1.5f;
        myChargeScript.chargeIndicator.sprite = null;
        myMagnetManagerScript.playerKilled();
        myVerticalMovementScript.PlayerKilled();
        StartCoroutine(HandleDeath());
    }
    protected IEnumerator HandleDeath()
    {
        yield return new WaitUntil(() => DeathAnimation.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Dead"));
        gameObject.SetActive(false);
    }

    public float startMagnetMaxYSpeed(float maxYSpeed, float maxYSpeedPressingDown)
    {
        bool pressingDown = verticalDirection <= -.25;
        return myMagnetManagerScript.getMagnetMaxYSpeed(maxYSpeed, maxYSpeedPressingDown, pressingDown, repelOn, attractOn);
    }
    public float startMagnetMaxXSpeed(float maxXSpeed)
    {
        return myMagnetManagerScript.getMagnetMaxXSpeed(maxXSpeed, repelOn, attractOn);
    }
    //public void Aim(InputAction.CallbackContext context)
    //{
    //    if (Mathf.Abs(context.ReadValue<Vector2>().x) > .1 || Mathf.Abs(context.ReadValue<Vector2>().y) > .1)
    //    {
    //        rightJoystick = context.ReadValue<Vector2>();
    //    }
    //}
    //public void JumpInput(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        jumpPressed = true;
    //        lastJumpInputTime = Time.time;
    //        if (cutsceneManagerScript != null)
    //        {
    //            cutsceneManagerScript.SkipCutscene();
    //        }
    //    }
    //    if (context.canceled)
    //    {
    //        jumpPressed = false;
    //    }
    //}

    //public void RecoverMagnetInput(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        recoverMagnet = true;
    //    }
    //    if (context.canceled)
    //    {
    //        recoverMagnet = false;
    //    }
    //}

    //public void ShootingInput(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        shootingInput = true;
    //    }
    //    if (context.canceled)
    //    {
    //        shootingInput = false;
    //    }
    //}
    //public void LaunchMagnet(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        launchMagnetHeld = true;
    //        launchMagnet = true;
    //    }
    //    if (context.canceled)
    //    {
    //        launchMagnetHeld = false;
    //        launchMagnet = false;
    //    }
    //}
    //public void MagnetRepel(InputAction.CallbackContext context)
    //{
    //    repelButtonHeld = context.ReadValueAsButton();
    //    if (context.started)
    //    {
    //        repelButtonMostRecent = true;
    //        attractButtonMostRecent = false;
    //    }
    //    if (context.canceled)
    //    {
    //        repelButtonMostRecent = false;
    //    }
    //    if (context.performed && myMagnetManagerScript.returnMyMagnet() != null)
    //    {
    //        lastRepelInputTime = Time.time;
    //    }
    //}
    //public void MagnetAttract(InputAction.CallbackContext context)
    //{
    //    attractButtonHeld = context.ReadValueAsButton();
    //    if (context.started)
    //    {
    //        attractButtonMostRecent = true;
    //        repelButtonMostRecent = false;
    //    }
    //    if (context.canceled)
    //    {
    //        attractButtonMostRecent = false;
    //    }
    //    if (context.performed && myMagnetManagerScript.returnMyMagnet() != null)
    //    {
    //        lastAttractInputTime = Time.time;
    //    }
    //}
    //public void Pause(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        logic.SetPausePressed();
    //    }
    //}
}

