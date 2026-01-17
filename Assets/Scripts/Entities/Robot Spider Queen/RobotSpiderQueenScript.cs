using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class RobotSpiderQueenScript : MonoBehaviour
{
    //main script for the Robot Spider Queen
    [Header("Horizontal Movement")]
    public float speed = 3;
    public bool facingRight;
    [Header("Variables")]
    private float bulletDamage = 1f;
    public float chargeDamage = 1f;
    public float distanceBeforeWalking = 2f;
    public bool bossActive;
    public LayerMask groundLayer;
    public bool movementEnabled;
    public float flashTime = .2f;
    [Header("Big Attacks")]
    private bool attackActive = false;
    public float timeBetweenAttacks = 10f;
    public float attackTimer;
    private float attackChangeModifier = .2f;
    private float currentAttackModifier = 0;
    private bool previousAttack = false;
    [Header("Components")]
    public SpiderQueenHealthScript robotSpiderQueenHealthScript;
    public Animator animator;
    public Transform playerTransform;
    public Rigidbody2D myRigidbody2D;
    public SpriteRenderer sprite;
    public AudioSource backgroundMusic;
    public AudioManager audioManager;
    [Header("Sensors")]
    public float horizontalCheckLength = .65f;
    public float groundCheckHeight = 1f;
    private bool groundInFront = true;
    [Header("manage Stages")]
    public RockSpawnerScript rockSpawnerScript;
    public GoomechSpawnerScript goomechSpawnerScriptTop;
    public GoomechSpawnerScript goomechSpawnerScriptBottom;
    public SpiderQueenBulletSpawnerScript spiderQueenBulletSpawnerScript;
    public WideAttackScript wideAttackScript;
    public LaserScript laserScript;
    void Awake()
    {
        GameObject robotSpiderQueenHealth= GameObject.FindGameObjectWithTag("RobotSpiderQueenHealth");
        if (robotSpiderQueenHealth != null) robotSpiderQueenHealthScript = robotSpiderQueenHealth.GetComponent<SpiderQueenHealthScript>();
        else Debug.Log("Spider Queen Health could not be found");
        animator = GetComponent<Animator>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.GetComponent<Transform>();
        else Debug.Log("player could not be found");
        myRigidbody2D = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        groundLayer = LayerMask.GetMask("Ground", "Plank Ground");
        movementEnabled = true;
        attackTimer = 0f;
        bossActive = false;
    }
    private void Update()
    {
        //handles walking
        if (!bossActive) return;
        movementEnabled = true;
        groundInFront = Physics2D.Raycast(transform.position + Vector3.right * horizontalCheckLength * (facingRight ? 1 : -1), Vector2.down, groundCheckHeight, groundLayer);
        float xDistanceFromPlayer = (playerTransform ==null ? 24 :  playerTransform.position.x)- transform.position.x;
        if (!groundInFront||Mathf.Abs(xDistanceFromPlayer) < distanceBeforeWalking)
        {
            movementEnabled = false;
            animator.SetBool("isWalking",false);
        }
        else animator.SetBool("isWalking", true);
        if (xDistanceFromPlayer < 0 && facingRight)
        {
            Flip();
        }
        if (xDistanceFromPlayer > 0 && !facingRight)
        {
            Flip();
        }
    }
    void FixedUpdate()
    {
        if (!bossActive)
        {
            animator.SetBool("hasDied", false);
            return;
        }
        if (movementEnabled)
        {
            Vector2 moveHorizontal = Vector2.right * speed;
            if (!facingRight) moveHorizontal.x *= -1;
            myRigidbody2D.MovePosition(myRigidbody2D.position + moveHorizontal * Time.fixedDeltaTime);
        }
        if (!attackActive)
        {
            if(attackTimer >= timeBetweenAttacks)
            {
                triggerBigAttack();
            }
            else
            {
                attackTimer += Time.fixedDeltaTime;
            }
        }
    }
    private void triggerBigAttack()
    {
        //triggers one of the two big attacks (the cone or sweeping laser)
        attackTimer = 0;
        float randDecimal = Random.Range(0.0f, 1.0f);
        randDecimal += currentAttackModifier;
        bool attackToUse = randDecimal >= .5f;
        if (attackToUse)
        {
            ShootWideAttack();
            if(currentAttackModifier < .0)
            {
                currentAttackModifier = 0;
            }
            currentAttackModifier -= attackChangeModifier;
        }
        else
        {
            ShootLaser();
            if (currentAttackModifier > 0)
            {
                currentAttackModifier = 0;
            }
            currentAttackModifier += attackChangeModifier;
        }
        previousAttack = attackToUse;
    }
    public void Flip()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            damageBoss(bulletDamage);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3 && playerTransform.GetComponent<PlayerScript>().isCharging)
        {
            damageBoss(chargeDamage);
        }
    }
    public void damageBoss(float damage)
    {
        if (robotSpiderQueenHealthScript == null || !bossActive) return;
        robotSpiderQueenHealthScript?.takeDamage(damage);
        StartCoroutine(FlashRed());
    }
    public IEnumerator FlashRed()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        sprite.color = Color.white;
    }
    public void KillBoss()
    {
        if (!bossActive)
        {
            return;
        }
        bossActive = false;
        animator.SetBool("hasDied", true);
        myRigidbody2D.linearVelocity = new Vector3(0, 0, 0);
        TurnOffAttacks();
        StartCoroutine(HandleDeath());
    }
    IEnumerator HandleDeath()
    {
        yield return new WaitUntil(() => gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Dead"));
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
    private void TurnOffAttacks()
    {
        goomechSpawnerScriptTop.bossActive = false;
        goomechSpawnerScriptBottom.bossActive = false;
        spiderQueenBulletSpawnerScript.DisableShooting();
        rockSpawnerScript.BossDied();
        laserScript.BossDied();
        wideAttackScript.BossDied();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position + Vector3.right * (horizontalCheckLength - .01f) * (facingRight ? 1 : -1), transform.position + Vector3.right * horizontalCheckLength * (facingRight ? 1 : -1));
        Gizmos.DrawLine(transform.position + Vector3.right * horizontalCheckLength * (facingRight ? 1 : -1), transform.position + Vector3.right * horizontalCheckLength * (facingRight ? 1 : -1) + Vector3.down * groundCheckHeight);
    }

    //Manage Stages
    
    public void TriggerAllStage2()
    {
        rockSpawnerScript.TriggerStage2();
        goomechSpawnerScriptTop.TriggerStage2();
        goomechSpawnerScriptBottom.TriggerStage2();
        spiderQueenBulletSpawnerScript.TriggerStage2();
        wideAttackScript.TriggerStage2();
        laserScript.TriggerStage2();
        AudioClip loadedClip = Resources.Load<AudioClip>("BackgroundMusic/the_robot_spider_queen_invasion_Part3");
        if (backgroundMusic != null && loadedClip != null && audioManager != null)
        {
            backgroundMusic.clip = loadedClip;
            backgroundMusic.Play();
            //StartCoroutine(SwapMusic(loadedClip));
        }
    }
    //public IEnumerator SwapMusic(AudioClip loadedClip)
    //{
    //    audioManager.fade(1.25f);
    //    yield return new WaitForSeconds(1.5f);
    //    audioManager.stopFade();
    //    backgroundMusic.clip = loadedClip;
    //    backgroundMusic.Play();
    //}
    public void ShootWideAttack()
    {
        wideAttackScript.startLaser();
    }
    public void ShootLaser()
    {
        laserScript.setInitialAngle();
    }
    public void startBigAttack()
    {
        attackActive = true;
        spiderQueenBulletSpawnerScript.DisableShooting();
    }
    public void endBigAttack()
    {
        attackActive = false;
        spiderQueenBulletSpawnerScript.EnableShooting();
    }
    public void startLaserAttack()
    {
        attackActive = true;
    }
    public void endLaserAttack()
    {
        attackActive = false;
    }
}
