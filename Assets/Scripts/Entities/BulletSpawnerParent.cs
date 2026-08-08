using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BulletSpawnerParent : MonoBehaviour
{
    //the parent class for scripts which spawn bullets
    [Header("Objects")]
    protected int maxBullets = 6;
    protected GameObject muzzleEffect;
    protected GameObject[] bulletsArray;
    protected Queue<int> bulletsQueue;
    [Header("Components")]
    public GameObject bulletPrefab;
    public GameObject MuzzlePrefab;
    public GameObject bulletSpawnpoint;
    public GameObject muzzleSpawnpoint;
    protected GameObject parentObject;
    protected AudioSource audioBox;
    [Header("Variables")]
    protected float bulletForce;
    protected void SetUpGameObjects()
    {
        //sets up all of the objects the bullet spawner uses
        if (MuzzlePrefab != null)
        {
            muzzleEffect = Instantiate(MuzzlePrefab, muzzleSpawnpoint.transform.position, transform.rotation);
            muzzleEffect.SetActive(false);
        }
        if (bulletPrefab != null)
        {
            bulletsArray = new GameObject[maxBullets];
            for (int i = 0; i < maxBullets; i++)
            {
                GameObject tempBullet = Instantiate(bulletPrefab, bulletSpawnpoint.transform.position, transform.rotation);
                tempBullet.GetComponent<BulletScript>().parent = parentObject;
                bulletsArray[i] = tempBullet;
                tempBullet.GetComponent<BulletScript>().index = i;
                tempBullet.SetActive(false);
            }
            bulletsQueue = new Queue<int>();
            for (int i = 0; i < maxBullets; i++)
            {
                bulletsQueue.Enqueue(i);
            }
        }
    }
    public void SpawnBullet()
    {
        int currentIndex = bulletsQueue.Dequeue();
        GameObject tempBullet = bulletsArray[currentIndex];
        tempBullet.SetActive(true);
        tempBullet.transform.position = bulletSpawnpoint.transform.position;
        tempBullet.transform.rotation = transform.rotation;
        BulletScript tempBulletScript = tempBullet.GetComponent<BulletScript>();
        tempBulletScript.bulletSpawnerParent = this;
        tempBulletScript.parent = parentObject;
        tempBulletScript.SetDeathTime();
        Rigidbody2D bulletRB = tempBullet.GetComponent<Rigidbody2D>();
        bulletRB.AddForce(transform.right * bulletForce, ForceMode2D.Impulse);
    }
    public void SpawnMuzzleEffect()
    {
        muzzleEffect.SetActive(true);
        muzzleEffect.transform.position = muzzleSpawnpoint.transform.position;
        muzzleEffect.transform.rotation = transform.rotation;
        muzzleEffect.GetComponent<Rigidbody2D>().linearVelocity = parentObject.GetComponent<Rigidbody2D>().linearVelocity;
        StartCoroutine(DestroyEffect());
    }
    public IEnumerator DestroyEffect()
    {
        yield return new WaitForSeconds(muzzleEffect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        muzzleEffect.SetActive(false);
    }
    public void BulletKilled(int index)
    {
        bulletsQueue.Enqueue(index);
    }
}
