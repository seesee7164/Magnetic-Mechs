using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class BulletSpawnerScript : BulletSpawnerParent
{
    //main script for spawning bullets for the player
    [Header("Variables")]
    private float reloadTime = .55f;
    private float timer;
    private bool shootingDisabled = false;
    public bool red;
    [Header("Components")]
    //public GameObject bulletPrefab;
    //public GameObject MuzzlePrefab;
    //public GameObject bulletSpawnpoint;
    //public GameObject muzzleSpawnpoint;
    //private AudioSource audioBox;
    public GameObject player;
    //public Animator animator;
    void Start()
    {
        bulletForce = 28f;
        timer = reloadTime;
        audioBox = gameObject.GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        parentObject = player;
        SetUpGameObjects();
        if(red) ChangeToRed();
    }
    private void FixedUpdate()
    {
        timer = timer + Time.fixedDeltaTime;
    }
    public void Shoot()
    {
        if ( player == null)
        {
            Debug.Log("The player could not be found");
            return;
        }
        if (timer < reloadTime || shootingDisabled || bulletsQueue.Count ==0) return;
        SpawnBullet();
        SpawnMuzzleEffect();
        audioBox.Play();
        timer = 0;
        //animator.SetBool("Shoot", true);
    }
    //Events
    public void DisableShooting()
    {
        shootingDisabled = true;
    }
    public void EnableShooting()
    {
        shootingDisabled = false;
    }
    public void ChangeToRed()
    {
        red = true;
        if (muzzleEffect == null || bulletsArray[0] == null) return;
        muzzleEffect.GetComponent<MuzzleScript>().ChangeToRed();
        for (int i = 0; i < maxBullets; i++)
        {
            bulletsArray[i].GetComponent<BulletScript>().ChangeToRed();
        }
    }
    public void ChangeToBlue()
    {
        red = false;
        if (muzzleEffect == null || bulletsArray[0] == null) return;
        muzzleEffect.GetComponent<MuzzleScript>().ChangeToBlue();
        for (int i = 0; i < maxBullets; i++)
        {
            bulletsArray[i].GetComponent<BulletScript>().ChangeToBlue();
        }
    }
}
