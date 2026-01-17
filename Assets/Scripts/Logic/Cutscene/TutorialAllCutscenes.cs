using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAllCutscenes : MonoBehaviour
{
    //a file which holds all of the dialgoue, images and sounds used in cutscenes
    public Dictionary<string,Dialogue> cutscenes;
    private Dictionary<string, string> imageLocations;
    private Dictionary<string, string> audioLocations;
    [Header("Dialogues")]
    private Dialogue firstCutscene;
    private Dialogue magnetCutscene;
    private Dialogue lastCutscenePartOne;
    private Dialogue lastCutscenePartTwo;
    private Dialogue lastCutscenePartThree;
    private Dialogue lastCutscenePartFour;
    //private Dialogue lastCutscenePartFive;
    [Header("Components")]
    public GameObject dialogueBox;
    public GameObject dialogueText;
    public GameObject dialogueImage;
    public GameObject dialogueAudio;
    

    // Start is called before the first frame update
    void Awake()
    {
        //set up images
        imageLocations = new Dictionary<string, string>
        {
            { "General", "DialogueImages/High_Command" },
            { "Player", "DialogueImages/PlayerImage" },
            { "Goon", "DialogueImages/Goon" },
            { "DeadGoon", "DialogueImages/Dead_Goon" },

        };
        //set up audio
        audioLocations = new Dictionary<string, string>
        {
            { "General", "DialogueAudio/DM-CGS-38" },
            { "Player", "DialogueAudio/High-Beep" }
        };
        //set up cutscenes
        cutscenes = new Dictionary<string, Dialogue>();

        //first cutscene
        firstCutscene = gameObject.AddComponent<Dialogue>();
        firstCutscene.postHocConstructor(
        "First Cutscene",
        new DialogueIndividualLine("Alright Ace 1, we need you to proceed and eliminate any and all hostiles in the area", imageLocations["General"], audioLocations["General"], 2.5f),
        new DialogueIndividualLine("Maneuver the mech over and proceed with the mission. Report back upon completion, over", imageLocations["General"], audioLocations["General"], 2.5f),
        new DialogueIndividualLine("Mech, head over to the right.", imageLocations["Goon"], audioLocations["General"], 1.5f),
        new DialogueIndividualLine("Standby. Orders not understood. Hostiles are not…", imageLocations["Player"], audioLocations["Player"], 2.5f),
        new DialogueIndividualLine("Don’t make us go through this again. You proceed with my order. Do you damn job.", imageLocations["Goon"], audioLocations["General"], 2.5f)
        );
        cutscenes.Add("First Cutscene", firstCutscene);

        //magnet cutscene
        magnetCutscene = gameObject.AddComponent<Dialogue>();
        magnetCutscene.postHocConstructor(
        "Magnet Cutscene",
        new DialogueIndividualLine("Mission control, we've encountered a cliff too large for our jetpacks to get over it. Next steps needed.", imageLocations["Goon"], audioLocations["General"], 3),
        new DialogueIndividualLine("Ace 1, that mech is equipped with state of the art magnetic technology for this issue.", imageLocations["General"], audioLocations["General"], 2.5f),
        new DialogueIndividualLine("Get your mech to proceed. Make it happen.", imageLocations["General"], audioLocations["General"], 1.5f),
        new DialogueIndividualLine("Mech, you heard. Hurry and do it.", imageLocations["Goon"], audioLocations["General"], 2)
        );
        cutscenes.Add("Magnet Cutscene", magnetCutscene);

        //Finale Part One Cutscene
        lastCutscenePartOne = gameObject.AddComponent<Dialogue>();
        lastCutscenePartOne.postHocConstructor(
        "Last Cutscene Part One",
        new DialogueIndividualLine("Mission control, multiple hostiles identified.", imageLocations["Goon"], audioLocations["General"], 2),
        new DialogueIndividualLine("Excellent work, neutralize them.", imageLocations["General"], audioLocations["General"], 2),
        new DialogueIndividualLine("But, sir, this is not right. Permission to revert course…", imageLocations["Player"], audioLocations["Player"], 2),
        new DialogueIndividualLine("I didn’t say a damn thing about reverting course. Listen and kill, or you'll be parts for my next mech.", imageLocations["Goon"], audioLocations["General"], 4),
                new DialogueIndividualLine("You see the hostiles? Neutralize. Now.", imageLocations["Goon"], audioLocations["General"], 2)

        );
        cutscenes.Add("Last Cutscene Part One", lastCutscenePartOne);

        //Last Cutscene Part Two
        lastCutscenePartTwo = gameObject.AddComponent<Dialogue>();
        lastCutscenePartTwo.postHocConstructor(
        "Last Cutscene Part Two",
        new DialogueIndividualLine("...", imageLocations["Player"], "", 2),
        new DialogueIndividualLine("... I understand.", imageLocations["Player"], audioLocations["Player"], 2)
        );
        cutscenes.Add("Last Cutscene Part Two", lastCutscenePartTwo);

        //Last Cutscene Part Three
        lastCutscenePartThree = gameObject.AddComponent<Dialogue>();
        lastCutscenePartThree.postHocConstructor(
        "Last Cutscene Part Three",
        new DialogueIndividualLine("Damn it, my override controls are completely fried. The dumb mech’s not cooperating at all.", imageLocations["Goon"], audioLocations["General"], 3),
        new DialogueIndividualLine("Hostile identified. Neutralizing.", imageLocations["Player"], audioLocations["Player"], 2),
        new DialogueIndividualLine("Wait, what's this thing -", imageLocations["Goon"], audioLocations["General"], 2)
        );
        cutscenes.Add("Last Cutscene Part Three", lastCutscenePartThree);

        //Last Cutscene Part Four
        lastCutscenePartFour = gameObject.AddComponent<Dialogue>();
        lastCutscenePartFour.postHocConstructor(
        "Last Cutscene Part Four",
        new DialogueIndividualLine("AAAHHHHHHHHHHHHHHHHHH…", imageLocations["Goon"], "", 3),
        new DialogueIndividualLine("…", imageLocations["Player"], "", 2),
        new DialogueIndividualLine("What in the…", imageLocations["General"], audioLocations["General"], 2),
        new DialogueIndividualLine("Ace 1… do you copy Ace 1 ? ", imageLocations["General"], audioLocations["General"], 2),
        new DialogueIndividualLine("Pilot, do you copy? Can you copy? What is going on?", imageLocations["General"], audioLocations["General"], 2.5f),
        new DialogueIndividualLine("Something’s wrong… we need better visuals. We have lost control…", imageLocations["General"], audioLocations["General"], 3),
        new DialogueIndividualLine("…", imageLocations["Player"], "", 2),
        new DialogueIndividualLine("Neutralize hostiles", imageLocations["Player"], "", 2)
        );
        cutscenes.Add("Last Cutscene Part Four", lastCutscenePartFour);

        ////Last Cutscene Part five
        //lastCutscenePartFive = gameObject.AddComponent<Dialogue>();
        //lastCutscenePartFive.postHocConstructor(
        //"Last Cutscene Part Five",
        //new DialogueIndividualLine("What in the…", imageLocations["General"], "", 2),
        //new DialogueIndividualLine("Ace 1… do you copy Ace 1 ? ", imageLocations["General"], "", 2),
        //new DialogueIndividualLine("Pilot, do you copy? Can you copy? What is going on?", imageLocations["General"], "", 2.5f),
        //new DialogueIndividualLine("Something’s wrong… we need better visuals. We have lost control…", imageLocations["General"], "", 3),
        //new DialogueIndividualLine("…", imageLocations["Player"], "", 2),
        //new DialogueIndividualLine("Neutralize hostiles", imageLocations["Player"], "", 2)
        //);
        //cutscenes.Add("Last Cutscene Part Five", lastCutscenePartFive);

        //grab components
        dialogueBox = GameObject.FindGameObjectWithTag("DialogueBox");
        dialogueImage = GameObject.FindGameObjectWithTag("DialogueImage");
        dialogueText = GameObject.FindGameObjectWithTag("DialogueText");
        dialogueAudio = GameObject.FindGameObjectWithTag("DialogueAudio");
        if (dialogueBox != null) dialogueBox.SetActive(false);
    }
}
