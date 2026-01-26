using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartScreenLogic : MonoBehaviour
{
    public enum MenuState
    {
        StartScreen,
        LevelSelect,
        Settings
    }

    //holds the logic for functions which are called into during the starting screen
    public GameObject startScreenStart;
    public GameObject startScreenLevelSelect;
    public GameObject startScreenSettings;
    //public GameObject backgroundCanvas;
    public MultiSceneVariables variableStorage;
    public PlayerInput myInput;
    public PlayerInput selectionInput;
    public MainMenuButtonSelectionManager mainMenuButtonSelectionManager;
    public StartMenuButtonSelectionManager startMenuButtonSelectionManager;
    public SettingsButtonSelectionManager settingsButtonSelectionManager;
    public MenuState currScreen = MenuState.StartScreen;// 0 = start screen, 1 = level select, 2 = settings

    private void Awake()
    {
        myInput = GetComponent<PlayerInput>();
        myInput.SwitchCurrentActionMap("UI");
        variableStorage = GameObject.FindGameObjectWithTag("MultiSceneVariables").GetComponent<MultiSceneVariables>();
        variableStorage.resetFullGamePlaythrough();
    }

    public void returnToStartScreen()
    {
        currScreen = MenuState.StartScreen;
        startMenuButtonSelectionManager.startEnabling();
        startScreenStart.SetActive(true);
        startScreenLevelSelect.SetActive(false);
        startScreenSettings.SetActive(false);
        //backgroundCanvas.SetActive(true);
    }

    public void StartGame()
    {
        currScreen = MenuState.LevelSelect;
        startMenuButtonSelectionManager.stopEnabling();
        startScreenStart.SetActive(false);
        startScreenLevelSelect.SetActive(true);
        //backgroundCanvas.SetActive(false);
    }

    public void GoToSettings()
    {
        currScreen = MenuState.Settings;
        startMenuButtonSelectionManager.stopEnabling();
        startScreenStart.SetActive(false);
        startScreenSettings.SetActive(true);
        //backgroundCanvas.SetActive(false);
    }

    public void StartStage(string level)
    {
        variableStorage.fullyRestartLevel();
        SceneManager.LoadScene(level);
    }
    public void Quit()
    {
        Application.Quit();
    }
    //public void GamePadPressed(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        variableStorage.gamePadNotMouse = true;
    //        myInput.SwitchCurrentActionMap("UI");
    //        StartGame();
    //    }
    //}
    //public void StartGame(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        variableStorage.gamePadNotMouse = false;
    //        myInput.SwitchCurrentActionMap("UI");
    //        StartGame();
    //    }
    //}
    public void Move(InputAction.CallbackContext context)
    {
        switch (currScreen)
        {
            case MenuState.StartScreen:
                startMenuButtonSelectionManager.Move(context);
                break;
            case MenuState.LevelSelect:
                mainMenuButtonSelectionManager.Move(context);
                break;
            case MenuState.Settings:
                settingsButtonSelectionManager.Move(context);
                break;
            default:
                break;
        }
        //if (currScreen == 0)
        //{
        //    startMenuButtonSelectionManager.Move(context);
        //}
        //else if (currScreen == 1)
        //{
        //    mainMenuButtonSelectionManager.Move(context);
        //}
        //else
        //{
        //    settingsButtonSelectionManager.Move(context);
        //}
    }
    public void Select(InputAction.CallbackContext context)
    {
        switch (currScreen)
        {
            case MenuState.StartScreen:
                startMenuButtonSelectionManager.Select(context);
                break;
            case MenuState.LevelSelect:
                mainMenuButtonSelectionManager.Select(context);
                break;
            case MenuState.Settings:
                settingsButtonSelectionManager.Select(context);
                break;
            default:
                break;
        }
        //if (currScreen == 0)
        //{
        //    startMenuButtonSelectionManager.Select(context);
        //}
        //else if (currScreen == 1)
        //{
        //    mainMenuButtonSelectionManager.Select(context);
        //}
        //else
        //{
        //    settingsButtonSelectionManager.Select(context);
        //}
    }
    public void Escape(InputAction.CallbackContext context)
    {
        switch (currScreen)
        {
            case MenuState.LevelSelect:
            case MenuState.Settings:
                returnToStartScreen();
                break;
            default:
                break;
        }
        //if (currScreen == 1 || currScreen == 2)
        //{
        //    returnToStartScreen();
        //}
    }
    public void StartWithLevelOne()
    {
        variableStorage.StartWithLevelOne();
    }
}
