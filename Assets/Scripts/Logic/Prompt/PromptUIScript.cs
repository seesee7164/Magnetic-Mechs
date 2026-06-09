using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class PromptUIScript : MonoBehaviour
{
    //handles the UI for prompts during the tutorial level
    //private Text promptText;
    private TextMeshProUGUI promptText;
    private Prompt currentPrompt;
    [Header("parameters")]
    public bool promptActive;
    public bool promptEnabled;
    [Header("Arrow Component")]
    private GameObject directionalArrow;
    private Transform directionalArrowOrientation;
    [Header("Don't Clear Prompt")]
    public bool playIndefinitely;

    // Start is called before the first frame update
    void Start()
    {
        promptText = GetComponent<TextMeshProUGUI>();
        directionalArrow = GameObject.FindGameObjectWithTag("DirectionalArrow");
        GameObject directionalArrowOrientationObject = GameObject.FindGameObjectWithTag("DirectionalArrowOrientation");
        if (directionalArrowOrientationObject != null) directionalArrowOrientation = directionalArrowOrientationObject.GetComponent<Transform>();
        else Debug.Log("Can't find DirectionalArrowOrientation");

        promptEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (playIndefinitely)
        {
            return;
        }
        if (currentPrompt == null)
        {
            promptText.enabled = false;
            directionalArrow.GetComponent<Image>().enabled = false;
        }
        clearPrompt();
    }
    public void updateCurrentPrompt(Prompt prompt, float arrowOrientation)
    {
        if(promptEnabled&&(currentPrompt == null||prompt.priority > currentPrompt.priority))
        {
            updatePrompt(prompt, arrowOrientation);
        }
    }
    private void updatePrompt(Prompt prompt,float arrowOrientation)
    {
        currentPrompt = prompt;
        promptText.text = currentPrompt.promptText;
        promptText.enabled = true;
        if (arrowOrientation != 360)
        {
            directionalArrow.GetComponent<Image>().enabled = true;
            directionalArrowOrientation.localRotation = Quaternion.Euler(0, 0, arrowOrientation);

        }
        else
        {
            directionalArrow.GetComponent<Image>().enabled = false;
        }
    }
    private void clearPrompt()
    {
        currentPrompt = null;
    }
    public void stopIndefinitePrompt()
    {
        playIndefinitely = false;
    }
}
