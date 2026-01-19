using UnityEngine;

public class BouncyDeathPitScript : MonoBehaviour
{
    [Header("Components")]
    public PlayerScript playerScript;
    [Header("variables")]
    private float defaultDamage = 2f;
    private float defaultInvincibleTime = .5f;
    private float knockbackStrength = 1.5f;
    public bool lessBouncy = false;
    private void Awake()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        if (lessBouncy) knockbackStrength = .9f;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            DealDamage(collision.gameObject);
        }
    }
    void DealDamage(GameObject ObjectHit)
    {
        if (playerScript != null)
        {
            playerScript.DamagePlayer(defaultDamage, new Vector2(0, 1), knockbackStrength, defaultInvincibleTime);
        }
    }
}
