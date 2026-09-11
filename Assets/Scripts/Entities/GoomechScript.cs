using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class GoomechScript : MonoBehaviour
{
    //code for the goomech
    [Header("Starting Behavior")]
    public bool startingBehavior = false;
    public float startingTime = 2f;
    private float startingSpeed = .9f;
    public int index;
    public bool initialFaceRight = true;

    [Header("Horizontal Movement")]
    public float speed = 3;
    public bool facingRight;

    [Header("Tracking Player")]
    public Transform playerTransform;
    public float reticleDistance = 10;

    [Header("Components")]
    public GameObject targetingReticle;
    public Rigidbody2D myRigidBody2D;
    public SpriteRenderer sprite;
    public LayerMask groundLayer;
    public LayerMask spikeLayer;
    public LayerMask enemyLayer;
    public BoxCollider2D myCollider;
    public Animator animator;
    public PlayPromptScript shootingPromptScript;
    public AudioSource DeathSound;
    public GoomechSpawnerScript GoomechSpawnerScript;
    public GameObject takeDamageWhenSpawningCollider;

    [Header("Sensors")]
    public float horizontalCheckLength = .65f;
    public float groundCheckHeight = 1f;
    protected bool approachingWall = false;
    protected bool approachingEnemy = false;
    protected bool approachingSpike = false;
    protected bool onGround = true;

    [Header("Statistics")]
    public float health;
    private float startingHealth = 1;
    protected bool isAlive = true;
    public bool includePrompt = false;

    [Header("Knockback")]
    private float knockbackTime = 0.20f;
    public bool movementEnabled;

    protected void Awake()
    {
        if (includePrompt)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player!= null)playerTransform = player.GetComponent<Transform>();
            GameObject shootingScript = GameObject.FindGameObjectWithTag("ShootingPrompt");
            shootingPromptScript = shootingScript.GetComponent<PlayPromptScript>();
        }
        myRigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<BoxCollider2D>();
        facingRight = true;
        health = startingHealth;
        movementEnabled = true;
        groundLayer = LayerMask.GetMask("Ground", "Plank Ground", "Wall");
        spikeLayer = LayerMask.GetMask("Spike");
        enemyLayer = LayerMask.GetMask("Enemy");
        StartCoroutine(FreezeY());
    }

    // Update is called once per frame
    void Update()
    {
        if (startingBehavior)
        {
            SpawnBehavior();
            return;
        }
        approachingWall = Physics2D.Raycast(transform.position, facingRight ? Vector2.right : Vector2.left, horizontalCheckLength, groundLayer);
        approachingSpike = Physics2D.Raycast(transform.position, facingRight ? Vector2.right : Vector2.left, horizontalCheckLength *1.65f, spikeLayer);
        approachingEnemy = Physics2D.Raycast(transform.position + Vector3.right * (horizontalCheckLength - .01f) * (facingRight ? 1 : -1), facingRight ? Vector2.right : Vector2.left, horizontalCheckLength, enemyLayer);
        onGround = Physics2D.Raycast(transform.position + Vector3.right * horizontalCheckLength * (facingRight ? 1 : -1), Vector2.down, groundCheckHeight, groundLayer);
        if (approachingWall || approachingEnemy || approachingSpike || !onGround)
        {
            Flip();
        }
        if (movementEnabled)
        {
            myRigidBody2D.linearVelocity = new Vector3(speed * (facingRight ? 1 : -1), myRigidBody2D.linearVelocity.y, 0);
        }
        if(includePrompt) handleTargetingReticle();
    }
    private void FixedUpdate()
    {
        //animator.SetBool("hasDied", false);
    }
    protected virtual IEnumerator FreezeY()
    {
        yield return new WaitForSeconds(1f);
        myRigidBody2D.constraints = RigidbodyConstraints2D.FreezePositionY;
    }
    public void SpawnBehavior()
    {
        if (!isAlive)
        {
            myRigidBody2D.linearVelocity = Vector3.zero;
            return;
        }
        myRigidBody2D.linearVelocity = new Vector3(startingSpeed * (facingRight ? 1 : -1), myRigidBody2D.linearVelocity.y, 0);
    }
    public void startSpawnBehavior()
    {
        //starts the traits that are active for a few seconds after a goomech is spawned
        myCollider.enabled = false;
        takeDamageWhenSpawningCollider.SetActive(true);
        myRigidBody2D.gravityScale = 0;
        startingBehavior = true;
        animator.Play("Run");
        movementEnabled = true;
        isAlive = true;
    }
    public IEnumerator endSpawnBehavior()
    {
        //ends the traits that are active for a few seconds after a goomech is spawned
        yield return new WaitForSeconds(startingTime);
        myCollider.enabled = true;
        takeDamageWhenSpawningCollider.SetActive(false);
        myRigidBody2D.gravityScale = 1;
        startingBehavior = false;
    }
    public void Flip()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
        myRigidBody2D.linearVelocity = new Vector3(-myRigidBody2D.linearVelocity.x, myRigidBody2D.linearVelocity.y, 0);
    }
    protected void handleTargetingReticle()
    {
        //turns the targeting prompt on and off during the tutorial
        if (playerTransform == null||shootingPromptScript == null) return;
        if (isAlive && (Vector2.Distance((Vector2)playerTransform.position, myRigidBody2D.position) <= reticleDistance))
        {
            targetingReticle.GetComponent<SpriteRenderer>().enabled = true;
            shootingPromptScript.playPrompt();
        }
        else
        {
            targetingReticle.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            TakeDamage(1, collision.transform.up.normalized, .25f);
        }

        if (collision.gameObject.layer == 3 && playerTransform!=null && playerTransform.GetComponent<PlayerScript>().isCharging)
        {
            TakeDamage(1, collision.transform.up.normalized, .25f);
        }
    }
    public void TakeDamage(float Damage, Vector2 knockbackDirection, float knockback)
    {
        health -= Damage;
        if (health <= 0)
        {
            KillGoomech();
        }
        else
        {
            StartCoroutine(handleKnockback(knockback, knockbackDirection));
        }
    }
    IEnumerator handleKnockback(float knockback, Vector2 knockbackDirection)
    {
        movementEnabled = false;
        myRigidBody2D.AddForce(knockbackDirection * knockback, ForceMode2D.Impulse);
        sprite.color = Color.red;
        yield return new WaitForSeconds(knockbackTime);
        movementEnabled = true;
        sprite.color = Color.white;
    }
    void KillGoomech()
    {
        if (!isAlive)
        {
            return;
        }
        movementEnabled = false;
        isAlive = false;
        animator.SetBool("hasDied", true);
        myRigidBody2D.linearVelocity = new Vector3(0, 0, 0);
        DeathSound.Play();
        //gameObject.layer = 9;
        //Debug.Log(gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        //Destroy(gameObject, gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        myCollider.enabled = false;
        myRigidBody2D.gravityScale = 0;
        //Debug.Log(gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        StartCoroutine(HandleDeath());
    }
    IEnumerator HandleDeath()
    {
        yield return new WaitUntil(() => gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Dead"));
        animator.SetBool("hasDied", false);
        if (GoomechSpawnerScript != null)
        {
            GoomechSpawnerScript.GoomechKilled(index);
        }
        gameObject.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.right * (horizontalCheckLength-.01f) * (facingRight ? 1 : -1), transform.position + Vector3.right * horizontalCheckLength * (facingRight ? 1 : -1));
        Gizmos.DrawLine(transform.position + Vector3.right * horizontalCheckLength * (facingRight ? 1 : -1), transform.position + Vector3.right * horizontalCheckLength * (facingRight ? 1 : -1) + Vector3.down * groundCheckHeight);
    }
}
