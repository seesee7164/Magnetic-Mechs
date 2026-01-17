using Cinemachine;
using UnityEngine;

public class BeeBossParentScript : MonoBehaviour
{
    [Header("components")]
    public BeeBossScript beeBossScript;
    private Rigidbody2D myRigidbody2D;
    public CinemachineVirtualCamera myVirtualCamera;
    public CeilingLaserScript ceilingLaserScript;
    public RockSpawnerScript rockSpawnerScript;
    public DroneRespawnerScript droneRespawnerScript;
    [Header("variables")]
    public bool beeBossActive;
    private float speed;
    [Header("Stages")]
    private float Stage1Speed = 4f;
    private float Stage2Speed = 5f;
    
    void Awake()
    {
        beeBossActive = false;
        speed = Stage1Speed;
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        if (beeBossActive)
        {
            Vector2 moveHorizontal = Vector2.right * speed;
            this.transform.Translate(moveHorizontal * Time.fixedDeltaTime);
            //myRigidbody2D.MovePosition(myRigidbody2D.position + moveHorizontal * Time.fixedDeltaTime);
        }
    }
    public void Activate()
    {
        beeBossActive = true;
        if(beeBossScript != null) beeBossScript.activateBoss();
        myVirtualCamera.Follow = gameObject.transform;
    }
    public void ActivateStage2()
    {
        speed = Stage2Speed;
        ceilingLaserScript.TriggerStage2();
        rockSpawnerScript.TriggerStage2();
    }
    public void BossDied()
    {
        speed = 0;
        ceilingLaserScript.BossDied();
        rockSpawnerScript.BossDied();
        droneRespawnerScript.BossDied();
    }
}
