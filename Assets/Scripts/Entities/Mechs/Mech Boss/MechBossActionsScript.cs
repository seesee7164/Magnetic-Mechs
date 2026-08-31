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
        Vector2 mouseRelativePosition = (Vector2)playerTransform.position - myRigidbody2D.position;
        BulletSpawner.transform.right = mouseRelativePosition;
        myMagnetManagerScript.setMagnetSpawnerAngle(mouseRelativePosition);
        torsoFacingRight = playerAnimationManagerScript.setFiringAngle(Mathf.Atan2(mouseRelativePosition.y, mouseRelativePosition.x) * Mathf.Rad2Deg);
    }
    protected override void handleGunOrientation()
    {

    }

    public void StartShooting()
    {
        shootingInput = true;
    }
    public void StopShooting()
    {
        shootingInput = false;
    }

    //public void Move(InputAction.CallbackContext context)
    //{
    //    Vector2 input = context.ReadValue<Vector2>();
    //    direction = input.x;
    //    verticalDirection = input.y;
    //    if (context.performed && Mathf.Abs(input.x) > 0.1f)
    //    {
    //        lastMoveInputTime = Time.time;
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
