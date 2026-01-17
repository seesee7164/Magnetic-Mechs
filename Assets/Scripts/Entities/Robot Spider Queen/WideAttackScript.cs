using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
//using static UnityEngine.RuleTile.TilingRuleOutput;

public class WideAttackScript : BulletSpawnerParent
{
    //script for managing the spider queens wide cone attack
    [Header("Components")]
    public LineRenderer myLineRenderer;
    public LayerMask blockLaserLayers;
    public GameObject ObjectHit;
    public GameObject player;
    public GameObject RobotSpiderQueen;
    [Header("Orientation")]
    public Vector2 playerRelativePosition;
    public Vector2 lockedAngle;
    [Header("variables")]
    private bool laserOn = false;
    public float angleBetweenShots = 10;
    public int shotsOnEachSide;
    private bool currentlyShooting = false;
    private bool bossIsDead = false;
    [Header("Timing")]
    public float laserTime;
    public float shootDelay = .5f;
    [Header("Stages")]
    public float laserTimeStage1 = 3f;
    public int shotsOnEachSideStage1;
    public float laserTimeStage2 = 2.5f;
    public int shotsOnEachSideStage2;
    private bool triggerStage2 = false;
    void Awake()
    {
        blockLaserLayers = LayerMask.GetMask("Ground", "Player");
        myLineRenderer = GetComponent<LineRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        RobotSpiderQueen = GameObject.FindGameObjectWithTag("RobotSpiderQueen");
        bulletForce = 35;
        parentObject = RobotSpiderQueen;
        maxBullets = shotsOnEachSideStage2 * 2 + 1;
        SetUpGameObjects();
        TriggerStage1();
    }
    private void TriggerStage1()
    {
        laserTime = laserTimeStage1;
        shotsOnEachSide = shotsOnEachSideStage1;
    }
    public void TriggerStage2()
    {
        if (currentlyShooting)
        {
            triggerStage2 = true;
        }
        else ActuallyTriggerStage2();
    }
    private void ActuallyTriggerStage2()
    {
        laserTime = laserTimeStage2;
        shotsOnEachSide = shotsOnEachSideStage2;
    }
    private void FixedUpdate()
    {
        if (laserOn)
        {
            LaserPhase();
        }
    }
    public void LaserPhase()
    {
        HandleOrientation();
        ShootLaser();
    }
    private void ShootLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 100, blockLaserLayers);
        if (hit.point == null)
        {
            return;
        }
        //Vector2 Extension = new Vector2(.2f * hit.point.x - transform.position.x, .2f * hit.point.y - transform.position.y);
        Draw2DRay(transform.position, hit.point);
        ObjectHit = hit.collider.gameObject;

    }
    void Draw2DRay(Vector2 startPosition, Vector2 endPosition)
    {
        myLineRenderer.SetPosition(0, startPosition);
        myLineRenderer.SetPosition(1, endPosition);
    }
    private void HandleOrientation()
    {
        if (player == null) return;
        playerRelativePosition = (Vector2)(player.transform.position - transform.position);
        gameObject.transform.right = playerRelativePosition;
    }
    
    //External functions
    public void startLaser()
    {
        currentlyShooting = true;
        RobotSpiderQueen?.GetComponent<RobotSpiderQueenScript>().startBigAttack();
        laserOn = true;
        myLineRenderer.enabled = true;
        StartCoroutine(freezeLaser());
    }
    public IEnumerator freezeLaser()
    {
        //Handles making the targetting laser flash white and red
        float numberOfFlashes = laserTime * 5;
        for(int i = 0; i < numberOfFlashes; i++)
        {
            if(i %2 == 0)
            {
                myLineRenderer.startColor = Color.red;
                myLineRenderer.endColor = Color.red;
            }
            else
            {
                myLineRenderer.startColor = Color.white;
                myLineRenderer.endColor = Color.white;
            }
            yield return new WaitForSeconds(laserTime / numberOfFlashes);
        }
        lockedAngle = playerRelativePosition;
        laserOn=false;
        myLineRenderer.enabled=false;
        StartCoroutine(startShooting());
    }
    public  IEnumerator startShooting()
    {
        //starts the countdown between locking on and shooting
        yield return new WaitForSeconds(shootDelay);
        ShootWide();
    }
    void ShootWide()
    {
        //Vector3 rot = transform.rotation.eulerAngles.;
        //List<Vector3> shootAngles = new List<Vector3>();
        if(bossIsDead) return;
        transform.rotation *= Quaternion.Euler(0, 0, -angleBetweenShots * shotsOnEachSide);
        for (int i = 0; i < (shotsOnEachSide*2+1); i++)
        {
            SpawnBullet();
            transform.rotation *= Quaternion.Euler(0, 0, angleBetweenShots);
            //shootAngles.Add(transform.rotation.eulerAngles);
        }
        RobotSpiderQueen?.GetComponent<RobotSpiderQueenScript>().endBigAttack();
        currentlyShooting = false;
        if (triggerStage2) ActuallyTriggerStage2();
    }
    public void BossDied()
    {
        bossIsDead = true;
    }
}
