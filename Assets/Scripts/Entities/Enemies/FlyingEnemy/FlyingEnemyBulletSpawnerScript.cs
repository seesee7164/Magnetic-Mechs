using UnityEngine;

public class FlyingEnemyBulletSpawnerScript : BulletSpawnerParent
{
    //handles spawning bullets for the robot spider queen
    [Header("variables")]
    public float shootingTime = 1;
    public float shootingCounter;
    [Header("Player")]
    public GameObject player;
    [Header("Components")]
    private bool isAlive = true;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        parentObject = gameObject;
        audioBox = GetComponent<AudioSource>();
        bulletForce = 20f;
        shootingCounter = shootingTime;
        SetUpGameObjects();
        muzzleEffect.GetComponent<MuzzleScript>().ChangeToRed();
    }

    // Update is called once per frame
    void Update()
    {
        if (shootingCounter <= 0 && player != null && bulletsQueue.Count > 0 && isAlive)
        {
            Shoot();
        }
        shootingCounter -= Time.deltaTime;
    }
    void Shoot()
    {
        shootingCounter = shootingTime;
        SpawnBullet();
        SpawnMuzzleEffect();
        audioBox.Play();
    }
    public void EnemyUnkilled()
    {
        shootingCounter = shootingTime;
        isAlive = true;
    }
    public void EnemyKilled()
    {
        isAlive = false;
    }
}
