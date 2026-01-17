using UnityEngine;

public class FirewallScript : MonoBehaviour
{
    [Header("Components")]
    public PlayerScript playerScript;
    [Header("variables")]
    private float defaultDamage = .5f;
    private float defaultInvincibleTime = .25f;
    [Header("Position")]
    private Vector2 StartPosition;
    private Vector2 EndPosition;
    private float timeRate = 0.5f;
    private float timer = 0f;
    private bool finishedMoving = false;
    private void Awake()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        EndPosition = (Vector2) transform.localPosition + Vector2.right * 3f;
        StartPosition = (Vector2) transform.localPosition;
        transform.localPosition = StartPosition;
    }

    private void FixedUpdate()
    {
        if(finishedMoving) return;
        if (timer >= .99f)
        {
            Debug.Log("done");
            transform.localPosition = EndPosition;
            finishedMoving = true;
        }
        transform.localPosition = Vector2.Lerp(StartPosition, EndPosition, timer);
        Debug.Log(timer);
        timer += Time.deltaTime * timeRate;
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
            playerScript.DamagePlayer(defaultDamage, new Vector2(1, 1), 1, defaultInvincibleTime);
        }
    }
}
