using UnityEngine;

public class ChargeScript : MonoBehaviour
{

    [Header("Charging")]
    public bool isCharging = false;
    private float chargeTime = 1f;
    private float chargeTimer = 0f;
    private float chargeCooldown = 3f;
    private float chargeCooldownTimer = 0f;
    private float chargeSpeed = 21f;
    [Header("Components")]
    public Sprite chargeIndicatorImage;
    public Sprite invincibilityIndicatorImage;
    public Sprite chargeCooldownIndicatorImage;
    public SpriteRenderer chargeIndicator;
    public Rigidbody2D playerRigidBody;
    [Header("Scripts")]
    public PlayerHealthScript playerHealthScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool handleCharging(bool chargePressed)
    {
        // check if we have hit charging speed
        if (playerRigidBody.linearVelocity.magnitude >= chargeSpeed && !playerHealthScript.invincible)
        {
            // if charging cooldown is over and we aren't charging
            if (!isCharging && chargeCooldownTimer < 0f)
            {
                //chargeIndicator.sprite = chargeIndicatorImage;

                // start charging if we press the button
                if (chargePressed)
                {
                    chargeTimer = chargeTime;

                    isCharging = true;
                    chargeIndicator.sprite = invincibilityIndicatorImage;
                }
            }
        }
        else
        {
            // too slow for charge speed and not charging
            if (!isCharging)
            {
                chargeIndicator.sprite = null;
            }
        }

        // if we stop charging
        if (chargeTimer <= 0f && isCharging)
        {
            isCharging = false;
            chargeCooldownTimer = chargeCooldown;
        }

        // if cooldown is happening
        if (chargeCooldownTimer > 0f)
        {
            chargeIndicator.sprite = chargeCooldownIndicatorImage;
        }

        chargeTimer -= Time.deltaTime;
        chargeCooldownTimer -= Time.deltaTime;
        return isCharging;
    }
}
