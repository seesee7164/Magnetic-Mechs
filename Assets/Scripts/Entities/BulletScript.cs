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
    public int index;
    public BulletSpawnerParent bulletSpawnerParent;
    public float lifetime = 3;
    private LayerMask blockBulletLayers;
    //private GameObject explosionEffect;
    //private Vector3 explosionOffset = new Vector3(0, .05f, 0);

    public bool isPlatformMissile;

    void Start()
    {
        isPlatformMissile = gameObject.GetComponent<MissilePlatformManager>() != null;
    }
    private void Awake()
    {
        blockBulletLayers = LayerMask.GetMask("Player", "Enemy", "Rock");
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
        if (parent!= null && collision.gameObject != parent && (blockBulletLayers & (1 << collision.gameObject.layer)) != 0)
        {
            if (!isPlatformMissile)
            {
                KillBullet();
            }
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
}
