using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneScript : MonoBehaviour
{
    //this scripts is set on each cutscene object, and manages displaying that cutscene
    private Dialogue currentDialogue;
    private GameObject allCutscenes;
    private bool hasNotTriggered;
    [Header("Public Variables")]
    public int cutsceneInt;
    [Header("Components")]
    public TutorialAllCutscenes allCutscenesScript;
    public TutorialAllEvents allCutscenesEvents;
    private GameObject dialogueBox;
    private GameObject dialogueText;
    private GameObject dialogueImage;
    private GameObject dialogueAudio;
    public CutsceneManager cutsceneManagerScript;

    [Header("Events")]
    public UnityEvent preCutsceneEvent;
    public UnityEvent postCutsceneEvent;
    private UnityEvent endCutscene;
    // Start is called before the first frame update
    void Start()
    {
        hasNotTriggered = true;
        GameObject cutsceneManager = GameObject.FindGameObjectWithTag("CutsceneManager");
        if (cutsceneManager != null) cutsceneManagerScript = cutsceneManager.GetComponent<CutsceneManager>();
        else Debug.Log("Can't find CutsceneManager");
        allCutscenes = GameObject.FindGameObjectWithTag("AllCutscenes");
        if (allCutscenes == null)
        {
            Debug.Log("Can't find AllCutscenes");
            return;
        }
        allCutscenesScript = allCutscenes.GetComponent<TutorialAllCutscenes>();
        currentDialogue = allCutscenesScript.cutscenes[cutsceneInt];
        dialogueBox = allCutscenesScript.dialogueBox;
        dialogueImage = allCutscenesScript.dialogueImage;
        dialogueText = allCutscenesScript.dialogueText;
        dialogueAudio = allCutscenesScript.dialogueAudio;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasNotTriggered && collision.gameObject.layer == 3)
        {
            hasNotTriggered = false;
            triggerDisplayDialogue();
        }
    }
    public void triggerDisplayDialogue()
    {
        cutsceneManagerScript.currentCutsceneScript = gameObject.GetComponent<CutsceneScript>();
        currentDialogue.displayDialogue(dialogueBox, dialogueText, dialogueImage, dialogueAudio, cutsceneManagerScript, preCutsceneEvent, postCutsceneEvent);
    }
    public void SkipCutscene()
    {
        currentDialogue.StartSkip();
    }
}
