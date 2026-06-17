using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentScript : MonoBehaviour
{
    //old script for an agent which was used as part of a cutscene. likely to be deleted
    public float fallingSpeed = 4f;
    [Header("Components")]
    public Rigidbody2D myRigidBody2D;
    public GameObject allEvents;
    // Start is called before the first frame update
    private void Awake()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        allEvents = GameObject.FindGameObjectWithTag("AllEvents");
    }

    // Update is called once per frame
    void Update()
    {
        myRigidBody2D.linearVelocity = new Vector3(0, -fallingSpeed, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3 && allEvents!= null)
        {
            //allEvents.GetComponent<TutorialAllEvents>().AfterAgentDestroyed();
            Destroy(gameObject);
        }
    }
}
