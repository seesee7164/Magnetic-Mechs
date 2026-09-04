using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class MechBossControllerScript : MonoBehaviour
{
    [Header("Components")]
    public MechBossActionsScript availableActionsScript;
    public Transform centerCeilingTransform;
    [Header("variables")]
    private bool bossActive = false;
    private bool trackPlayer = true;

    [Header("Behaviors")]
    public CurrentObjective currentObjective;

    [Header("BehaviorVariables")]
    private bool startedLeft = false;
    private bool performMidBehaviorAction = false;
    private float targetX;
    [Header("Timers")]
    private float startBehaviorTimer = 0f;
    private float currentCooldown = 2f;
    [Header("Probabilities")]
    private float moveAcrossScreenWithAttractionProbability = .3f;
    private float flyToCeilingProbability = .3f;
    private float launchAcrossScreenProbability = .3f;
    //private float moveAcrossScreenProbability = .1f;
    public enum CurrentObjective
    {
        Stationary,
        MoveAcrossScreen,
        AimAtGround,
        FlyToCeiling,
        AimAcrossArena,
        MoveAcrossScreenWithAttraction,
        AimBehindMech,
        launchAcrossScreen
    }

    private void Awake()
    {
        flyToCeilingProbability = flyToCeilingProbability + moveAcrossScreenWithAttractionProbability;
        launchAcrossScreenProbability = launchAcrossScreenProbability + flyToCeilingProbability;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        if (!bossActive) return;
        PerformObjectiveBehavior();
        HandleBehaviorTimer();
        if (trackPlayer) availableActionsScript.trackPlayerWithGun();

    }
    //Fixed Update Functions
    private void HandleBehaviorTimer()
    {
        if (currentObjective == CurrentObjective.Stationary)
        {
            if (startBehaviorTimer >= currentCooldown)
            {
                float newBehavior = Random.value;
                Debug.Log(newBehavior);
                if(newBehavior <= moveAcrossScreenWithAttractionProbability) StartAimAcrossArena();
                else if(newBehavior <= flyToCeilingProbability) StartAimAtGround();
                else if (newBehavior <= launchAcrossScreenProbability) StartAimBehindMech();
                else StartMoveAcrossScreen();
                startBehaviorTimer = 0f;
            }
            else
            {
                startBehaviorTimer += Time.fixedDeltaTime;
            }
        }
    }
    private void PerformObjectiveBehavior()
    {
        if (currentObjective == CurrentObjective.MoveAcrossScreen) MoveAcrossScreen();
        else if (currentObjective == CurrentObjective.AimAtGround) AimAtGround();
        else if (currentObjective == CurrentObjective.FlyToCeiling) FlyToCeiling();
        else if (currentObjective == CurrentObjective.AimAcrossArena) AimAcrossArena();
        else if (currentObjective == CurrentObjective.MoveAcrossScreenWithAttraction) MoveAcrossScreenWithAttraction();
        else if (currentObjective == CurrentObjective.AimBehindMech) AimBehindMech();
        else if (currentObjective == CurrentObjective.launchAcrossScreen) LaunchAcrossScreen();
    }

    //actions
    public void ActivateBoss()
    {
        bossActive = true;
        StartTrackingAndShootingPlayer();
    }
    private void StartTrackingAndShootingPlayer()
    {
        trackPlayer = true;
        availableActionsScript.StartShooting();
    }
    private void SetTargetAcrossArena()
    {
        startedLeft = gameObject.transform.position.x <= centerCeilingTransform.position.x;
        if (startedLeft)
        {
            targetX = UnityEngine.Random.Range(31f, 41f);
        }
        else
        {
            targetX = UnityEngine.Random.Range(8f, 18f);
        }
    }
    private void FinishBehavior(float cooldown)
    {
        currentObjective = CurrentObjective.Stationary;
        currentCooldown = cooldown;
    }
    //moving across Screen
    private void StartMoveAcrossScreen()
    {
        currentObjective = CurrentObjective.MoveAcrossScreen;
        SetTargetAcrossArena();
    }
    private void MoveAcrossScreen()
    {
        if (startedLeft)
        {
            availableActionsScript.Move(1f, 0f);
            if(gameObject.transform.position.x >= targetX)
            {
                availableActionsScript.Move(0f, 0f);
                FinishBehavior(2);
            }
        }
        else
        {
            availableActionsScript.Move(-1f, 0f);
            if (gameObject.transform.position.x <= targetX)
            {
                availableActionsScript.Move(0f, 0f);
                FinishBehavior(2);
            }
        }
    }
    //flying to ceiling
    private void StartAimAtGround()
    {
        trackPlayer = false;
        availableActionsScript.Move(0, 0);
        Vector2 MagnetAngle = (new Vector2(0, -1f) + ((Vector2)(gameObject.transform.position - centerCeilingTransform.position)).normalized).normalized;
        availableActionsScript.SetAimTarget(MagnetAngle);
        availableActionsScript.finishedAiming = false;
        availableActionsScript.StopShooting();
        currentObjective = CurrentObjective.AimAtGround;
        SetTargetAcrossArena();
        //StartCoroutine(ChangeBehaviorToAimingAtGround());
    }
    IEnumerator ChangeBehaviorToAimingAtGround()
    {
        yield return new WaitForSeconds(.1f);
        currentObjective = CurrentObjective.AimAtGround;
    }
    private void AimAtGround()
    {
        if(availableActionsScript.finishedAiming) startFlyingToCeiling();
    }
    private void startFlyingToCeiling()
    {
        currentObjective = CurrentObjective.FlyToCeiling;
        performMidBehaviorAction = false;
        availableActionsScript.StartFlying();
        availableActionsScript.LaunchMagnet();
        availableActionsScript.StartRepel();
        //availableActionsScript.StartShooting();
        StartCoroutine(LaunchMagnetToAttract());
    }
    private void FlyToCeiling()
    {
        if (gameObject.transform.position.x <= (centerCeilingTransform.position.x-.2f)) availableActionsScript.Move(1f, 0f);
        else if (gameObject.transform.position.x >= (centerCeilingTransform.position.x + .2f)) availableActionsScript.Move(-1f, 0f);
        else availableActionsScript.Move(0f, 0f);
        if(performMidBehaviorAction) availableActionsScript.SetAimTarget((Vector2)(centerCeilingTransform.position - gameObject.transform.position));
    }
    IEnumerator LaunchMagnetToAttract()
    {
        yield return new WaitForSeconds(.17f);
        availableActionsScript.StopRepel();
        availableActionsScript.SetAimTarget((Vector2) (centerCeilingTransform.position - gameObject.transform.position));
        availableActionsScript.finishedAiming = false;
        performMidBehaviorAction = true;
        yield return new WaitForSeconds(.01f);
        while (!availableActionsScript.finishedAiming) yield return new WaitForSeconds(.01f);
        StartAttracting();
    }
    private void StartAttracting()
    {
        performMidBehaviorAction = false;
        availableActionsScript.LaunchMagnet();
        availableActionsScript.StartAttract();
        availableActionsScript.StopFlying();
        StartTrackingAndShootingPlayer();
        StartCoroutine(EndCeilingBehavior());
    }
    IEnumerator EndCeilingBehavior()
    {
        yield return new WaitForSeconds(5);
        availableActionsScript.StopAttract();
        currentObjective = CurrentObjective.MoveAcrossScreen;
        yield return new WaitForSeconds(.5f);
        availableActionsScript.RecoverMagnet();
        yield return new WaitForSeconds(.1f);
        availableActionsScript.MagnetRetrieved();
    }

    //Move Across Screen With Attraction Scripts
    private void StartAimAcrossArena()
    {
        trackPlayer = false;
        availableActionsScript.Move(0, 0);
        SetTargetAcrossArena();
        availableActionsScript.SetAimTarget(new Vector2(startedLeft ? 1 : -1, 0f));
        availableActionsScript.finishedAiming = false;
        availableActionsScript.StopShooting();
        currentObjective = CurrentObjective.AimAcrossArena;
    }
    private void AimAcrossArena()
    {
        if (availableActionsScript.finishedAiming) StartMoveAcrossScreenWithAttraction();
    }

    private void StartMoveAcrossScreenWithAttraction()
    {
        currentObjective = CurrentObjective.MoveAcrossScreenWithAttraction;
        availableActionsScript.LaunchMagnet();
        availableActionsScript.StartAttract();
        StartTrackingAndShootingPlayer();
    }
    private void MoveAcrossScreenWithAttraction()
    {
        if (startedLeft)
        {
            availableActionsScript.Move(1f, 0f);
            if (gameObject.transform.position.x >= targetX)
            {
                StartCoroutine(EndMoveAcrossScreenWithAttraction());
            }
        }
        else
        {
            availableActionsScript.Move(-1f, 0f);
            if (gameObject.transform.position.x <= targetX)
            {
                StartCoroutine(EndMoveAcrossScreenWithAttraction());
            }
        }
    }
    IEnumerator EndMoveAcrossScreenWithAttraction()
    {
        availableActionsScript.Move(startedLeft ? -1f : 1f, 0f);
        availableActionsScript.StopAttract();
        yield return new WaitForSeconds(.7f);
        availableActionsScript.Move(0f, 0f);
        availableActionsScript.RecoverMagnet();
        yield return new WaitForSeconds(.1f);
        availableActionsScript.MagnetRetrieved();
        FinishBehavior(3);
    }
    //Launch Across Screen functions
    private void StartAimBehindMech()
    {
        trackPlayer = false;
        availableActionsScript.Move(0, 0);
        SetTargetAcrossArena();
        availableActionsScript.SetAimTarget(new Vector2(startedLeft ? -.7f : .7f, -.6f));
        availableActionsScript.finishedAiming = false;
        availableActionsScript.StopShooting();
        currentObjective = CurrentObjective.AimBehindMech;
    }
    private void AimBehindMech()
    {
        if (availableActionsScript.finishedAiming) StartLaunchAcrossScreen();
    }

    private void StartLaunchAcrossScreen()
    {
        Debug.Log("test 1");
        currentObjective = CurrentObjective.launchAcrossScreen;
        availableActionsScript.LaunchMagnet();
        StartCoroutine(midLaunchAcrossScreenActions());
        StartTrackingAndShootingPlayer();
    }
    IEnumerator midLaunchAcrossScreenActions()
    {
        yield return new WaitForSeconds(.2f);
        availableActionsScript.StartRepel();
        availableActionsScript.StartFlying();
        yield return new WaitForSeconds(.12f);
        availableActionsScript.StopFlying();
        yield return new WaitForSeconds(.5f);
        availableActionsScript.StopRepel();
    }
    private void LaunchAcrossScreen()
    {
        if (startedLeft)
        {
            availableActionsScript.Move(1f, 0f);
            if (gameObject.transform.position.x >= targetX)
            {
                StartCoroutine(EndLaunchingAcrossScreen());
            }
        }
        else
        {
            availableActionsScript.Move(-1f, 0f);
            if (gameObject.transform.position.x <= targetX)
            {
                StartCoroutine(EndLaunchingAcrossScreen());
            }
        }
    }
    IEnumerator EndLaunchingAcrossScreen()
    {
        availableActionsScript.Move(startedLeft ? -1f : 1f, 0f);
        yield return new WaitForSeconds(1.2f);
        availableActionsScript.Move(0f, 0f);
        availableActionsScript.RecoverMagnet();
        yield return new WaitForSeconds(.05f);
        availableActionsScript.MagnetRetrieved();
        FinishBehavior(4);
    }
}
