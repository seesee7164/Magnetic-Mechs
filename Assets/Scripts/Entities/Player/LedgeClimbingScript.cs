using Unity.VisualScripting;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Components")]
    public GameObject player;
    private PlayerScript playerScript;
    private Rigidbody2D playerRigidBody;
    //private BoxCollider2D myCollider;
    [Header("Varaibles")]
    private float upwardsForce = 20f;
    private float horizontalForce = 20f;
    private void Awake()
    {
        playerScript = player.GetComponent<PlayerScript>();
        playerRigidBody = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("test1");
        if(collision.gameObject.layer == 6)
        {
            Debug.Log("test2");
            playerRigidBody.AddForce(new Vector2(horizontalForce,upwardsForce));
        }
    }
}
