using UnityEngine;

public class BulletRearScript : MonoBehaviour
{
    [Header("Components")]
    public BulletScript bulletScript;
    private LayerMask blockBulletLayers;
    private void Awake()
    {
        blockBulletLayers = LayerMask.GetMask("Ground", "Wall", "Spike", "FireWall", "Platform Turret");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //GameObject effect = Instantiate(explosionEffect, transform.position + explosionOffset, Quaternion.identity);
        //Destroy(effect, effect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        if ((blockBulletLayers & (1 << collision.gameObject.layer))!=0)
        {
            bulletScript.KillBullet();
        }
    }
}
