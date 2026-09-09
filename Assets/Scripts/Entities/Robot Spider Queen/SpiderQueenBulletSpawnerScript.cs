using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderQueenBulletSpawnerScript : BulletSpawnerParent
{
    //handles spawning bullets for the robot spider queen
    [Header("Components")]
    //public GameObject bulletPrefab;
    //public GameObject MuzzlePrefab;
    //public AudioSource audioBox;
    //public GameObject bulletSpawnpoint;
    public GameObject RobotSpiderQueen;
    [Header("Player")]
    public GameObject player;
    public Vector2 playerRelativePosition;
    [Header("variables")]
    private float reloadTime;
    private float shootingVariabilityAngle;
    private float timer;
    private bool shootingDisabled = false;
    [Header("Stages")]
    public float bulletForceStage1 = 20f;
    public float reloadTimeStage1 = 1f;
    public float shootingVariabilityAngleStage1 = 5;
    public float bulletForceStage2 = 25f;
    public float reloadTimeStage2 = .7f;
    public float shootingVariabilityAngleStage2 = 20;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        RobotSpiderQueen = GameObject.FindGameObjectWithTag("RobotSpiderQueen");
        parentObject = RobotSpiderQueen;
        shootingDisabled = true;
        audioBox = GetComponent<AudioSource>();
        SetUpGameObjects();
        muzzleEffect.GetComponent<MuzzleScript>().ChangeToRed();
        TriggerStage1();
    }
    // Start is called before the first frame update
    void Start()
    {
        //readyForNextShot = 0;
        timer = 0;
    }
    private void TriggerStage1()
    {
        bulletForce = bulletForceStage1;
        reloadTime = reloadTimeStage1;
        shootingVariabilityAngle = shootingVariabilityAngleStage1;
    }
    public void TriggerStage2()
    {
        bulletForce = bulletForceStage2;
        reloadTime = reloadTimeStage2;
        shootingVariabilityAngle = shootingVariabilityAngleStage2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer > reloadTime && !shootingDisabled && player != null && bulletsQueue.Count > 0)
        {
            handleOrientation();
            Shoot();
            //readyForNextShot = Time.time + reloadTime;
            timer = 0;
        }
        timer = timer + Time.fixedDeltaTime;
    }
    private void handleOrientation()
    {
        playerRelativePosition = (Vector2)(player.transform.position - transform.position);
        gameObject.transform.right = playerRelativePosition;
        float randAngle = (float)(Random.value - .5) * 2 * shootingVariabilityAngle;
        transform.rotation *= Quaternion.Euler(0, 0, randAngle);
    }
    void Shoot()
    {
        //GameObject bullet = Instantiate(bulletPrefab, bulletSpawnpoint.transform.position, transform.rotation);
        //bullet.GetComponent<BulletScript>().parent = RobotSpiderQueen;
        //Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
        //bulletRB.AddForce(transform.right * bulletForce, ForceMode2D.Impulse);
        //GameObject effect = Instantiate(MuzzlePrefab, bulletSpawnpoint.transform.position, transform.rotation);
        //audioBox.Play();
        //Destroy(effect, effect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        SpawnBullet();
        SpawnMuzzleEffect();
        audioBox.Play();
        timer = 0;
    }
    public void DisableShooting()
    {
        shootingDisabled = true;
    }
    public void EnableShooting()
    {
        shootingDisabled = false;
        timer = 0;
        //readyForNextShot = Time.time + reloadTime;
    }
}
