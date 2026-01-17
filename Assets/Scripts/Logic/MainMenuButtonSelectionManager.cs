using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class MainMenuButtonSelectionManager : MonoBehaviour
{
    //manages selecting buttons in the menu and pause screen
    [Header("Game Objects")]
    public Transform buttonParent;
    public Transform page1Parent;
    public Transform page2Parent;
    public GameObject currentLevelCapacitor;
    public GameObject previousButton;
    public GameObject nextButton;
    public GameObject beatenConnectors;
    private GameObject beatenConnectorsPageOne;
    private GameObject beatenConnectorsPageTwo;
    [Header("Variables")]
    public List<GameObject> buttons;
    public List<GameObject> page1Buttons;
    public List<GameObject> page2Buttons;
    public RuntimeAnimatorController lockedLevelAnim;
    public RuntimeAnimatorController currentLevelAnim;
    public RuntimeAnimatorController beatenLevelAnim;
    [Header("Timers")]
    private float delay = .02f;
    private float readyToChange = 0f;
    public int currentSelection = 0;
    public int currentPage = 0;
    public int currentLevel = 0;
    public int currentLevelPage;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Level 1") || PlayerPrefs.GetInt("Level 1") != 1)
        {
            PlayerPrefs.SetInt("Level 1", 1);
        }
        int i = 1;
        bool currentPicked = false;
        beatenConnectorsPageOne = beatenConnectors.transform.GetChild(0).gameObject;
        beatenConnectorsPageTwo = beatenConnectors.transform.GetChild(1).gameObject;
        foreach (Transform child in page1Parent)
        {
            if (child.gameObject.GetComponent<Button>() == null) continue;
            GameObject button = child.gameObject;
            page1Buttons.Add(button);
            if (PlayerPrefs.HasKey($"Level {i}") && PlayerPrefs.GetInt($"Level {i}") == 1)
            {
                button.GetComponent<Button>().interactable = true;
                button.GetComponent<Animator>().runtimeAnimatorController = beatenLevelAnim;
                beatenConnectorsPageOne.transform.GetChild(i - 1).gameObject.SetActive(true);
            }
            else
            {
                button.GetComponent<Button>().interactable = false;
                button.transform.GetChild(0).gameObject.SetActive(true);
                button.GetComponent<Animator>().runtimeAnimatorController = lockedLevelAnim;
            }
            if (!currentPicked && (!PlayerPrefs.HasKey($"Level {i + 1}") || PlayerPrefs.GetInt($"Level {i + 1}") != 1))
            {
                currentPicked = true;
                button.GetComponent<Animator>().runtimeAnimatorController = currentLevelAnim;
                beatenConnectorsPageOne.transform.GetChild(i - 1).gameObject.SetActive(false);
                currentLevelCapacitor.transform.position = button.transform.position;
                currentLevel = i - 1;
                currentLevelPage = 0;
            }
            i++;
        }

        foreach (Transform child in page2Parent)
        {
            if (child.gameObject.GetComponent<Button>() == null) continue;
            GameObject button = child.gameObject;
            page2Buttons.Add(button);
            if (PlayerPrefs.HasKey($"Level {i}") && PlayerPrefs.GetInt($"Level {i}") == 1)
            {
                button.GetComponent<Button>().interactable = true;
                button.GetComponent<Animator>().runtimeAnimatorController = beatenLevelAnim;
                beatenConnectorsPageTwo.transform.GetChild((i - 1)%8).gameObject.SetActive(true);
            }
            else
            {
                button.GetComponent<Button>().interactable = false;
                button.transform.GetChild(0).gameObject.SetActive(true);
                button.GetComponent<Animator>().runtimeAnimatorController = lockedLevelAnim;
            }
            if (!currentPicked && (!PlayerPrefs.HasKey($"Level {i + 1}") || PlayerPrefs.GetInt($"Level {i + 1}") != 1))
            {
                currentPicked = true;
                button.GetComponent<Animator>().runtimeAnimatorController = currentLevelAnim;
                beatenConnectorsPageTwo.transform.GetChild((i - 1)%8).gameObject.SetActive(false);
                currentLevelCapacitor.transform.position = button.transform.position;
                currentLevel = i - 1;
                currentLevelPage = 1;
            }
            i++;
        }
        buttons = page1Buttons;
        currentLevelCapacitor.SetActive(currentLevelPage == 0);
        GameObject savedVariablesObject = GameObject.FindGameObjectWithTag("MultiSceneVariables");
        SetButtonSize(currentSelection);
        page1Parent.gameObject.SetActive(true);
        page2Parent.gameObject.SetActive(false);
        previousButton.SetActive(false);
        if (!PlayerPrefs.HasKey($"Level {buttons.Count + 1}") || PlayerPrefs.GetInt($"Level {buttons.Count + 1}") != 1)
        {
            nextButton.SetActive(false);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        float change = context.ReadValue<Vector2>().x;
        if(Time.realtimeSinceStartup > readyToChange)
        {
            if(change > .25 && (currentPage * page1Buttons.Count) + currentSelection < currentLevel)
            {
                currentSelection += 1;
                if(currentSelection >= buttons.Count)
                {
                    if (currentPage == 0)
                    {
                        currentPage = 1;
                        buttons = page2Buttons;
                        //page1Parent.gameObject.SetActive(false);
                        //page2Parent.gameObject.SetActive(true);
                        setPageTwoActive();
                        UpdateNavButtons();
                    }
                    currentSelection = 0;
                }
            }
            else if (change < -.25)
            {
                if (currentSelection < 1 && (currentPage == 1 || (buttons.Count - 1) < currentLevel))
                {
                    if (currentPage == 1)
                    {
                        currentPage = 0;
                        buttons = page1Buttons;
                        //page1Parent.gameObject.SetActive(true);
                        //page2Parent.gameObject.SetActive(false);
                        setPageOneActive();
                        UpdateNavButtons();
                    }
                    currentSelection = buttons.Count - 1;
                }
                else if (currentSelection > 0)
                {
                    currentSelection -= 1;                
                }
            }
            currentLevelCapacitor.SetActive(currentLevelPage == currentPage);
            readyToChange = Time.realtimeSinceStartup + delay;
            SetButtonSize(currentSelection);
        }
    }

    public void SetButtonSize(int currentSelection)
    {
        foreach (GameObject button in buttons) 
        {
            button.GetComponent<RectTransform>().localScale = Vector3.one;
        }
        buttons[currentSelection].GetComponent<RectTransform>().localScale = new Vector3(1.125f, 1.125f, 1.125f);
    }

    public void Select(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            buttons[currentSelection].GetComponent<Button>().onClick.Invoke();
        }
    }

    public void HoverButton(int hover)
    {
        if (PlayerPrefs.HasKey($"Level {hover + (currentPage * page1Buttons.Count) + 1}") && PlayerPrefs.GetInt($"Level {hover + (currentPage * page1Buttons.Count) + 1}") == 1)
        {
            currentSelection = hover;
            readyToChange = Time.realtimeSinceStartup + delay;
            SetButtonSize(currentSelection);
        }
    }

    public void NextPage()
    {
        if (currentPage == 1 || !PlayerPrefs.HasKey($"Level {buttons.Count + 1}") || PlayerPrefs.GetInt($"Level {buttons.Count + 1}") != 1)
        {
            return;
        }

        currentPage = 1;
        buttons = page2Buttons;
        setPageTwoActive();
        currentSelection = 0;
        currentLevelCapacitor.SetActive(currentLevelPage == currentPage);
        readyToChange = Time.realtimeSinceStartup + delay;
        SetButtonSize(currentSelection);
    }

    public void PreviousPage()
    {
        if (currentPage == 0)
        {
            return;
        }

        currentPage = 0;
        buttons = page1Buttons;
        setPageOneActive();
        currentSelection = 0;
        currentLevelCapacitor.SetActive(currentLevelPage == currentPage);
        readyToChange = Time.realtimeSinceStartup + delay;
        SetButtonSize(currentSelection);
    }
    public void setPageOneActive()
    {
        nextButton.SetActive(true);
        previousButton.SetActive(false);
        page1Parent.gameObject.SetActive(true);
        page2Parent.gameObject.SetActive(false);
        beatenConnectorsPageOne.SetActive(true);
        beatenConnectorsPageTwo.SetActive(false);
    }
    public void setPageTwoActive()
    {
        nextButton.SetActive(false);
        previousButton.SetActive(true);
        page1Parent.gameObject.SetActive(false);
        page2Parent.gameObject.SetActive(true);
        beatenConnectorsPageOne.SetActive(false);
        beatenConnectorsPageTwo.SetActive(true);
    }

    public void UpdateNavButtons()
    {
        if (currentPage == 1)
        {
            nextButton.SetActive(false);
            previousButton.SetActive(true);
        }
        else
        {
            if (PlayerPrefs.HasKey($"Level {buttons.Count + 1}") && PlayerPrefs.GetInt($"Level {buttons.Count + 1}") == 1)
            {
                nextButton.SetActive(true);
            }
            previousButton.SetActive(false);
        }
    }

    public void UnlockAllLevels()
    {
        PlayerPrefs.SetInt("Level 1", 1);
        PlayerPrefs.SetInt("Level 2", 1);
        PlayerPrefs.SetInt("Level 3", 1);
        PlayerPrefs.SetInt("Level 4", 1);
        PlayerPrefs.SetInt("Level 5", 1);
        PlayerPrefs.SetInt("Level 6", 1);
        PlayerPrefs.SetInt("Level 7", 1);
        PlayerPrefs.SetInt("Level 8", 1);
        PlayerPrefs.SetInt("Level 9", 1);
        PlayerPrefs.SetInt("Level 10", 1);
        PlayerPrefs.SetInt("Level 11", 1);
        PlayerPrefs.SetInt("Level 12", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LockAllLevels()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
