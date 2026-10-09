using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class MechBossControllerScript : MonoBehaviour
{
    [Header("Components")]
    public MechBossActionsScript availableActionsScript;
    public Transform centerCeilingTransform;
    public Transform centerFloorTransform;
    [Header("variables")]
    protected bool bossActive = false;
    protected bool trackPlayer = true;

    [Header("Behaviors")]
    public CurrentObjective currentObjective;

    [Header("BehaviorVariables")]
    protected bool startedLeft = false;
    protected bool performMidBehaviorAction = false;
    protected float targetX;
    [Header("Timers")]
    protected float startBehaviorTimer = 0f;
    protected float currentCooldown = 2f;
    [Header("Probabilities")]
    protected float moveAcrossScreenWithAttractionProbability = .3f;
    protected float flyToCeilingProbability = .3f;
    protected float launchAcrossScreenProbability = .3f;
    protected float attractThenRepelAcrossScreenProbability = .0f;
    //protected float moveAcrossScreenProbability = .1f;
    [Header("Cooldowns")]
    protected float moveAcrossScreenCooldown = 2f;
    protected float moveAcrossScreenWithAttractionCooldown = 2.5f;
    //protected float flyToCeilingCooldown = 2f;
    protected float LaunchingAcrossScreenCooldown = 3.5f;

    public enum CurrentObjective
    {
        None,
        Stationary,
        MoveAcrossScreen,
        AimAtGround,
        FlyToCeiling,
        AimAcrossArena,
        MoveAcrossScreenWithAttraction,
        AimBehindMech,
        launchAcrossScreen,
        AimAtMiddleGround,
        AttractToMiddleGround,
        CheckToRepelFromMiddleGround,
        RepelFromMiddleGround
    }

    protected virtual void Awake()
    {
        flyToCeilingProbability = flyToCeilingProbability + moveAcrossScreenWithAttractionProbability;
        launchAcrossScreenProbability = launchAcrossScreenProbability + flyToCeilingProbability;
        attractThenRepelAcrossScreenProbability = attractThenRepelAcrossScreenProbability + launchAcrossScreenProbability;
        currentObjective = CurrentObjective.Stationary;
    }

    // Update is called once per frame
    void Update()
    {

    }
    protected void FixedUpdate()
    {
        if (!bossActive) return;
        PerformObjectiveBehavior();
        HandleBehaviorTimer();
        if (trackPlayer) availableActionsScript.trackPlayerWithGun();

    }
    //Fixed Update Functions
    protected virtual void HandleBehaviorTimer()
    {
        if (currentObjective == CurrentObjective.Stationary)
        {
            if (startBehaviorTimer >= currentCooldown)
            {
                float newBehavior = Random.value;
                Debug.Log(newBehavior);
                if (newBehavior <= moveAcrossScreenWithAttractionProbability) StartAimAcrossArena();
                else if (newBehavior <= flyToCeilingProbability) StartAimAtGround();
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
    protected virtual void PerformObjectiveBehavior()
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
    protected void StartTrackingAndShootingPlayer()
    {
        trackPlayer = true;
        availableActionsScript.StartShooting();
    }
    protected void SetTargetAcrossArena()
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
    protected void FinishBehavior(float cooldown)
    {
        availableActionsScript.jumpPressed = false;
        currentObjective = CurrentObjective.Stationary;
        currentCooldown = cooldown;
    }
    //moving across Screen
    protected void StartMoveAcrossScreen()
    {
        currentObjective = CurrentObjective.MoveAcrossScreen;
        SetTargetAcrossArena();
    }
    protected virtual void MoveAcrossScreen()
    {
        if (startedLeft)
        {
            availableActionsScript.Move(1f, 0f);
            if(gameObject.transform.position.x >= targetX)
            {
                availableActionsScript.Move(0f, 0f);
                FinishBehavior(moveAcrossScreenCooldown);
            }
        }
        else
        {
            availableActionsScript.Move(-1f, 0f);
            if (gameObject.transform.position.x <= targetX)
            {
                availableActionsScript.Move(0f, 0f);
                FinishBehavior(moveAcrossScreenCooldown);
            }
        }
    }
    //flying to ceiling
    protected void StartAimAtGround()
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
    protected void AimAtGround()
    {
        if(availableActionsScript.finishedAiming) startFlyingToCeiling();
    }
    protected void startFlyingToCeiling()
    {
        currentObjective = CurrentObjective.FlyToCeiling;
        performMidBehaviorAction = false;
        availableActionsScript.StartFlying();
        availableActionsScript.LaunchMagnet();
        availableActionsScript.StartRepel();
        //availableActionsScript.StartShooting();
        StartCoroutine(LaunchMagnetToAttract());
    }
    protected void FlyToCeiling()
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
    protected void StartAttracting()
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
    protected void StartAimAcrossArena()
    {
        trackPlayer = false;
        availableActionsScript.Move(0, 0);
        SetTargetAcrossArena();
        availableActionsScript.SetAimTarget(new Vector2(startedLeft ? 1 : -1, 0f));
        availableActionsScript.finishedAiming = false;
        availableActionsScript.StopShooting();
        currentObjective = CurrentObjective.AimAcrossArena;
    }
    protected void AimAcrossArena()
    {
        if (availableActionsScript.finishedAiming) StartMoveAcrossScreenWithAttraction();
    }

    protected void StartMoveAcrossScreenWithAttraction()
    {
        currentObjective = CurrentObjective.MoveAcrossScreenWithAttraction;
        availableActionsScript.LaunchMagnet();
        availableActionsScript.StartAttract();
        StartTrackingAndShootingPlayer();
    }
    protected virtual void MoveAcrossScreenWithAttraction()
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
        FinishBehavior(moveAcrossScreenWithAttractionCooldown);
    }
    //Launch Across Screen functions
    protected void StartAimBehindMech()
    {
        trackPlayer = false;
        availableActionsScript.Move(0, 0);
        SetTargetAcrossArena();
        availableActionsScript.SetAimTarget(new Vector2(startedLeft ? -.7f : .7f, -.6f));
        availableActionsScript.finishedAiming = false;
        availableActionsScript.StopShooting();
        currentObjective = CurrentObjective.AimBehindMech;
    }
    protected void AimBehindMech()
    {
        if (availableActionsScript.finishedAiming) StartLaunchAcrossScreen();
    }

    protected void StartLaunchAcrossScreen()
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
    protected void LaunchAcrossScreen()
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
        yield return new WaitForSeconds(1.1f);
        availableActionsScript.Move(0f, 0f);
        availableActionsScript.RecoverMagnet();
        yield return new WaitForSeconds(.05f);
        availableActionsScript.MagnetRetrieved();
        FinishBehavior(4);
    }
}
