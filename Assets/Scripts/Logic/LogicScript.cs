using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class LogicScript : MonoBehaviour
{
    public enum GameMenuState {
        PLAYING,
        PAUSE_MENU,
        SETTINGS_MENU
    }

    //A singleton intended to hold functions that are used regularly by other scripts
    [Header("Components")]
    public GameObject gameOverScreen;
    public GameObject pauseScreen;
    private MultiSceneVariables multiSceneVariables;

    [Header("Variables")]
    private float delayForEndScreen = 1f;

    //public Text remainingFuelText;
    public GameObject settingsScreen;
    public PlayerInput playerInput;
    public ButtonSelectionManager buttonSelectionManager;
    public ControlScreenFade controlScreenFade;

    [Header("Singleton")]
    public static LogicScript logicSingleton;

    private GameMenuState menuState;
    private bool pausePressed;

    private void Awake()
    {
        if (logicSingleton == null)
        {
            logicSingleton = this;
        }
        multiSceneVariables = GameObject.FindGameObjectWithTag("MultiSceneVariables").GetComponent<MultiSceneVariables>();
    }
    private void Start()
    {
        buttonSelectionManager.SetGameMenuState(GameMenuState.PLAYING);
    }
    private void Update() {
        if (pausePressed) {
            Pause();
            pausePressed = false;
        }
    }
    public void TryAgain()
    {
        menuState = GameMenuState.PLAYING;
        Time.timeScale = 1.0f;
        //playerInput.SwitchCurrentActionMap("Player");
        multiSceneVariables.resetTimer();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartLevel(string level)
    {
        multiSceneVariables.fullyRestartLevel();
        PlayerPrefs.SetInt(level, 1);
        SceneManager.LoadScene(level);
    }
    public void StartLevelSelect()
    {
        menuState = GameMenuState.PLAYING;
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Main Menu");
    }
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
    }
    public void SetPausePressed() {
        pausePressed = true;
    }
    public void Pause()
    {
        switch (menuState) {
            case GameMenuState.PLAYING:
                ShowPauseMenu();
                break;
            case GameMenuState.PAUSE_MENU:
                HideMenus();
                break;
            case GameMenuState.SETTINGS_MENU:
                ShowPauseMenu();
                break;
            default:
                break;
        }
    }
    public void ShowSettingsMenu()
    {
        // Pause game
        Time.timeScale = 0.0f;
        //playerInput.SwitchCurrentActionMap("UI");

        // Show settings menu
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(true);
        
        // Update button selection visual
        menuState = GameMenuState.SETTINGS_MENU;
        buttonSelectionManager.SetGameMenuState(menuState);
    }
    public void ShowPauseMenu()
    {
        // Pause game
        Time.timeScale = 0.0f;
        playerInput.SwitchCurrentActionMap("UI");

        // Show pause menu
        pauseScreen.SetActive(true);
        settingsScreen.SetActive(false);

        // Update button selection visual
        menuState = GameMenuState.PAUSE_MENU;
        buttonSelectionManager.SetGameMenuState(menuState);
    }
    public void HideMenus()
    {
        // Unpause game
        Time.timeScale = 1.0f;
        playerInput.SwitchCurrentActionMap("Player");

        // Hide menus
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(false);
        
        menuState = GameMenuState.PLAYING;
    }
    /*
    public void changeBind(GameObject button)
    {

    }
    */
    public void Quit()
    {
        Application.Quit();
    }
    public bool IsPaused
    {
        get { return menuState != GameMenuState.PLAYING; }
    }
    public void StartPostSpiderBossDelay()
    {
        multiSceneVariables.FinishLevel(7);
        StartScreenFade(1.5f, 1.5f);
        float timeUntilLevelEnd = 3.25f;
        if (multiSceneVariables.ShowTime()) timeUntilLevelEnd += delayForEndScreen;
        StartCoroutine(StartPostSpiderBoss(timeUntilLevelEnd));
    }
    public IEnumerator StartPostSpiderBoss(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartLevel("Level 8");
    }
    public void StartPostBeeBossDelay()
    {
        multiSceneVariables.FinishLevel(12);
        StartScreenFade(1.5f, 1.5f);
        float timeUntilLevelEnd = 3.25f;
        if (multiSceneVariables.ShowTime()) timeUntilLevelEnd += delayForEndScreen;
        StartCoroutine(StartPostBeeBoss(timeUntilLevelEnd));
    }
    public IEnumerator StartPostBeeBoss(float delay)
    {
        yield return new WaitForSeconds(delay);
        multiSceneVariables.FinishGame();
        StartLevel("Ending");
    }
    public void StartScreenFade(float duration = 1.0f, float delay = .25f)
    {
        controlScreenFade.startFadeIn(duration, delay);
    }
    // public void StartPostBeeBossDelay()
    // {
    //     StartCoroutine(StartPostBeeBoss(3f));
    // }
    // public IEnumerator StartPostBeeBoss(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     StartLevelNine();
    // }
    public float ReturnDelayForEndScreen()
    {
        return delayForEndScreen;
    }
}
