using System;
using UnityEngine;

public class Turret : BulletSpawnerParent
{
    // Store this script in the turret chamber
    // Inherits from the bullet spawner parent script, similar to BulletSpawnerScript

    [Header("Enemy Specifications")]

    protected float shootingAngle = 0f;

    [SerializeField] private float timeBetweenShots;
    private float timer;
    [SerializeField] private float missileForce;

    private void Start()
    {
        SetUpTurret();
    }

    public virtual void SetUpTurret()
    {
        // Set the base and shooting angles based on specified values
        transform.eulerAngles = new Vector3(0f, 0f, shootingAngle);

        // increase bullet force to account for large missile mass for letting players walk on bullet
        bulletForce = missileForce * 10000;
        timer = timeBetweenShots;
        audioBox = gameObject.GetComponent<AudioSource>();
        parentObject = gameObject;
        SetUpGameObjects();
    }

    private void FixedUpdate()
    {
        if (timer <= 0f && bulletsQueue.Count != 0)
        {
            Fire();
        }

        timer -= Time.fixedDeltaTime;
    }

    protected virtual void Fire()
    {
        SpawnBullet();
        SpawnMuzzleEffect();
        audioBox.Play();
        timer = timeBetweenShots;
    }
}
