using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    //script for managing the robot spider queen's sweeping laser attack
    [Header("Components")]
    public LayerMask blockLaserLayers;
    public GameObject laserSpawnMiddle;
    public GameObject laserSpawnLeft;
    public GameObject laserSpawnRight;
    public LineRenderer lineRendererMiddle;
    public LineRenderer lineRendererLeft;
    public LineRenderer lineRendererRight;
    public GameObject ObjectHit;
    public Transform playerTransform;
    public PlayerScript playerScript;
    public GameObject explosionEffect;
    public LineRenderer laserPointer;
    public RobotSpiderQueenScript robotSpiderQueenScript;
    [Header("variables")]
    public float laserAngle;
    public float startingAngle;
    private float rotationSpeed;
    private float laserPointerTime;
    private float goThroughDegrees = 360f;
    private float defaultDamage = .25f;
    private float defaultInvincibleTime = .1f;
    public bool laserEnabled = false;
    private bool triggerStage2 = false;
    private bool bossDied = false;
    [Header("Explosion")]
    public float explosionTime = .2f;
    private float explosionTimer;
    public float explosionOffset = .05f;
    [Header("Stages")]
    public float rotationSpeedStage1 = 23f;
    public float laserPointerTimeStage1 = 1.5f;
    public float goThroughDegreesStage1 = 360f;
    public float rotationSpeedStage2 = 28f;
    public float laserPointerTimeStage2 = 1f;
    public float goThroughDegreesStage2 = 540f;
    [Header("Explosion Arrays")]
    private GameObject[] ExplosionArray;
    private Queue<int> ExplosionsAvailableQueue;
    private int maxExplosions = 10;
    void Awake()
    {
        laserAngle = 0;
        explosionTimer = 0;
        //rotationSpeed = 30f;
        blockLaserLayers = LayerMask.GetMask("Ground", "Player", "Wall");
        lineRendererMiddle = laserSpawnMiddle.GetComponent<LineRenderer>();
        lineRendererLeft = laserSpawnLeft.GetComponent<LineRenderer>();
        lineRendererRight = laserSpawnRight.GetComponent<LineRenderer>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.GetComponent<Transform>();
        else Debug.Log("Player could not be found");
        GameObject robotSpiderQueen = GameObject.FindGameObjectWithTag("RobotSpiderQueen");
        if (robotSpiderQueen != null) robotSpiderQueenScript = robotSpiderQueen.GetComponent<RobotSpiderQueenScript>();
        else Debug.Log("Robot Spider Queen could not be found");
        SetUpExplosionArrays();
        TriggerStage1();
    }
    private void SetUpExplosionArrays()
    {
        ExplosionArray = new GameObject[maxExplosions];
        for (int i = 0; i < maxExplosions; i++)
        {
            GameObject tempExplosion = Instantiate(explosionEffect, transform.position, Quaternion.Euler(0, 0, 0));
            ExplosionArray[i] = tempExplosion;
            tempExplosion.SetActive(false);
            ExplosionScript tempExplosionScript = tempExplosion.GetComponent<ExplosionScript>();
            tempExplosionScript.laserScript = this;
            tempExplosionScript.index = i;
        }
        ExplosionsAvailableQueue = new Queue<int>();
        for (int i = 0; i < maxExplosions; i++)
        {
            ExplosionsAvailableQueue.Enqueue(i);
        }
    }
    private void TriggerStage1()
    {
        rotationSpeed = rotationSpeedStage1;
        laserPointerTime = laserPointerTimeStage1;
        goThroughDegrees = goThroughDegreesStage1;
    }
    public void TriggerStage2()
    {
        if (laserEnabled)
        {
            triggerStage2 = true;
        }
        else
        {
            ActuallyTriggerStage2();
        }
    }
    public void ActuallyTriggerStage2()
    {
        rotationSpeed = rotationSpeedStage2;
        laserPointerTime = laserPointerTimeStage2;
        goThroughDegrees = goThroughDegreesStage2;
    }
    private void FixedUpdate()
    {
        if (!laserEnabled)
        {
            return;
        }
        laserAngle -= rotationSpeed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(Vector3.forward * laserAngle);
        explosionTimer += Time.fixedDeltaTime;
        ShootLaser(laserSpawnMiddle.GetComponent<Transform>(),lineRendererMiddle);
        ShootLaser(laserSpawnLeft.GetComponent<Transform>(),lineRendererLeft);
        ShootLaser(laserSpawnRight.GetComponent<Transform>(), lineRendererRight);
        if (Mathf.Abs(laserAngle - startingAngle) > goThroughDegrees)
        {
            stopLaser();
        }
    }
    //External Functions
    public void setInitialAngle()
    {
        if (robotSpiderQueenScript == null) return;
        robotSpiderQueenScript.startLaserAttack();
        if (playerTransform != null)
        {
            rotationSpeed = Mathf.Abs(rotationSpeed);
            Vector3 relativePositionToTarget = playerTransform.position - gameObject.transform.position;
            float playerPositionAngle = Mathf.Atan2(relativePositionToTarget.y, relativePositionToTarget.x) * Mathf.Rad2Deg;
            int rounded = (int)Mathf.Round(playerPositionAngle / 90);
            laserAngle = (float)(rounded * 90) % 360;
            startingAngle = laserAngle;
            transform.rotation = Quaternion.Euler(Vector3.forward * laserAngle);
            //if (playerPositionAngle > laserAngle)
            //{
            //    rotationSpeed *= -1;
            //}
        }
        laserPointer.enabled = true;
        laserPointer.startColor = Color.red;
        laserPointer.endColor = Color.red;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 100, blockLaserLayers);
        if (hit.point == null)
        {
            return;
        }
        laserPointer.SetPosition(0, laserSpawnMiddle.GetComponent<Transform>().position);
        laserPointer.SetPosition(1, hit.point);
        StartCoroutine(laserPointerLife());
    }
    public IEnumerator laserPointerLife()
    {
        laserEnabled = false;
        float timePerFlash = .1f;
        float cycles = laserPointerTime /timePerFlash;
        for(int i = 0; i < cycles; i++) 
        { 
            if(i%2 == 0)
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
        Vector3 relativePositionToTarget = playerTransform.position - gameObject.transform.position;
        float playerPositionAngle = Mathf.Atan2(relativePositionToTarget.y, relativePositionToTarget.x) * Mathf.Rad2Deg;
        if (playerPositionAngle > laserAngle)
        {
            rotationSpeed *= -1;
        }
        enableLaserRenderer();
    }
    private void ShootLaser(Transform laserSpawnPoint, LineRenderer lineRenderer)
    {
        RaycastHit2D hit = Physics2D.Raycast(laserSpawnPoint.position,transform.right,100,blockLaserLayers);
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
        if (explosionTimer > explosionTime && ExplosionsAvailableQueue.Count >0)
        {
            CreateExplosion(startPosition, endPosition);
            explosionTimer = 0;
        }
    }
    void DealDamage(GameObject ObjectHit)
    {
        playerScript = ObjectHit.GetComponent<PlayerScript>();
        if (playerScript != null)
        {
            playerScript.DamagePlayer(defaultDamage, new Vector2(0, 0), 0, defaultInvincibleTime);
        }
    }
    void CreateExplosion(Vector3 startPoint, Vector3 endPoint)
    {
        endPoint.z -= .1f;
        endPoint.x -= .01f * (endPoint.x - startPoint.x);
        endPoint.y -= .02f * (endPoint.y - startPoint.y) - explosionOffset;

        int currentIndex = ExplosionsAvailableQueue.Dequeue();
        GameObject tempExplosion = ExplosionArray[currentIndex];
        tempExplosion.SetActive(true);
        tempExplosion.transform.position = endPoint;
        tempExplosion.GetComponent<ExplosionScript>().restartExplosion();
        //GameObject effect = Instantiate(explosionEffect, endPoint, Quaternion.identity);
        //Destroy(effect, effect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
    }
    public void ExplosionKilled(int index)
    {
        ExplosionsAvailableQueue.Enqueue(index);
    }
    void stopLaser()
    {
        laserEnabled = false;
        //robotSpiderQueenScript.endBigAttack();
        robotSpiderQueenScript.endLaserAttack();
        disableLaserRenderer();
        if (triggerStage2)
        {
            ActuallyTriggerStage2();
        }
    }
    void enableLaserRenderer()
    {
        if (bossDied) return;
        lineRendererMiddle.enabled = true;
        lineRendererLeft.enabled = true;
        lineRendererRight.enabled = true;
        laserEnabled = true;
    }
    void disableLaserRenderer()
    {
        lineRendererMiddle.enabled = false;
        lineRendererLeft.enabled = false;
        lineRendererRight.enabled = false;
        laserEnabled = false;
    }
    public void BossDied()
    {
        disableLaserRenderer ();
        bossDied = true;
    }
}
