using UnityEngine;
using UnityEngine.UI;
using System;


public class VerticalMovementScript : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D playerRigidBody;
    public Image remainingFuelImage;
    public GameObject remainingFuelParent;

    [Header("Scripts")]
    public PlayerAnimationManagerScript playerAnimationManagerScript;
    public PlayerPhysicsScript playerPhysicsScript;
    public PlayerScript playerScript;
    public GroundCheckScript playerGroundCheckScript;

    [Header("Variables")]
    private float maxYSpeed = 20f;
    private float maxYSpeedPressingDown = 32f;
    private bool trulyOnGround;
    private bool recentlyOnGround;

    [Header("Timers")]
    private float remainingFuelTimer = 0;
    private float remainingFuelTimeToDisappear = .5f;

    [Header("Jumping")]
    public bool jumpPressed = false;
    public float jumpTimer;
    private float jumpDelay = .15f;
    public float maxYSpeedTimer;
    private float maxYSpeedDelay = .7f;
    private float jumpForce = 7f;

    [Header("Jetpack")]
    private float jetpackTotalTime = 1.0f;
    public float jetpackCurrentTime;
    private float jetPackForce = 12f;
    private float maxJetSpeed = 19f;
    private float jetpackRecoveryTimer = 0f;
    private float jetpackRecoveryTime = 0.25f;
    private float jetPackTimeRecoveryMultiplier = .85f;
    private float slowSpeedMultiplyer = 1.4f;
    private bool jetpackOn;
    public AudioSource jetpackAudio;

    [Header("Overused Jetpack")]
    private bool jetpackAvailable = true;
    private float jetpackRecoveryRatio = .75f;

    [Header("Jetpack Components")]
    public GameObject jetpackLower;
    public GameObject jetpackBackwards;
    public bool jetpackBackwardsOn;

    void Awake()
    {
        jetpackCurrentTime = jetpackTotalTime;
    }

    public bool handleVerticalUpdates(float verticalDirection, bool playerAlive, bool jump)
    {
        jumpPressed = jump;
        playerGroundCheckScript.setGroundLayer(verticalDirection);
        playerPhysicsScript.SetDownPressed(verticalDirection);
        trulyOnGround = playerGroundCheckScript.isTrulyOnGround();
        bool nearGround = playerGroundCheckScript.isNearGround();
        recentlyOnGround = playerGroundCheckScript.recentlyGrounded();
        if (playerAlive)
        {
            //playerAnimationManagerScript.setLanding(trulyOnGround, onGround, myRigidbody2D.linearVelocity.y);
            playerAnimationManagerScript.setLanding(nearGround);
        }
        //Method One Recover on Ground
        if (trulyOnGround)
        {
            jetpackCurrentTime = jetpackTotalTime;
            handleJetPackTime();
        }
        checkReenableJetpack();
        jetpackOn = jumpPressed && (!trulyOnGround) && jetpackAvailable;
        if (jetpackOn)
        {
            if (jetpackCurrentTime <= 0)
            {
                jetpackAvailable = false;
                jetpackOn = false;
            }
            else
            {
                jetpackCurrentTime -= Time.deltaTime;
                jetpackRecoveryTimer = 0f;
                handleJetPackTime();
            }
        }
        //Method Two Recover whenever jetpack isn't in use
        if (!jetpackOn && jetpackCurrentTime < jetpackTotalTime)
        {
            jetpackRecoveryTimer += Time.deltaTime;
            if (jetpackRecoveryTimer > jetpackRecoveryTime)
            {
                jetpackCurrentTime += Time.deltaTime * 1.4f;
            }
            handleJetPackTime();
        }
        if (jumpPressed)
        {
            jumpTimer = Time.time + jumpDelay;
            if (recentlyOnGround)
            {
                maxYSpeedTimer = Time.time + maxYSpeedDelay;
            }
        }
        if (jetpackAudio != null)
        {
            if (jetpackOn)
            {
                if (!jetpackAudio.isPlaying) jetpackAudio.Play();
            }
            else
            {
                jetpackAudio.Stop();
            }
        }
        return jetpackOn;
    }

    public void handleJetPackTime()
    {
        int remainingFuelPercent = (int)(100 * jetpackCurrentTime / jetpackTotalTime);
        if (remainingFuelPercent < 0)
        {
            remainingFuelPercent = 0;
        }
        remainingFuelImage.fillAmount = remainingFuelPercent / 100.0f;
    }
    public void handleRemainingFuelBar()
    {
        if (jetpackCurrentTime >= jetpackTotalTime)
        {
            remainingFuelTimer += Time.deltaTime * jetPackTimeRecoveryMultiplier;
            if (remainingFuelTimer > remainingFuelTimeToDisappear)
            {
                remainingFuelParent.SetActive(false);
            }
        }
        else
        {
            remainingFuelParent.SetActive(true);
            remainingFuelTimer = 0;
        }
    }

    public void SetJetpackSprites(float direction, float verticalDirection)
    {
        bool downPressed = verticalDirection <= -.25f;
        jetpackLower.GetComponent<JetpackScript>().setJetpackDown(jetpackOn, downPressed, trulyOnGround);
        //jetpackLowerRight.GetComponent<JetpackScript>().setJetpack(jetpackOn);
        jetpackBackwardsOn = !trulyOnGround && Mathf.Abs(direction) > 0;
        jetpackBackwards.GetComponent<JetpackScript>().setJetpackBack(jetpackBackwardsOn);
    }
    public void handleVerticalMovement()
    {
        //handles checks and related to vertical movement once every Update cycle
        if (recentlyOnGround && jumpTimer > Time.time)
        {
            jump();
        }
        if (!trulyOnGround && jetpackOn)
        {
            float currentJetPackForce = jetPackForce;
            if (playerRigidBody.linearVelocity.y < 5)
            {
                currentJetPackForce *= slowSpeedMultiplyer;
            }
            playerRigidBody.AddForce(new Vector2(0, currentJetPackForce));

            if (playerRigidBody.linearVelocity.y > maxJetSpeed && maxYSpeedTimer < Time.time)
            {
                if (playerScript.repelOn || playerScript.attractOn)
                {
                    return;
                }
                playerPhysicsScript.ApplyMaxJetpackSpeed();
                //myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, maxJetSpeed);
            }
        }
        adjustMaxYSpeed();
    }
    void adjustMaxYSpeed()
    {
        //turns on damping if the player is above a certain y speed
        float currentMaxSpeed = playerScript.startMagnetMaxYSpeed(maxYSpeed, maxYSpeedPressingDown);
        playerPhysicsScript.ApplyMaxVerticalSpeedDrag(currentMaxSpeed);
    }

    private void jump()
    {
        playerRigidBody.linearVelocity = new Vector2(playerRigidBody.linearVelocity.x, 0);
        playerRigidBody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        //jumpSound.Play();
        //Instantiate(JumpDust, new Vector3(transform.position.x, transform.position.y - groundLength * 3 / 4, transform.position.z), transform.rotation);
        jumpTimer = 0;
    }

    private void checkReenableJetpack()
    {
        if((jetpackCurrentTime/jetpackTotalTime >= jetpackRecoveryRatio) || playerScript.lastJumpInputTime >= Time.time -.1f)
        {
            jetpackAvailable = true;
        }
    }

    public void PlayerKilled()
    {
        if (jetpackAudio != null && jetpackAudio.isPlaying) jetpackAudio.Stop();
        jetpackLower.GetComponent<JetpackScript>().setJetpackDown(false, false, true);
        //jetpackLowerRight.GetComponent<JetpackScript>().setJetpack(false);
        jetpackBackwards.GetComponent<JetpackScript>().setJetpackBack(false);
    }
    
    public bool returnJetpackOn()
    {
        return jetpackOn;
    }
}
