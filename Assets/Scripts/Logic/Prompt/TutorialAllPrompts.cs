using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct StringPromptPair
{
    public string myString;
    public Prompt myPrompt;
}
public class TutorialAllPrompts : MonoBehaviour
{
    //holds all prompts which might appear to the player during the tutorial
    
    public List<Prompt> prompts;
    private Dictionary<string, float> priorityOrder;
    public MultiSceneVariables savedVariables;
    private bool gamePadNotMouse = false;
    [Header("Prompts")]
    private Prompt movePrompt;
    private Prompt shootPrompt;
    private Prompt jumpPrompt;
    private Prompt afterJumpPrompt;
    private Prompt magnetOnePrompt;
    private Prompt magnetTwoPrompt;
    private Prompt recoverMagnetPrompt;
    private Prompt magnetRepelPrompt;
    private Prompt magnetAttractPrompt;
    private Prompt dropPrompt;
    private Prompt killPilotPrompt;
    [Header("Prompt Input Strings")]
    private string firstMoveString = "Move";
    private string shootString = "Shoot";
    private string jumpString = "Jump";
    private string afterJumpString = "After Jump";
    private string magnetOneString = "MagnetOne";
    private string magnetTwoString = "MagnetTwo";
    private string recoverMagnetString = "RecoverMagnet";
    private string magnetAttractString = "Attract";
    private string magnetRepelString = "Repel";
    private string dropString = "Drop";
    private string killPilotString = "Kill";
    [Header("Prompt Output Strings keyboard")]
    private string moveRightK = "Press \"A\" and \"D\" to move left and right";
    private string shootingK = "Hold “Left Mouse” to Shoot";
    private string jumpingK = "Press Space to Jump. Hold Space while mid-air to Use Jetpack";
    private string afterJumpKG = "Your Jetpack will Refill immediately on the Ground, but slowly while mid-air";
    private string magnetingOneK = "Right Mouse to Shoot Magnet. Hold \"Left Shift\" to Repel";
    private string recoverMagnetK = "Press \"Q\" to recover the Magnet. You can still fire the magnet without recovering it";
    private string magnetingTwoK = "Hold \"W\" to Attract to the magnet. Release to stop. Hold space mid-air to hover";
    private string magnetingAttractK = "Hold \"W\" to Attract. Red X Blocks will destroy the magnet if they touch it";
    private string magnetingRepelK = "Hold \"Left Shift\" to Repel";
    private string dropK = "Hold \"S\" To Drop Through Wooden Floors or to Fall Faster";
    private string killingPilotK= "Hold \"G\" + \"L\"";
    [Header("Prompt Output Strings gamePad")]
    private string moveRightG = "Use the Left Joystick to move";
    private string shootingG = "Hold Right Trigger to Shoot";
    private string jumpingG = "Press A to Jump. Hold A Mid-air to Use Jetpack";
    private string magnetingG = "Press Left Trigger to Shoot Magnet. Hold Right or Left Bumpers to Repel/Attract";
    private string dropG = "Move Left Joystick down to Drop Through Wooden Floors";
    private string killingPilotG = "Hold Left Trigger, Right Trigger, and A";

    void Awake()
    {
        //set up saved variables
        GameObject savedVariablesObject = GameObject.FindGameObjectWithTag("MultiSceneVariables");
        if (savedVariablesObject != null)
        {
            gamePadNotMouse = savedVariablesObject.GetComponent<MultiSceneVariables>().gamePadNotMouse;
        }

        //set up prompts
        prompts = new List<Prompt>();

        //set up priority order
        priorityOrder = new Dictionary<string, float>
        {
            { firstMoveString, 0 },
            { shootString, 7 },
            { jumpString, 5 },
            { magnetOneString, 6 },
            { magnetTwoString, 6 },
            { recoverMagnetString, 7 },
            { magnetRepelString, 8 },
            { magnetAttractString, 9 },
            { dropString, 4 },
            {afterJumpString, 3 },
            { killPilotString, 10 }
        };

        //move prompt 0
        movePrompt = gameObject.AddComponent<Prompt>();
        movePrompt.postHocConstructor(
        (gamePadNotMouse ? moveRightG : moveRightK),
        priorityOrder[firstMoveString]
        );
        prompts.Add(movePrompt);

        //shooting prompt 1
        shootPrompt = gameObject.AddComponent<Prompt>();
        shootPrompt.postHocConstructor(
        (gamePadNotMouse ? shootingG : shootingK),
        priorityOrder[shootString]
        );
        prompts.Add(shootPrompt);

        //jump prompt 2
        jumpPrompt = gameObject.AddComponent<Prompt>();
        jumpPrompt.postHocConstructor(
        (gamePadNotMouse ? jumpingG : jumpingK),
        priorityOrder[jumpString]
        );
        prompts.Add(jumpPrompt);
        //jump prompt 3
        afterJumpPrompt = gameObject.AddComponent<Prompt>();
        afterJumpPrompt.postHocConstructor(
        afterJumpKG,
        priorityOrder[afterJumpString]
        );
        prompts.Add(afterJumpPrompt);

        //magnet one prompt 4
        magnetOnePrompt = gameObject.AddComponent<Prompt>();
        magnetOnePrompt.postHocConstructor(
        (gamePadNotMouse ? magnetingG : magnetingOneK),
        priorityOrder[magnetOneString]
        );
        prompts.Add(magnetOnePrompt);

        //recover magnet prompt 5
        recoverMagnetPrompt = gameObject.AddComponent<Prompt>();
        recoverMagnetPrompt.postHocConstructor(
        (gamePadNotMouse ? magnetingG : recoverMagnetK),
        priorityOrder[recoverMagnetString]
        );
        prompts.Add(recoverMagnetPrompt);

        //magnet two prompt 6
        magnetTwoPrompt = gameObject.AddComponent<Prompt>();
        magnetTwoPrompt.postHocConstructor(
        (gamePadNotMouse ? magnetingG : magnetingTwoK),
        priorityOrder[magnetTwoString]
        );
        prompts.Add(magnetTwoPrompt);

        //magnet repel prompt 7
        magnetRepelPrompt = gameObject.AddComponent<Prompt>();
        magnetRepelPrompt.postHocConstructor(
        (gamePadNotMouse ? magnetingG : magnetingRepelK),
        priorityOrder[magnetRepelString]
        );
        prompts.Add(magnetRepelPrompt);

        //magnet attract prompt 8
        magnetAttractPrompt = gameObject.AddComponent<Prompt>();
        magnetAttractPrompt.postHocConstructor(
        (gamePadNotMouse ? magnetingG : magnetingAttractK),
        priorityOrder[magnetAttractString]
        );
        prompts.Add(magnetAttractPrompt);

        //drop prompt 9
        dropPrompt = gameObject.AddComponent<Prompt>();
        dropPrompt.postHocConstructor(
        (gamePadNotMouse ? dropG : dropK),
        priorityOrder[dropString]
        );
        prompts.Add(dropPrompt);

        //kill pilot prompt 10
        killPilotPrompt = gameObject.AddComponent<Prompt>();
        killPilotPrompt.postHocConstructor(
        (gamePadNotMouse ? killingPilotG : killingPilotK),
        priorityOrder[killPilotString]
        );
        prompts.Add(killPilotPrompt);
    }
}
