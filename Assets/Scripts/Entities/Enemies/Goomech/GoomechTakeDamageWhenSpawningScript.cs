using UnityEngine;

public class GoomechTakeDamageWhenSpawningScript : MonoBehaviour
{
    public GoomechScript myGoomechScript;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            myGoomechScript.TakeDamage(1, collision.transform.up.normalized, .25f);
        }

        if (collision.gameObject.layer == 3 && myGoomechScript.playerTransform != null && myGoomechScript.playerTransform.GetComponent<PlayerScript>().isCharging)
        {
            myGoomechScript.TakeDamage(1, collision.transform.up.normalized, .25f);
        }
    }
}
