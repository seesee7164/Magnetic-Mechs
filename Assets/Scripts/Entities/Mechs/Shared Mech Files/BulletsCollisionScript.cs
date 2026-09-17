using UnityEngine;

public class BulletsCollisionScript : MonoBehaviour
{
    [Header("Components")]
    public BulletScript bulletScript;
    private LayerMask blockBulletLayers;
    [Header("MechBoss")]
    public bool isPlayerBullet = false;
    private void Awake()
    {
        if (isPlayerBullet)
        {
            blockBulletLayers |= LayerMask.GetMask("Enemy Bullet Collider");
        }
        else blockBulletLayers |= LayerMask.GetMask("Player Bullet Collider");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //GameObject effect = Instantiate(explosionEffect, transform.position + explosionOffset, Quaternion.identity);
        //Destroy(effect, effect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        if ((blockBulletLayers & (1 << collision.gameObject.layer)) != 0)
        {
            bulletScript.PublicKillBulletWithDelay(0.02f);
        }
    }
}
