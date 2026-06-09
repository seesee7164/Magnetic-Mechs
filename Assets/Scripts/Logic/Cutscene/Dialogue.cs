using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue : MonoBehaviour
{
    //handles turning on the dialogue box and displaying the appropriate diagloue
    private string title;
    private Queue<DialogueIndividualLine> dialogue;
    private float endCutsceneTime;
    private bool skipCutscene = false;
    //private bool confirmationNotSent;
    public void postHocConstructor(string title, params DialogueIndividualLine[] linesToAdd)
    {
        this.title = title;
        dialogue = new Queue<DialogueIndividualLine>();
        if (linesToAdd != null) {
            foreach (DialogueIndividualLine line in linesToAdd)
            {
                dialogue.Enqueue(line);
            }
        }
    }
    public void displayDialogue(GameObject dialogueBox,GameObject dialogueText, GameObject dialogueImage, GameObject dialogueAudio, CutsceneManager cutsceneManagerScript, UnityEvent preCutsceneEvent, UnityEvent postCutsceneEvent)
    {
        StartCoroutine(handleDialogue(dialogueBox,dialogueText, dialogueImage, dialogueAudio, cutsceneManagerScript, preCutsceneEvent, postCutsceneEvent));
    }
    public IEnumerator handleDialogue(GameObject dialogueBox, GameObject dialogueText, GameObject dialogueImage, GameObject dialogueAudio, CutsceneManager cutsceneManagerScript, UnityEvent preCutsceneEvent, UnityEvent postCutsceneEvent)
    {
        dialogueBox.SetActive(true);
        //dialogueText.SetActive(true);
        //dialogueImage.SetActive(true);
        //dialogueAudio.SetActive(true);
        TextMeshProUGUI textBox = dialogueText.GetComponent<TextMeshProUGUI>();
        Image imageBox = dialogueImage.GetComponent<Image>();
        AudioSource audioBox = dialogueAudio.GetComponent<AudioSource>();
        if(preCutsceneEvent != null) {
            preCutsceneEvent.Invoke();
        }
        while (dialogue.Count > 0)
        {
            DialogueIndividualLine nextLine = dialogue.Dequeue();
            textBox.text = nextLine.lineToLoad;
            //speaker image
            Sprite loadedSprite = Resources.Load<Sprite>(nextLine.speakerImage);
            switch(loadedSprite)
            {
                case null:
                    Debug.Log("File could not be found");
                    break;
                default:
                    imageBox.sprite = loadedSprite;
                    break;
            }
            //speaker audio
            AudioClip loadedAudio;
            if (nextLine.speakerAudio != "")
            {
                loadedAudio = Resources.Load<AudioClip>(nextLine.speakerAudio);
            }
            else
            {
                loadedAudio = null;
            }
            switch (loadedAudio)
            {
                case null:
                    Debug.Log("Silence");
                    break;
                default:
                    audioBox.clip = loadedAudio;
                    audioBox.Play();
                    break;
            }
            setTimer(nextLine.timeToPlay);
            while (!skipCutscene&&Time.time < endCutsceneTime)
            {
                yield return null;
            }
            yield return new WaitForSeconds(.1f);
            skipCutscene = false;
        }
        cutsceneManagerScript.currentCutsceneScript = null;
        if (postCutsceneEvent != null)
        {
            postCutsceneEvent.Invoke();
        }
        dialogueBox.SetActive(false);
        //dialogueText.SetActive(false);
        //dialogueImage.SetActive(false);
        //dialogueAudio.SetActive(false);
    }

    void setTimer(float waitTime)
    {
        endCutsceneTime = Time.time + waitTime;
    }
    public void StartSkip()
    {
        skipCutscene = true;
    }
}
