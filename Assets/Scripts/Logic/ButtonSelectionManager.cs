using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;

public class ButtonSelectionManager : MonoBehaviour
{
    //manages selecting buttons in the menu and pause screen
    [Header("Game Objects")]
    public Transform pauseButtonParent;
    public Transform settingsButtonParent;
    [Header("Variables")]
    public List<GameObject> pauseButtons;
    public List<GameObject> settingsButtons;
    [Header("Timers")]
    private float delay = .02f;
    private float readyToChange = 0f;
    public int currentSelection = 0;

    private List<GameObject> buttons;
    private bool isEnabled;
    public GameObject pauseEffect;
    private LogicScript.GameMenuState currentGameMenuState;

    [SerializeField] private float[] pauseEffectHeights;

    private void Awake()
    {
        foreach (Transform child in pauseButtonParent) {
            GameObject button = child.gameObject;
            pauseButtons.Add(button);
        }
        foreach (Transform child in settingsButtonParent) {
            GameObject button = child.gameObject;
            settingsButtons.Add(button);
        }
        SetGameMenuState(LogicScript.GameMenuState.PAUSE_MENU);
        GameObject savedVariablesObject = GameObject.FindGameObjectWithTag("MultiSceneVariables");
        SetButtonSize(currentSelection);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!isEnabled) {
            return;
        }

        float change = context.ReadValue<Vector2>().x - context.ReadValue<Vector2>().y;
        if(Time.realtimeSinceStartup > readyToChange)
        {
            if(change > .25)
            {
                currentSelection += 1;
                if(currentSelection >= buttons.Count)
                {
                    currentSelection = 0;
                }
            }
            else if (change < -.25)
            {
                currentSelection -= 1;
                if (currentSelection < 0)
                {
                    currentSelection = buttons.Count - 1;
                }
            }
            readyToChange = Time.realtimeSinceStartup + delay;
            SetButtonSize(currentSelection);
        }
    }

    public void SetButtonSize(int currentSelection)
    {
        Debug.Log(currentSelection);
        if (!isEnabled) {
            return;
        }
        
        foreach (GameObject button in buttons) 
        {
            button.GetComponent<RectTransform>().localScale = Vector3.one;
        }
        if (pauseEffect.activeInHierarchy)
        {
            pauseEffect.transform.localPosition = new Vector3(pauseEffect.transform.localPosition.x, pauseEffectHeights[currentSelection], 0f);
        }
        buttons[currentSelection].GetComponent<RectTransform>().localScale = new Vector3(1.25f, 1.25f, 1.25f);
    }

    public void Select(InputAction.CallbackContext context)
    {
        if (!isEnabled) {
            return;
        }

        if (context.performed)
        {
            buttons[currentSelection].GetComponent<Button>().onClick.Invoke();
        }
    }

    public void SetGameMenuState(LogicScript.GameMenuState menuState) {
        switch (menuState) {
            case LogicScript.GameMenuState.PAUSE_MENU:
                buttons = pauseButtons;
                isEnabled = true;
                break;
            case LogicScript.GameMenuState.SETTINGS_MENU:
                buttons = settingsButtons;
                isEnabled = true;
                break;
            default:
            case LogicScript.GameMenuState.PLAYING:
                isEnabled = false;
                return;
        }
        currentGameMenuState = menuState;
        currentSelection = 0;
        SetButtonSize(currentSelection);
    }
    public void ResetButtonColors()
    {
        if (currentGameMenuState != LogicScript.GameMenuState.SETTINGS_MENU) return;
        foreach(GameObject button in settingsButtons)
        {
            button.GetComponent<Image>().color= Color.white;
        }
    }
    public void SetCurrentButton(int newButton)
    {
        currentSelection = newButton;
        SetButtonSize(currentSelection);
    }
}
