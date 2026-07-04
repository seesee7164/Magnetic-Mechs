using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetProjectileScript : MonoBehaviour
{
    [Header("Componenets")]
    private LayerMask groundLayer;
    private CapsuleCollider2D capsuleCollider;
    //code for the magnet projectile
    private float deathTime;
    private float startCollidingTime;
    private Rigidbody2D myRigidBody;
    private bool attached;
    public MagnetSpawnerScript myMagnetSpawnerScript;
    public GameObject player;
    [Header("Return to Player")]
    public bool returnToPlayer = false;
    private float speed = 40f;
    private float retrievalDistance = 2f;
    
    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        myRigidBody = GetComponent<Rigidbody2D>();
        groundLayer = LayerMask.GetMask("Ground");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 17)
        {
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                Attach();
                myRigidBody.bodyType = RigidbodyType2D.Kinematic;
                gameObject.transform.SetParent(collision.transform);
            }
            else if (collision.gameObject.CompareTag("NonStickPlatform"))
            {
                DestroyThis();
            }
            else
            {
                Attach();
            }
        }
    }
    private void Update()
    {
        //if(!attached && Time.time > deathTime)
        //{
        //    DestroyThis();
        //}
    }
    private void FixedUpdate()
    {
        if (returnToPlayer)
        {
            Vector2 velocity = (player.transform.position - transform.position).normalized;
            myRigidBody.linearVelocity = velocity * speed;
            transform.right = -velocity;
            if((player.transform.position - transform.position).magnitude <= retrievalDistance)
            {
                returnToPlayer = false;
                DestroyThis();
                myMagnetSpawnerScript.EnableShooting();
            }
        }
    }
    public void FullReset()
    {
        capsuleCollider.enabled = true;
        //deathTime = Time.time + 5;
        Reset();
    }
    public void Reset()
    {
        myRigidBody.bodyType = RigidbodyType2D.Dynamic;
        gameObject.transform.parent = null;
        attached = false;
    }
    public void DestroyThis()
    {
        gameObject.SetActive(false);
        if(myMagnetSpawnerScript != null)
        {
            myMagnetSpawnerScript.magnetActive = false;
        }
    }
    public void Attach()
    {
        myRigidBody.linearVelocity = Vector3.zero;
        capsuleCollider.enabled = false;
        attached = true;
        //Vector2 myTransform = (Vector2) transform;
        //Vector2 startPos = (Vector2)transform.position + (Vector2) transform.right * .35f + (Vector2) transform.up * .25f;
        //RaycastHit2D hit = Physics2D.Raycast(startPos, -(Vector2)transform.up, .5f, groundLayer);
        Vector2 startPos = (Vector2)transform.position - (Vector2)transform.right * .35f;
        RaycastHit2D hit = Physics2D.Raycast(startPos, (Vector2)transform.right + (Vector2)transform.up * .3f, 1f, groundLayer);
        if(hit.collider == null)
        {
            hit = Physics2D.Raycast(startPos, (Vector2)transform.right - (Vector2)transform.up * .3f, 1f, groundLayer);
        }
        if(hit.collider != null)
        {
            Vector2 newNormal = hit.normal;
            float newAngle = Mathf.Atan2(newNormal.y, newNormal.x) * Mathf.Rad2Deg;
            Quaternion newRotation = Quaternion.Euler(0, 0, newAngle + 180);
            transform.rotation = newRotation;
            transform.position = hit.point;
        }
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    //Vector2 startPos = (Vector2) transform.position + Vector2.right * .35f + Vector2.up * .25f;
    //    //Gizmos.DrawLine(startPos, startPos - (Vector2) transform.up * .5f);
    //    Vector2 startPos = (Vector2)transform.position - (Vector2)transform.right * .35f;
    //    Gizmos.DrawLine(startPos, startPos + (Vector2) transform.right * 1f);
    //}
    public void RecoverMagnet()
    {
        Reset();
        returnToPlayer = true;
    }
}
