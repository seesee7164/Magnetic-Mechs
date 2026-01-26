using UnityEngine;
using System;

public class PlayerPhysicsScript : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D playerRigidBody;
    [Header("Dust Effects")]
    public ParticleSystem changeDirectionDust;
    [Header("Gravity")]
    private float linearDrag = 3f;
    public float defaultGravity = 1f;
    private float fallMultiplier = 3f;
    private float pressingDownGravityMultiplier = 2.5f;
    [Header("Drag Values")]
    private float defaultDrag = .05f;
    private float clampXDrag = 2.0f;
    private float clampYDrag = 2.0f;
    private float pressingDownDragMultiplier = .3f;
    [Header("Variables")]
    private bool downPressed = false;
    [Header("Scripts")]
    public MagnetManagerScript magnetManagerScript;
    public PlayerScript playerScript;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyMaxJetpackSpeed()
    {
        if (playerRigidBody.linearDamping < clampYDrag) playerRigidBody.linearDamping = clampYDrag / 2;
    }
    public void ApplyMaxHorizontalSpeedDrag(float currentMaxSpeed)
    {
        if (MathF.Abs(playerRigidBody.linearVelocity.x) > currentMaxSpeed && playerRigidBody.linearDamping < clampXDrag)
        {
            //myRigidbody2D.velocity = new Vector2(MathF.Sign(myRigidbody2D.velocity.x) * currentMaxSpeed, myRigidbody2D.velocity.y);
            playerRigidBody.linearDamping = clampXDrag;
        }
    }
    public void ApplyMaxVerticalSpeedDrag(float currentMaxSpeed)
    {
        if (MathF.Abs(playerRigidBody.linearVelocity.y) > currentMaxSpeed && playerRigidBody.linearDamping < clampYDrag)
        {
            //myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, MathF.Sign(myRigidbody2D.velocity.y) * currentMaxSpeed);
            playerRigidBody.linearDamping = clampYDrag;
        }
    }
    public void modifyPhysics(bool jetpackOn, float direction, bool trulyOnGround)
    {
        bool changingDirection = (direction > 0 && playerRigidBody.linearVelocity.x < 0) || (direction < 0 && playerRigidBody.linearVelocity.x > 0);
        if (trulyOnGround)
        {
            playerRigidBody.gravityScale = 0;
            if (MathF.Abs(direction) == 0f || changingDirection)
            {
                playerRigidBody.linearDamping = linearDrag * 2.5f;
                if (changingDirection)
                {
                    CreateDust();
                }
            }
            else
            {
                playerRigidBody.linearDamping = defaultDrag;
            }
        }
        else
        {
            float appliedGravity = defaultGravity;
            //playerRigidBody.gravityScale = defaultGravity;
            playerRigidBody.linearDamping = linearDrag * .15f;
            if (jetpackOn)
            {
                appliedGravity = defaultGravity / 2;
                if (playerRigidBody.linearVelocity.y < 0f)
                {
                    playerRigidBody.linearDamping = linearDrag;
                }
            }
            else
            {
                if (playerRigidBody.linearVelocity.y < 0f)
                {
                    appliedGravity = defaultGravity * fallMultiplier;
                }
                else
                {
                    appliedGravity = defaultGravity * fallMultiplier / 2;
                    if (downPressed)
                    {
                        playerRigidBody.linearDamping = linearDrag * pressingDownDragMultiplier;
                    }
                }
                if (magnetManagerScript.magnetActive && magnetManagerScript.returnMagnetMaximumDistance() > magnetManagerScript.magnetDistance)
                {
                    bool attractingUp = MathF.Abs(magnetManagerScript.returnPolarMagnetAngle()) >= 90 && magnetManagerScript.attracting;
                    bool repellingDown = MathF.Abs(magnetManagerScript.returnPolarMagnetAngle()) < 90 && !magnetManagerScript.attracting;
                    if (attractingUp || repellingDown)
                    {
                        float distanceEffect = (magnetManagerScript.returnMagnetMaximumDistance() - magnetManagerScript.magnetDistance) / magnetManagerScript.returnMagnetMaximumDistance();
                        float angleEffect = Mathf.Abs(Mathf.Cos(magnetManagerScript.returnPolarMagnetAngle() * Mathf.Deg2Rad));
                        appliedGravity = defaultGravity * fallMultiplier / 2 * (4 - 2 * distanceEffect - angleEffect) / 4;
                        //playerRigidBody.gravityScale = defaultGravity / 2 * (4 - 3 * );
                    }
                }
                if (downPressed)
                {
                    appliedGravity *= pressingDownGravityMultiplier;
                }
            }
            playerRigidBody.gravityScale = appliedGravity;
        }
    }
    public void SetDownPressed(float verticalDirection)
    {
        if(verticalDirection <= -.25f)
        {
            downPressed = true;
        }
        else
        {
            downPressed = false;
        }
    }
    private void CreateDust()
    {
        changeDirectionDust.Play();
    }
}
