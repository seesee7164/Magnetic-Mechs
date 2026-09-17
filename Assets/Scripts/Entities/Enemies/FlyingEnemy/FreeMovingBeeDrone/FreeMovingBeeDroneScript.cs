using UnityEngine;
using System.Collections;

public class FreeMovingBeeDroneScript : MonoBehaviour
{
    [Header("Tracking Player")]
    public Transform playerTransform;
    private float xOffsetFromPlayer = 15f;
    private float xSpeedMultiplier = 4.0f;
    private float ySpeedMultiplier = 2.0f;

    [Header("Components")]
    public Rigidbody2D myRigidBody2D;
    public BoxCollider2D myCollider;
    public Animator animator;
    public SpriteRenderer sprite;
    public AudioSource DeathSound;
    public FreeMovingBeeDroneSpawnerScript beeDroneRespawnerScript;
    public FlyingEnemyBulletSpawnerScript flyingEnemyBulletSpawnerScript;

    [Header("Statistics")]
    public float health;
    private float startingHealth = 1;
    public int index = 0;
    private bool isAlive = true;
    private bool movementEnabled = true;
    public bool despawnBehavior = false;
    public bool facingRight = false;

    private void Awake()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        myCollider = GetComponent<BoxCollider2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        sprite = GetComponent<SpriteRenderer>();
        health = startingHealth;
        movementEnabled = true;
    }
    private void FixedUpdate()
    {
        if (!movementEnabled) return;
        if (despawnBehavior) Despawn();
        else TrackPlayer();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            TakeDamage(1);
        }

        if (collision.gameObject.layer == 3 && playerTransform.GetComponent<PlayerScript>().isCharging)
        {
            TakeDamage(1);
        }
    }
    private void TrackPlayer()
    {
        float targetX = playerTransform.position.x + xOffsetFromPlayer;
        float xSpeed = (targetX - transform.position.x) * xSpeedMultiplier;
        float ySpeed = (playerTransform.position.y - transform.position.y) * ySpeedMultiplier;
        myRigidBody2D.linearVelocity = new Vector2(xSpeed, ySpeed);
        //myRigidBody2D.AddForce(new Vector2(appliedForce * 15, 0));
    }
    public void restartDrone()
    {
        health = startingHealth;
        animator.Play("Movement");
        isAlive = true;
        myCollider.enabled = true;
        movementEnabled = true;
        despawnBehavior = false;
        if (flyingEnemyBulletSpawnerScript != null)
        {
            flyingEnemyBulletSpawnerScript.EnemyUnkilled();
        }
    }

    void TakeDamage(float Damage)
    {
        health -= Damage;
        if (health <= 0)
        {
            KillFlyingEnemy();
        }
        else
        {
            StartCoroutine(damageFlash());
        }
    }
    IEnumerator damageFlash()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(.25f);
        sprite.color = Color.white;
    }
    void KillFlyingEnemy()
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
        myCollider.enabled = false;
        myRigidBody2D.gravityScale = 0;
        flyingEnemyBulletSpawnerScript.EnemyKilled();
        StartCoroutine(HandleDeath());
    }
    IEnumerator HandleDeath()
    {
        yield return new WaitUntil(() => gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Dead"));
        animator.SetBool("hasDied", false);
        gameObject.SetActive(false);
        if (beeDroneRespawnerScript != null)
        {
            beeDroneRespawnerScript.DroneKilled(index);
        }
    }
    public bool IsAlive()
    {
        return isAlive;
    }
    public void Flip()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
        //myRigidBody2D.linearVelocity = new Vector3(-myRigidBody2D.linearVelocity.x, myRigidBody2D.linearVelocity.y, 0);
    }
    private void Despawn()
    {
        float targetX = playerTransform.position.x + 2.0f * xOffsetFromPlayer;
        float xSpeed = (targetX - transform.position.x) * xSpeedMultiplier/8f;
        float ySpeed = (playerTransform.position.y - transform.position.y) * ySpeedMultiplier;
        myRigidBody2D.linearVelocity = new Vector2(xSpeed + 13, ySpeed);
        if (transform.position.x >= playerTransform.position.x + 1.5f * xOffsetFromPlayer)
        {
            if (beeDroneRespawnerScript != null)
            {
                beeDroneRespawnerScript.DroneKilled(index);
            }
            gameObject.SetActive(false);
        }
    }
}
