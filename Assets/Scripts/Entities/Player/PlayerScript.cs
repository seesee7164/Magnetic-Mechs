using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{

    //main script for managing the player
    [Header("Components")]
    public Rigidbody2D myRigidbody2D;
    public CapsuleCollider2D myCapsuleCollider2D;
    public PlayerAnimationManagerScript playerAnimationManagerScript;
    public PlayerHealthScript healthScript;
    public AudioSource jumpSound;
    public CutsceneManager cutsceneManagerScript;
    public MultiSceneVariables savedVariables;
    public PlayerInput myInput;
    public GameObject pointerArrow;
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
    private bool playerAlive = true;
    public bool gamePadNotMouse = false;
    public bool isCharging = false;

    [Header("Inputs")]
    public bool jumpPressed;
    private bool jetpackOn;
    public bool chargePressed = false;

    [Header("Horizontal Movement")]
    public float direction;
    public bool movementDisabled;
    public bool torsoFacingRight = true;

    [Header("Vertical Movement")]
    public float verticalDirection;

    [Header("Damage")]
    private float knockbackTime = 0.25f;
    const float invincibilityTimeDefault = .5f;

    [Header("Physics")]
    public bool repelOn = false;
    public bool attractButtonHeld = false;
    public bool attractOn = false;
    public bool holdToAttract = false;

    [Header("Orientation")]
    public Camera virtualCamera;
    public Vector2 mousePosition;
    public Vector2 mouseRelativePosition;

    [Header("Input")]
    public GameObject BulletSpawner;
    public BulletSpawnerScript bulletSpawnerScript;
    public bool shootingInput = false;
    public Vector2 rightJoystick = Vector2.left;

    [Header("Magnet")]
    private bool launchMagnetHeld = false;
    private bool launchMagnet = false;

    [Header("Recent Input Timers")]
    public float lastMoveInputTime = -10f;
    public float lastJumpInputTime = -10f;
    public float lastRepelInputTime = -10f;
    public float lastAttractInputTime = -10f;
    public bool checkMovementInput = false;
    public bool checkJumpInput = false;

    private void Awake()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
        //sprite = GetComponent<SpriteRenderer>();
        myInput = GetComponent<PlayerInput>();
        healthScript = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<PlayerHealthScript>();
        virtualCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        GameObject savedVariablesObject = GameObject.FindGameObjectWithTag("MultiSceneVariables");
        if(savedVariablesObject != null)
        {
            gamePadNotMouse = savedVariablesObject.GetComponent<MultiSceneVariables>().gamePadNotMouse;
        }
        if (gamePadNotMouse)
        {
            myInput.defaultControlScheme = "Gamepad";
            pointerArrow.GetComponent<SpriteRenderer>().enabled = true;
        }
        else myInput.defaultControlScheme = "KeyboardMouse";
        BulletSpawner = GameObject.FindGameObjectWithTag("BulletSpawner");
        bulletSpawnerScript = BulletSpawner.GetComponent<BulletSpawnerScript>();
    }

    private void Start() {
        holdToAttract = InputRebinding.Instance.GetHoldToAttract();
        InputRebinding.Instance.OnHoldToAttractChanged += InputRebinding_OnHoldToAttractChanged;
    }

    private void OnDestroy() {
        InputRebinding.Instance.OnHoldToAttractChanged -= InputRebinding_OnHoldToAttractChanged;
    }

    private void InputRebinding_OnHoldToAttractChanged(object sender, EventArgs e) {
        holdToAttract = InputRebinding.Instance.GetHoldToAttract();
    }

    // Update is called once per frame
    void Update()
    {
        attractOn = attractButtonHeld || (holdToAttract && launchMagnetHeld);
        if (!playerAlive || logic.IsPaused)
        {
            return;
        }
        mousePosition = virtualCamera.ScreenToWorldPoint(Input.mousePosition);//orientation
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
        if (checkMovementInput) {
            lastMoveInputTime = Time.time;
        }
        if (checkJumpInput) {
            lastJumpInputTime = Time.time;
        }
    }
    private void FixedUpdate()
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
        isCharging = myChargeScript.handleCharging(chargePressed);
        myGroundCheckScript.CheckIfStuckInGround();
    }

    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx) { 
        Vector2 v = ctx.ReadValue<Vector2>();

        if (ctx.performed) checkMovementInput = true;
        if (ctx.canceled) checkMovementInput = false;
        //Debug.Log("OnMove Activate");

    }
    public void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext ctx) { 
        if (ctx.performed) {
            jumpPressed = true;
            checkJumpInput = true;

        }
        if (ctx.canceled)
        {
            jumpPressed = false;
            checkJumpInput = false;
        }
        //Debug.Log("OnJump Activate");
    }

    public void OnRepel(UnityEngine.InputSystem.InputAction.CallbackContext ctx) { 
        if (ctx.ReadValueAsButton()) {
           
            lastRepelInputTime = Time.time;
        }
        repelOn = ctx.ReadValueAsButton();
    }
    public void OnAttract(UnityEngine.InputSystem.InputAction.CallbackContext ctx) { 
        if (ctx.ReadValueAsButton()) {
           
            lastAttractInputTime = Time.time;
        }
        attractButtonHeld = ctx.ReadValueAsButton();
    }

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
    private void handleGunOrientation()
    {
        if (gamePadNotMouse)
        {
            BulletSpawner.transform.right = rightJoystick;
            myMagnetManagerScript.setMagnetSpawnerAngle(rightJoystick);
            //playerAnimationManagerScript.setFiringAngle(Vector2.Angle(myRigidbody2D.position, mousePosition));
        }
        else
        {
            mouseRelativePosition = mousePosition - myRigidbody2D.position;
            BulletSpawner.transform.right = mouseRelativePosition;
            myMagnetManagerScript.setMagnetSpawnerAngle(mouseRelativePosition);
            torsoFacingRight = playerAnimationManagerScript.setFiringAngle(Mathf.Atan2(mouseRelativePosition.y, mouseRelativePosition.x) * Mathf.Rad2Deg);
        }
    }
    
    public void DamagePlayer(float Damage, Vector2 knockbackDirection, float knockback = 0, float invincibilityTime = invincibilityTimeDefault)
    {
        healthScript.takeDamage(Damage, knockbackDirection, knockback,invincibilityTime);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isCharging)
        {
            if (collision.gameObject.layer == 7) // enemy
            {
                Vector2 relativePosition = transform.position - collision.transform.position;
                float knockbackVal = 1;
                if (collision.gameObject.tag == "RobotSpiderQueen")
                {
                    knockbackVal = 1.5f;
                    if (relativePosition.y > Math.Abs(relativePosition.x) / .9f)
                    {
                        knockbackVal = 3.25f;
                    }
                }
                DamagePlayer(1, relativePosition.normalized, knockbackVal);
            }
            if (collision.gameObject.layer == 12) // death pit
            {
                //Vector2 relativePosition = transform.position - collision.transform.position;
                DamagePlayer(16, new Vector2(0, 0));
            }
            if (collision.gameObject.layer == 19) // spike
            {
                Vector2 relativePosition = transform.position - collision.transform.position;
                float knockbackVal = .5f;
                if (relativePosition.y > Math.Abs(relativePosition.x) / .9f)
                {
                    knockbackVal = 1.25f;
                }
                DamagePlayer(1, relativePosition.normalized, knockbackVal);
            }
        }
        else
        {
            if (collision.gameObject.layer == 7) // enemy
            {
                Vector2 relativePosition = transform.position - collision.transform.position;
                float knockbackVal = 1;
                if (collision.gameObject.tag == "RobotSpiderQueen")
                {
                    knockbackVal = 1.5f;
                    if (relativePosition.y > Math.Abs(relativePosition.x) / .9f)
                    {
                        knockbackVal = 3.25f;
                    }
                }

                StartCoroutine(handleKnockback(knockbackVal, relativePosition.normalized));
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isCharging)
        {
            if (collision.gameObject.layer == 13) //enemy bullet
            {
                Vector2 relativePosition = transform.position - collision.transform.position;
                DamagePlayer(1, relativePosition.normalized, .5f);
            }
            if (collision.gameObject.layer == 16) // rock
            {
                Vector2 relativePosition = transform.position - collision.transform.position;
                DamagePlayer(1, relativePosition.normalized, 0f);
            }
            if (collision.gameObject.layer == 7) // enemy
            {
                Vector2 relativePosition = transform.position - collision.transform.position;
                DamagePlayer(1, relativePosition.normalized, .5f);
            }
        }
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

    public void KillPlayer()
    {
        if (!playerAlive) return;
        playerAlive = false;
        //TODO dying stuff
        //animator.SetBool("hasDied", true);
        playerAnimationManagerScript.startDeath();
        DeathAnimation.SetActive(true);
        myRigidbody2D.linearVelocity = new Vector3(0, 0, 0);
        myRigidbody2D.gravityScale = 1.5f;
        myChargeScript.chargeIndicator.sprite = null;
        myMagnetManagerScript.playerKilled();
        myVerticalMovementScript.PlayerKilled();
        StartCoroutine(HandleDeath());
    }
    IEnumerator HandleDeath()
    {
        yield return new WaitUntil(() => DeathAnimation.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Dead"));
        gameObject.SetActive(false);
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        direction = input.x;
        verticalDirection = input.y;
        if (context.performed && Mathf.Abs(input.x) > 0.1f) { 
            lastMoveInputTime = Time.time;
        }
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
    public void Aim(InputAction.CallbackContext context)
    {
        if(Mathf.Abs(context.ReadValue<Vector2>().x) > .1 || Mathf.Abs(context.ReadValue<Vector2>().y) > .1)
        {
            rightJoystick = context.ReadValue<Vector2>();
        }
    }
    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("test");
            jumpPressed = true;
            lastJumpInputTime = Time.time;
            if (cutsceneManagerScript != null)
            {
                cutsceneManagerScript.SkipCutscene();
            }
        }
        if (context.canceled)
        {
            jumpPressed = false;
        }
    }

    public void ChargeInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            chargePressed = true;
        }
        if (context.canceled)
        {
            chargePressed = false;
        }
    }

    public void ShootingInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            shootingInput = true;
        }
        if (context.canceled)
        {
            shootingInput = false;
        }
    }
    public void LaunchMagnet(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            launchMagnetHeld = true;
            launchMagnet = true;
        }
        if (context.canceled)
        {
            launchMagnetHeld = false;
            launchMagnet = false;
        }
    }
    public void MagnetRepel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            repelOn = true;
        }
        if (context.canceled)
        {
            repelOn = false;
        }
        if (context.performed && myMagnetManagerScript.returnMyMagnet() != null){
            lastRepelInputTime= Time.time;
        }
    }
    public void MagnetAttract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            attractButtonHeld = true;
        }
        if (context.canceled)
        {
            attractButtonHeld = false;
        }
        if (context.performed && myMagnetManagerScript.returnMyMagnet() != null){
            lastAttractInputTime= Time.time;
        }
    }
    public void Pause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            logic.SetPausePressed();
        }
    }
}
