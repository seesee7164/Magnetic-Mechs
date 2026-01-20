using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//A script to hold all of the event
public class TutorialAllEvents : MonoBehaviour
{
    //Holds the scripts for the events which play out in cutscenes during the tutorial level
    [Header("Components")]
    public GameObject title;
    public Queue<CivilianScript> civilianScripts;
    public GameObject[] civilians;
    public LogicScript logic;
    public MultiSceneVariables multiSceneVariables;

    //[Header("Agent")]
    //public GameObject agentPrefab;
    //private float agentHeight = 6f;

    [Header("Player")]
    public GameObject player;
    public PlayerScript playerScript;
    public GameObject pilotsDeath;

    [Header("Specific Dialogue")]
    public GameObject firstDialogueToTrigger;
    private CutsceneScript lastCutscenePartThree;
    private CutsceneScript lastCutscenePartFour;

    [Header("Prompt")]
    private PromptUIScript promptUIScript;
    public PlayPromptScript shootingPromptScript;

    [Header("Kill Pilot")]
    private PlayPromptScript killPilotPromptScript;
    private bool startKillingPilot;
    private bool inputOne;
    private bool inputTwo;
    void Awake()
    {
        //set up player
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();

        pilotsDeath = GameObject.FindGameObjectWithTag("PilotsDeath");
        GameObject promptUI = GameObject.FindGameObjectWithTag("PromptText");
        GameObject killPilotPrompt = GameObject.FindGameObjectWithTag("KillPilotPrompt");
        GameObject lastCutscenePartThreeObject = GameObject.FindGameObjectWithTag("LastCutscenePartThree");
        GameObject lastCutscenePartFourObject = GameObject.FindGameObjectWithTag("LastCutscenePartFour");
        GameObject shootingPrompt = GameObject.FindGameObjectWithTag("ShootingPrompt");
        multiSceneVariables = GameObject.FindGameObjectWithTag("MultiSceneVariables").GetComponent<MultiSceneVariables>();
        startKillingPilot = inputOne = inputTwo = false;

        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();

        //title = GameObject.FindGameObjectWithTag("TitleDrop").GetComponent<Text>();
        //title.enabled = false;
        civilians = GameObject.FindGameObjectsWithTag("Civilian");
        civilianScripts = new Queue<CivilianScript>();
        foreach (GameObject civilian in civilians) 
        {
            civilianScripts.Enqueue(civilian.GetComponent<CivilianScript>());
        }
        if (promptUI != null) promptUIScript = promptUI.GetComponent<PromptUIScript>();
        else Debug.Log("Can't find PromptText");
        if (killPilotPrompt != null) killPilotPromptScript = killPilotPrompt.GetComponent<PlayPromptScript>();
        else Debug.Log("Can't find KillPilotPrompt");
        if (lastCutscenePartThreeObject != null) lastCutscenePartThree = lastCutscenePartThreeObject.GetComponent<CutsceneScript>();
        else Debug.Log("Can't find LastCutscenePartThree");
        if (lastCutscenePartFourObject != null) lastCutscenePartFour = lastCutscenePartFourObject.GetComponent<CutsceneScript>();
        else Debug.Log("Can't find LastCutscenePartFour");
        if (shootingPrompt != null) shootingPromptScript = shootingPrompt.GetComponent<PlayPromptScript>();
        else Debug.Log("Can't find ShootingPrompt");

    }

    private void FixedUpdate()
    {
        //manages the cutscene for killing the pilot. Will likely end up being 
        if(playerScript == null) return;
        if (startKillingPilot) {
            if (playerScript.gamePadNotMouse)
            {
                if (playerScript.repelOn && playerScript.jumpPressed && playerScript.shootingInput)
                {
                    killPilot();
                }
            }
            else
            {
                inputOne = Input.GetKey(KeyCode.G);
                inputTwo = Input.GetKey(KeyCode.L);
                if (inputOne && inputTwo)
                {
                    killPilot();
                }
            }
        }

    }
    //public functions
    public void AfterAgentDestroyed()
    {

        enablePlayerMovement();
        promptUIScript.stopIndefinitePrompt();
        StartCoroutine(TriggerFinalCutscenePartTwo());
    }

    public IEnumerator TriggerFinalCutscenePartTwo()
    {
        yield return new WaitForSeconds(2);
        if (firstDialogueToTrigger!=null) firstDialogueToTrigger.GetComponent<CutsceneScript>().triggerDisplayDialogue();
    }

    //Hold Events
    public void disablePlayerMovement()
    {
        if (playerScript!=null)playerScript.DisableMovement();
    }
    public void enablePlayerMovement()
    {
        if (playerScript != null) playerScript.EnableMovement();
    }

    public void enablePrompt()
    {
        promptUIScript.promptEnabled = true;
    }
    public void disablePrompt()
    {
        promptUIScript.promptEnabled = false;
    }

    public void disablePlayerMovementAndPrompt()
    {
        disablePlayerMovement();
        disablePrompt();
    }
    public void enablePlayerMovementAndPrompt()
    {
        enablePlayerMovement();
        enablePrompt();
    }

    //final cutscene events

    public void targetCivilians()
    {
        shootingPromptScript.playPromptIndefinitely();
        foreach (CivilianScript civilianScript in civilianScripts)
        {
            civilianScript.turnOnTargetingReticle();
        }
    }
    public void spawnAgent()
    {
        AfterAgentDestroyed();
        //GameObject agent = Instantiate(agentPrefab, new Vector3(player.transform.position.x, player.transform.position.y + agentHeight, 0), player.transform.rotation);
    }

    public void startKillPilotEvent()
    {
        startKillingPilot = true;
        killPilotPromptScript.playPromptIndefinitely();
    }
    public void killPilot()
    {
        startKillingPilot = false;
        promptUIScript.stopIndefinitePrompt();
        lastCutscenePartThree.triggerDisplayDialogue();
    }
    public void killingPilotEvent()
    {
        StartCoroutine(killingPilot());
    }
    public IEnumerator killingPilot()
    {
        if (playerScript == null) { 
            Debug.Log("playerScript disappeared");
            yield break; 
        }
        if (!playerScript.torsoFacingRight)
        {
            pilotsDeath.transform.localScale = new Vector3(pilotsDeath.transform.localScale.x * -1, pilotsDeath.transform.localScale.y, pilotsDeath.transform.localScale.z);
        }
        pilotsDeath.GetComponent<ParticleSystem>().Play();
        pilotsDeath.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(.01f);
        foreach (CivilianScript civilianScript in civilianScripts)
        {
            civilianScript.turnOffTargetingReticle();
        }
        lastCutscenePartFour.triggerDisplayDialogue();
    }

    public void titleDropEvent()
    {
        StartCoroutine(titleDrop());
    }
    public IEnumerator titleDrop()
    {
        yield return new WaitForSeconds(1);
        disablePlayerMovement();
        title.SetActive(true);
        StartCoroutine(startNextScene());
    }
    public IEnumerator startNextScene()
    {
        yield return new WaitForSeconds(2);
        logic.StartLevel("Level 2");
    }
    public void FinishLevel1()
    {
        multiSceneVariables.finishLevel(1);
    }
}
