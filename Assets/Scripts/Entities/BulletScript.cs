using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    //this is a test
    private float deathTime;
    private float startCollidingTime;
    [Header("Components")]
    public GameObject parent;
    public BulletSpawnerParent bulletSpawnerParent;
    public Animator animator;
    [Header("Variable")]
    public int index;
    public float lifetime = 3;
    protected LayerMask blockBulletLayers;
    protected LayerMask otherBulletLayer;

    //private GameObject explosionEffect;
    //private Vector3 explosionOffset = new Vector3(0, .05f, 0);

    public bool isPlatformMissile;
    [Header("Player")]
    public bool isPlayerBullet = false;

    void Start()
    {
        isPlatformMissile = gameObject.GetComponent<MissilePlatformManager>() != null;
    }
    private void Awake()
    {
        //blockBulletLayers = LayerMask.GetMask("Player", "Enemy", "Rock", "Non Damaging Enemy", "Enemy Mech Boss");
        if (isPlayerBullet)
        {
            blockBulletLayers = LayerMask.GetMask("Enemy", "Rock", "Non Damaging Enemy", "Enemy Mech Boss");
            otherBulletLayer = LayerMask.GetMask("Enemy Bullet");
        }
        else
        {
            blockBulletLayers = LayerMask.GetMask("Player");
            otherBulletLayer = LayerMask.GetMask("Player Bullet");
        }
        
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > deathTime)
        {
            KillBullet();
        }
        //if (Time.time > startCollidingTime)
        //{
        //    GetComponent<CapsuleCollider2D>().enabled = true;
        //}
    }
    public void SetDeathTime()
    {
        //sets time to kill the bullet if it hasn't hit anything yet
        deathTime = Time.time + lifetime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //GameObject effect = Instantiate(explosionEffect, transform.position + explosionOffset, Quaternion.identity);
        //Destroy(effect, effect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        //collision.gameObject.layer != 8 && collision.gameObject.layer != 13 && collision.gameObject.layer != 14 && collision.gameObject.layer != 5 && collision.gameObject.layer != 6
        if (parent!= null && collision.gameObject.layer != parent.layer && (blockBulletLayers & (1 << collision.gameObject.layer)) != 0 && !isPlatformMissile)
        {
            KillBullet();
        }
        else if (parent != null && collision.gameObject.layer != parent.layer && (otherBulletLayer & (1 << collision.gameObject.layer)) != 0 && !isPlatformMissile)
        {
            StartCoroutine(KillBulletWithDelay());
        }
    }
    public void KillBullet()
    {
        if (bulletSpawnerParent != null) 
        {
            bulletSpawnerParent.BulletKilled(index);
        }

        // if this bullet is a platform turret missile, reset any magnets attached to platforms
        if (isPlatformMissile)
        {
            foreach (Transform child in gameObject.GetComponentInChildren<MissilePlatformManager>().transform)
            {
                if (child.GetComponent<MagnetProjectileScript>() != null)
                {
                    child.GetComponent<MagnetProjectileScript>().DestroyThis();
                }
            }
        }

        gameObject.SetActive(false);
    }
    public IEnumerator KillBulletWithDelay()
    {
        yield return new WaitForSeconds(.012f);
        KillBullet();
    }
}
