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
    public Transform page3Parent;
    public GameObject currentLevelCapacitor;
    public GameObject previousButton;
    public GameObject nextButton;
    public GameObject beatenConnectors;
    private GameObject beatenConnectorsPageOne;
    private GameObject beatenConnectorsPageTwo;
    private GameObject beatenConnectorsPageThree;
    [Header("Variables")]
    public List<GameObject> buttons;
    public List<GameObject> page1Buttons;
    public List<GameObject> page2Buttons;
    public List<GameObject> page3Buttons;
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
        beatenConnectorsPageThree = beatenConnectors.transform.GetChild(2).gameObject;
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

        foreach (Transform child in page3Parent)
        {
            if (child.gameObject.GetComponent<Button>() == null) continue;
            GameObject button = child.gameObject;
            page3Buttons.Add(button);
            if (PlayerPrefs.HasKey($"Level {i}") && PlayerPrefs.GetInt($"Level {i}") == 1)
            {
                button.GetComponent<Button>().interactable = true;
                button.GetComponent<Animator>().runtimeAnimatorController = beatenLevelAnim;
                beatenConnectorsPageThree.transform.GetChild((i - 1) % 8).gameObject.SetActive(true);
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
                beatenConnectorsPageThree.transform.GetChild((i - 1) % 8).gameObject.SetActive(false);
                currentLevelCapacitor.transform.position = button.transform.position;
                currentLevel = i - 1;
                currentLevelPage = 2;
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
                    if (currentPage != 2)
                    {
                        currentPage += 1;
                        //buttons = page2Buttons;
                        //page1Parent.gameObject.SetActive(false);
                        //page2Parent.gameObject.SetActive(true);
                        setCorrectPageActive();
                        //UpdateNavButtons();
                    }
                    currentSelection = 0;
                }
            }
            else if (change < -.25)
            {
                if (currentSelection < 1 && (currentPage != 0 || (buttons.Count - 1) < currentLevel))
                {
                    if (currentPage != 0)
                    {
                        currentPage -= 1;
                        //buttons = page1Buttons;
                        //page1Parent.gameObject.SetActive(true);
                        //page2Parent.gameObject.SetActive(false);
                        setCorrectPageActive();
                        //UpdateNavButtons();
                        currentSelection = buttons.Count - 1;
                    }
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
        if (currentPage == 2 || !PlayerPrefs.HasKey($"Level {buttons.Count + 1}") || PlayerPrefs.GetInt($"Level {buttons.Count + 1}") != 1)
        {
            return;
        }

        currentPage += 1;
        setCorrectPageActive();
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

        currentPage -= 1;
        setCorrectPageActive();
        currentSelection = 0;
        currentLevelCapacitor.SetActive(currentLevelPage == currentPage);
        readyToChange = Time.realtimeSinceStartup + delay;
        SetButtonSize(currentSelection);
    }
    public void setCorrectPageActive()
    {
        if (currentPage == 0) setPageOneActive();
        else if(currentPage == 1) setPageTwoActive();
        else if(currentPage == 2) setPageThreeActive();
    }
    public void setPageOneActive()
    {
        buttons = page1Buttons;
        //nextButton.SetActive(true);
        //previousButton.SetActive(false);
        UpdateNavButtons();
        page1Parent.gameObject.SetActive(true);
        page2Parent.gameObject.SetActive(false);
        page3Parent.gameObject.SetActive(false);
        beatenConnectorsPageOne.SetActive(true);
        beatenConnectorsPageTwo.SetActive(false);
        beatenConnectorsPageThree.SetActive(false);
    }
    public void setPageTwoActive()
    {
        buttons = page2Buttons;
        //nextButton.SetActive(true);
        //previousButton.SetActive(true);
        UpdateNavButtons();
        page1Parent.gameObject.SetActive(false);
        page2Parent.gameObject.SetActive(true);
        page3Parent.gameObject.SetActive(false);
        beatenConnectorsPageOne.SetActive(false);
        beatenConnectorsPageTwo.SetActive(true);
        beatenConnectorsPageThree.SetActive(false);
    }
    public void setPageThreeActive()
    {
        buttons = page3Buttons;
        //nextButton.SetActive(false);
        //previousButton.SetActive(true);
        UpdateNavButtons();
        page1Parent.gameObject.SetActive(false);
        page2Parent.gameObject.SetActive(false);
        page3Parent.gameObject.SetActive(true);
        beatenConnectorsPageOne.SetActive(false);
        beatenConnectorsPageTwo.SetActive(false);
        beatenConnectorsPageThree.SetActive(true);
    }

    public void UpdateNavButtons()
    {
        if (currentPage == 2)
        {
            nextButton.SetActive(false);
            previousButton.SetActive(true);
        }
        else if (currentPage == 1)
        {
            if (PlayerPrefs.HasKey($"Level {buttons.Count * 2 + 1}") && PlayerPrefs.GetInt($"Level {buttons.Count * 2 + 1}") == 1)
            {
                nextButton.SetActive(true);
            }
            else nextButton.SetActive(false);
            previousButton.SetActive(true);
        }
        else
        {
            if (PlayerPrefs.HasKey($"Level {buttons.Count + 1}") && PlayerPrefs.GetInt($"Level {buttons.Count + 1}") == 1)
            {
                nextButton.SetActive(true);
            }
            else nextButton.SetActive(false);
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
        PlayerPrefs.SetInt("Level 13", 1);
        PlayerPrefs.SetInt("Level 14", 1);
        PlayerPrefs.SetInt("Level 15", 1);
        PlayerPrefs.SetInt("Level 16", 1);
        PlayerPrefs.SetInt("Level 17", 1);
        //PlayerPrefs.SetInt("Level 18", 1);
        //PlayerPrefs.SetInt("Level 19", 1);
        //PlayerPrefs.SetInt("Level 20", 1);
        //PlayerPrefs.SetInt("Level 21", 1);
        //PlayerPrefs.SetInt("Level 22", 1);
        //PlayerPrefs.SetInt("Level 23", 1);
        //PlayerPrefs.SetInt("Level 24", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LockAllLevels()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
