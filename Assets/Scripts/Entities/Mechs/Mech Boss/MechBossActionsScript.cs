using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechBossActionsScript : MechActionsScript
{

    //main script for managing the player
    [Header("Components")]
    public MechBossHealthScript healthScript;
    public Transform playerTransform;

    [Header("Aiming")]
    private Vector2 aimTarget = Vector2.left;
    private Vector2 currentAim = Vector2.up;
    private float turningRate = 300f;
    private float aimingTimer = 0f;
    private float timeToUpdate = .01f;
    [Header("Variables")]
    public bool finishedAiming = false;
    public int repeatingAimCounter = 0;

    //[Header("Orientation")]
    //public Camera virtualCamera;
    //public Vector2 mousePosition;
    //public Vector2 mouseRelativePosition;

    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    protected override void FixedUpdate()
    {
        UpdateAimTarget();
        base.FixedUpdate();
    }

    public override void DamagePlayer(float Damage, Vector2 knockbackDirection, float knockback = 0, float invincibilityTime = invincibilityTimeDefault, bool DeathPit = false)
    {
        healthScript.takeDamage(Damage, knockbackDirection, knockback, invincibilityTime, DeathPit);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isCharging)
        {
            if (collision.gameObject.layer == 3) // player
            {
                Vector2 relativePosition = transform.position - collision.transform.position;
                float knockbackVal = 1;
                if (relativePosition.y > Math.Abs(relativePosition.x) / .9f)
                {
                    knockbackVal = 1.25f;
                }
                DamagePlayer(1, relativePosition.normalized, knockbackVal);
            }
            if (collision.gameObject.layer == 12) // death pit
            {
                //Vector2 relativePosition = transform.position - collision.transform.position;
                DamagePlayer(16, new Vector2(0, 0), 0, 0, true);
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
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isCharging)
        {
            if (collision.gameObject.layer == 8) //Player Bullet
            {
                Vector2 relativePosition = transform.position - collision.transform.position;
                DamagePlayer(1, relativePosition.normalized, .5f);
            }
        }
    }
    public void trackPlayerWithGun()
    {
        aimTarget = (Vector2)playerTransform.position - myRigidbody2D.position;
    }
    public void SetAimTarget(Vector2 newTarget)
    {
        aimTarget = newTarget;
    }
    private void UpdateAimTarget()
    {
        float degreesToTurn = Time.fixedDeltaTime * Mathf.Deg2Rad * turningRate;
        if (aimingTimer >= timeToUpdate)
        {
            Vector3 newTarget = Vector3.RotateTowards(currentAim, aimTarget, degreesToTurn, 0f);
            if (currentAim == (Vector2)newTarget) repeatingAimCounter++;
            else repeatingAimCounter = 0;
            currentAim = new Vector2(newTarget.x, newTarget.y);
            if(Vector2.Angle(currentAim,aimTarget) <= .03f || repeatingAimCounter >= 10)
            {
                finishedAiming = true;
            }
            aimingTimer = 0;
        }
        else
        {
            aimingTimer += Time.fixedDeltaTime;
        }
    }
    protected override void handleGunOrientation()
    {
        BulletSpawner.transform.right = currentAim;
        myMagnetManagerScript.setMagnetSpawnerAngle(currentAim);
        torsoFacingRight = playerAnimationManagerScript.setFiringAngle(Mathf.Atan2(currentAim.y, currentAim.x) * Mathf.Rad2Deg);
    }

    public void StartShooting()
    {
        bulletSpawnerScript.ResetTimer();
        shootingInput = true;
    }
    public void StopShooting()
    {
        shootingInput = false;
    }

    public void Move(float xInput, float yInput)
    {
        direction = xInput;
        verticalDirection = yInput;
    }
    public void StartFlying()
    {
        jumpPressed = true;
        lastJumpInputTime = Time.time;
    }
    public void StopFlying()
    {
        jumpPressed = false;
    }

    public void LaunchMagnet()
    {
        launchMagnet = true;
    }
    public void StartRepel()
    {
        //lastRepelInputTime = Time.time;
        repelButtonHeld = true;
        //if (ctx.started)
        //{
        //    repelButtonMostRecent = true;
        //    attractButtonMostRecent = false;
        //}
        //if (ctx.canceled)
        //{
        //    repelButtonMostRecent = false;
        //}
    }
    public void StopRepel()
    {
        repelButtonHeld= false;
    }
    public void StartAttract()
    {
        attractButtonHeld = true;
    }
    public void StopAttract()
    {
        attractButtonHeld = false;
    }
    public void RecoverMagnet()
    {
        recoverMagnet = true;
    }
    public void MagnetRetrieved()
    {
        recoverMagnet = false;
    }
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
}
