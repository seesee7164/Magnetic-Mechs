using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingLaserScript : MonoBehaviour
{
    //script for managing the flying bosses ceiling laser attack
    [Header("Components")]
    public LayerMask blockLaserLayers;
    public GameObject laserSpawnMiddle;
    public GameObject laserSpawnBottom;
    public GameObject laserSpawnTop;
    public LineRenderer lineRendererMiddle;
    public LineRenderer lineRendererBottom;
    public LineRenderer lineRendererTop;
    public GameObject ObjectHit;
    public Transform playerTransform;
    public PlayerScript playerScript;
    public LineRenderer laserPointer;
    public BeeBossParentScript beeBossParentScript;
    [Header("variables")]
    private float laserPointerTime;
    private float defaultDamage = .25f;
    private float defaultInvincibleTime = .25f;
    private float timeBetweenShooting;
    private float timeBetweenShootingTimer = 0f;
    private float laserFullTime;
    private float laserCurrTime = 0f;
    public bool laserEnabled = false;
    private bool bossDead = false;
    [Header("Stages")]
    private float laserPointerTimeStage1 = 1.5f;
    private float laserPointerTimeStage2 = 1.25f;
    private float timeBetweenShootingStage1 = 3f;
    private float timeBetweenShootingStage2 = 2f;
    private float laserFullTimeStage1 = 4f;
    private float laserFullTimeStage2 = 5f;

    void Awake()
    {
        blockLaserLayers = LayerMask.GetMask("Ground", "Player", "Wall", "FireWall");
        lineRendererMiddle = laserSpawnMiddle.GetComponent<LineRenderer>();
        lineRendererBottom = laserSpawnBottom.GetComponent<LineRenderer>();
        lineRendererTop = laserSpawnTop.GetComponent<LineRenderer>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.GetComponent<Transform>();
        else Debug.Log("Player could not be found");
        GameObject beeBossParent = GameObject.FindGameObjectWithTag("BeeBossParent");
        if (beeBossParent != null) beeBossParentScript = beeBossParent.GetComponent<BeeBossParentScript>();
        else Debug.Log("Bee Boss Parent could not be found");
        TriggerStage1();
    }
    private void TriggerStage1()
    {
        laserPointerTime = laserPointerTimeStage1;
        timeBetweenShooting = timeBetweenShootingStage1;
        laserFullTime = laserFullTimeStage1;
    }
    public void TriggerStage2()
    {
        laserPointerTime = laserPointerTimeStage2;
        timeBetweenShooting = timeBetweenShootingStage2;
        laserFullTime = laserFullTimeStage2;
    }
    private void FixedUpdate()
    {
        if (bossDead) return;
        if (!laserEnabled)
        {
            if (timeBetweenShootingTimer >= timeBetweenShooting)
            {
                StartShooting();
            }
            else
            {
                timeBetweenShootingTimer += Time.fixedDeltaTime;
            }
            return;
        }
        else
        {
            if (laserCurrTime >= laserFullTime)
            {
                stopLaser();
                return;
            }
            else
            {
                laserCurrTime += Time.fixedDeltaTime;
            }
        }
        ShootLaser(laserSpawnMiddle.GetComponent<Transform>(), lineRendererMiddle);
        ShootLaser(laserSpawnBottom.GetComponent<Transform>(), lineRendererBottom);
        ShootLaser(laserSpawnTop.GetComponent<Transform>(), lineRendererTop);
    }
    //External Functions
    public void StartShooting()
    {
        if (beeBossParentScript == null) return;
        //robotSpiderQueenScript.startLaserAttack();
        laserPointer.enabled = true;
        laserPointer.startColor = Color.red;
        laserPointer.endColor = Color.red;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.right, 100, blockLaserLayers);
        if (hit.point == null)
        {
            return;
        }
        laserPointer.SetPosition(0, laserSpawnMiddle.GetComponent<Transform>().position);
        laserPointer.SetPosition(1, hit.point);
        laserCurrTime = 0f;
        StartCoroutine(laserPointerLife());
    }
    public IEnumerator laserPointerLife()
    {
        laserEnabled = false;
        float timePerFlash = .1f;
        float cycles = laserPointerTime / timePerFlash;
        for (int i = 0; i < cycles; i++)
        {
            if (i % 2 == 0)
            {
                laserPointer.startColor = Color.red;
                laserPointer.endColor = Color.red;
            }
            else
            {
                laserPointer.startColor = Color.white;
                laserPointer.endColor = Color.white;
            }
            yield return new WaitForSeconds(timePerFlash);
        }
        laserPointer.enabled = false;
        yield return new WaitForSeconds(.1f);
        enableLaserRenderer();
    }
    private void ShootLaser(Transform laserSpawnPoint, LineRenderer lineRenderer)
    {
        RaycastHit2D hit = Physics2D.Raycast(laserSpawnPoint.position, -transform.right, 100, blockLaserLayers);
        if (hit.point == null)
        {
            return;
        }
        Draw2DRay(laserSpawnPoint.position, hit.point, lineRenderer);
        if (hit.collider == null) return;
        ObjectHit = hit.collider.gameObject;
        if (ObjectHit.layer == 3)
        {
            DealDamage(ObjectHit);
        }
    }
    void Draw2DRay(Vector2 startPosition, Vector2 endPosition, LineRenderer lineRenderer)
    {
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }
    void DealDamage(GameObject ObjectHit)
    {
        playerScript = ObjectHit.GetComponent<PlayerScript>();
        if (playerScript != null)
        {
            playerScript.DamagePlayer(defaultDamage, new Vector2(0, 0), 0, defaultInvincibleTime);
        }
    }
    void stopLaser()
    {
        timeBetweenShootingTimer = 0;
        //robotSpiderQueenScript.endBigAttack();
        //robotSpiderQueenScript.endLaserAttack();
        disableLaserRenderer();
    }
    void enableLaserRenderer()
    {
        if(bossDead) return;
        lineRendererMiddle.enabled = true;
        lineRendererBottom.enabled = true;
        lineRendererTop.enabled = true;
        laserEnabled = true;
    }
    void disableLaserRenderer()
    {
        lineRendererMiddle.enabled = false;
        lineRendererBottom.enabled = false;
        lineRendererTop.enabled = false;
        laserEnabled = false;
    }
    public void BossDied()
    {
        stopLaser();
        laserPointer.enabled = false;
        bossDead = true;
    }
}
