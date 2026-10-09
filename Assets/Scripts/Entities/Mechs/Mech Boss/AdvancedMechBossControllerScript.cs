using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class AdvancedMechBossControllerScript : MechBossControllerScript
{
    [Header("Components")]
    public MechBossHealthScript mechBossHealthScript;
    protected override void Awake()
    {
        base.Awake();
        UpdateBossStatisticsToAdvanced();
    }
    protected override void HandleBehaviorTimer()
    {
        if (currentObjective == CurrentObjective.Stationary)
        {
            if (startBehaviorTimer >= currentCooldown)
            {
                float newBehavior = Random.value;
                Debug.Log(newBehavior);
                //StartAimAtMiddleGround();
                if (newBehavior <= moveAcrossScreenWithAttractionProbability) StartAimAcrossArena();
                else if (newBehavior <= flyToCeilingProbability) StartAimAtGround();
                else if (newBehavior <= launchAcrossScreenProbability) StartAimBehindMech();
                else if (newBehavior <= attractThenRepelAcrossScreenProbability) StartAimAtMiddleGround();
                else StartMoveAcrossScreen();
                startBehaviorTimer = 0f;
            }
            else
            {
                startBehaviorTimer += Time.fixedDeltaTime;
            }
        }
    }
    protected override void PerformObjectiveBehavior()
    {
        base.PerformObjectiveBehavior();
        if (currentObjective == CurrentObjective.AimAtMiddleGround) AimAtMiddleGround();
        else if (currentObjective == CurrentObjective.AttractToMiddleGround) AttractToMiddleGround();
        else if (currentObjective == CurrentObjective.CheckToRepelFromMiddleGround) CheckingToRepelFromMiddleGround();
        else if (currentObjective == CurrentObjective.RepelFromMiddleGround) RepelFromMiddleGround();
    }
    private void UpdateBossStatisticsToAdvanced()
    {
        //update probabilities
        moveAcrossScreenWithAttractionProbability = .2f;
        flyToCeilingProbability = moveAcrossScreenWithAttractionProbability + .25f;
        launchAcrossScreenProbability = flyToCeilingProbability + .25f;
        attractThenRepelAcrossScreenProbability = launchAcrossScreenProbability + .2f;
        //update statistics
        mechBossHealthScript.AdvancedMechBossHealth();
        currentCooldown = 1.5f;
        //update cooldowns
        moveAcrossScreenCooldown = 1f;
        moveAcrossScreenWithAttractionCooldown = 1.5f;
        LaunchingAcrossScreenCooldown = 2f;
    }

    //moving to match player height
    private void AdjustToMatchPlayersHeight()
    {
        float playerHeight = availableActionsScript.playerTransform.position.y;
        Debug.Log("player height" + playerHeight);
        Debug.Log("mech height" + transform.position.y);
        if (transform.position.y <= playerHeight - 1)
        {
            availableActionsScript.jumpPressed = true;
        }
        else
        {
            availableActionsScript.jumpPressed = false;
        }
    }
    protected override void MoveAcrossScreen()
    {
        base.MoveAcrossScreen();
        AdjustToMatchPlayersHeight();
    }
    protected override void MoveAcrossScreenWithAttraction()
    {
        base.MoveAcrossScreenWithAttraction();
        AdjustToMatchPlayersHeight();
    }

    // Attract then repel across screen
    protected void StartAimAtMiddleGround()
    {
        trackPlayer = false;
        availableActionsScript.Move(0, 0);
        availableActionsScript.SetAimTarget((Vector2)(centerFloorTransform.position - gameObject.transform.position));
        availableActionsScript.finishedAiming = false;
        availableActionsScript.StopShooting();
        currentObjective = CurrentObjective.AimAtMiddleGround;
        SetTargetAcrossArena();
        //StartCoroutine(ChangeBehaviorToAimingAtGround());
    }
    protected void AimAtMiddleGround()
    {
        if (availableActionsScript.finishedAiming) StartAttractToMiddleGround();
    }
    protected void StartAttractToMiddleGround()
    {
        currentObjective = CurrentObjective.None;
        availableActionsScript.LaunchMagnet();
        StartTrackingAndShootingPlayer();
        StartCoroutine(DelayAttractToMiddleGround());
        //availableActionsScript.StartShooting();
    }
    protected IEnumerator DelayAttractToMiddleGround()
    {
        yield return new WaitForSeconds(.4f);
        availableActionsScript.StartAttract();
        currentObjective = CurrentObjective.AttractToMiddleGround;
    }
    protected void AttractToMiddleGround()
    {
        float magnetXPosition = availableActionsScript.myMagnetManagerScript.returnMyMagnet().transform.position.x;
        if (startedLeft)
        {
            availableActionsScript.Move(1f, 0f);
            if (gameObject.transform.position.x >= magnetXPosition - 2)
            {
                StartCheckingToRepelFromMiddleGround();
            }
        }
        else
        {
            availableActionsScript.Move(-1f, 0f);
            if (gameObject.transform.position.x <= magnetXPosition + 2)
            {
                StartCheckingToRepelFromMiddleGround();
            }
        }
    }
    protected void StartCheckingToRepelFromMiddleGround()
    {
        availableActionsScript.StopAttract();
        currentObjective = CurrentObjective.CheckToRepelFromMiddleGround;
    }
    protected void CheckingToRepelFromMiddleGround()
    {
        float magnetXPosition = availableActionsScript.myMagnetManagerScript.returnMyMagnet().transform.position.x;
        if (startedLeft)
        {
            availableActionsScript.Move(1f, 0f);
            if (gameObject.transform.position.x >= magnetXPosition + 2)
            {
                Debug.Log(gameObject.transform.position.x - magnetXPosition - 2);
                StartRepellingFromMiddleGround();
            }
        }
        else
        {
            availableActionsScript.Move(-1f, 0f);
            if (gameObject.transform.position.x <= magnetXPosition - 2)
            {
                StartRepellingFromMiddleGround();
            }
        }
    }
    protected void StartRepellingFromMiddleGround()
    {
        availableActionsScript.StartRepel();
        currentObjective = CurrentObjective.RepelFromMiddleGround;
    }
    protected void RepelFromMiddleGround()
    {
        if (startedLeft)
        {
            availableActionsScript.Move(1f, 0f);
            if (gameObject.transform.position.x >= targetX)
            {
                StartCoroutine(EndAttractThenRepelAcrossScreen());
            }
        }
        else
        {
            availableActionsScript.Move(-1f, 0f);
            if (gameObject.transform.position.x <= targetX)
            {
                StartCoroutine(EndAttractThenRepelAcrossScreen());
            }
        }
    }
    protected IEnumerator EndAttractThenRepelAcrossScreen()
    {
        currentObjective = CurrentObjective.None;
        availableActionsScript.StopRepel();
        availableActionsScript.Move(startedLeft ? -1f : 1f, 0f);
        yield return new WaitForSeconds(1.1f);
        availableActionsScript.Move(0f, 0f);
        availableActionsScript.RecoverMagnet();
        yield return new WaitForSeconds(.05f);
        availableActionsScript.MagnetRetrieved();
        FinishBehavior(2f);
    }
}
