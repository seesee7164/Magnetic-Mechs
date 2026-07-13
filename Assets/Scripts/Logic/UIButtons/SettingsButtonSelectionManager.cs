using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsButtonSelectionManager : ButtonSelectionParent
{
    [Header("Components")]
    public Transform settingsButtonParent;
    public List<GameObject> settingsButtons;

    [Header("Timers")]
    private float delay = .02f;
    private float readyToChange = 0f;

    private bool isEnabled = true;
    public float displayChange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        foreach (Transform child in settingsButtonParent)
        {
            GameObject button = child.gameObject;
            settingsButtons.Add(button);
        }
        GameObject savedVariablesObject = GameObject.FindGameObjectWithTag("MultiSceneVariables");
        SetButtonSize(currentSelection);
    }

    public override void SetButtonSize(int currentSelection)
    {
        if (!isEnabled)
        {
            return;
        }

        foreach (GameObject button in settingsButtons)
        {
            button.GetComponent<RectTransform>().localScale = Vector3.one;
        }
        settingsButtons[currentSelection].GetComponent<RectTransform>().localScale = new Vector3(1.25f, 1.25f, 1.25f);
    }
    public void Move(InputAction.CallbackContext context)
    {
        if (!isEnabled)
        {
            return;
        }
        Debug.Log("test 1");
        float change = context.ReadValue<Vector2>().x - context.ReadValue<Vector2>().y;
        if (Time.realtimeSinceStartup > readyToChange)
        {
            if (change > .25)
            {
                currentSelection += 1;
                if (currentSelection >= settingsButtons.Count)
                {
                    currentSelection = 0;
                }
            }
            else if (change < -.25)
            {
                currentSelection -= 1;
                if (currentSelection < 0)
                {
                    currentSelection = settingsButtons.Count - 1;
                }
            }
            readyToChange = Time.realtimeSinceStartup + delay;
            SetButtonSize(currentSelection);
        }
    }
    public void Select(InputAction.CallbackContext context)
    {
        if (!isEnabled)
        {
            return;
        }

        if (context.performed)
        {
            settingsButtons[currentSelection].GetComponent<Button>().onClick.Invoke();
        }
    }
}
