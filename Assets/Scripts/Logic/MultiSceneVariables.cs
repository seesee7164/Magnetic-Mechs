using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSceneVariables : MonoBehaviour
{
    //holds variables which are meant to persist across multiple scenes
    [Header("Multi Scene Variables")]
    public bool gamePadNotMouse = false;
    [Header("Singleton")]
    public static MultiSceneVariables multiSceneVariablesInstance;
    [Header("Checkpoint")]
    private int currCheckpoint = 0;
    [Header("Difficulty")]
    public int difficulty = 0;
    [Header("Level Clear Times")]
    private bool showTimer = true;
    public float[] clearTimes = new float[12];
    public bool startedFromLevelOne = false;
    public float currentLevelTime = 0;
    private float currentLevelTimePreCheckPoint = 0;
    private int currentLevelPreviousTime = 0;
    private int currentGamePreviousTime = 0;
    public float fullGameTime = 0f;
    public bool levelComplete = false;
    public bool gameComplete = false;
    private bool playerDead = false;
    private void Awake()
    {
        if (multiSceneVariablesInstance != null && multiSceneVariablesInstance != this)
        {
            Destroy(this); 
        }
        else
        {
            multiSceneVariablesInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        difficulty = PlayerPrefs.GetInt(DifficultyScript.DIFFICULTY_STRING, 0);
        showTimer = (PlayerPrefs.GetInt("TimerEnabled") == 0);
    }
    public void setCheckpoint(int newPoint)
    {
        currCheckpoint = newPoint;
        setCheckPointTimer();
    }
    public void fullyRestartLevel()
    {
        setCheckpoint(0);
        resetTimer();
    }
    public int getCheckpoint()
    {
        return currCheckpoint;
    }




    //Timer Stuff
    private void FixedUpdate()
    {
        if (levelComplete || playerDead) return;
        currentLevelTime += Time.deltaTime;
        if (!startedFromLevelOne || gameComplete) return;
        fullGameTime += Time.deltaTime;
    }
    public void resetTimer()
    {
        currentLevelTime = 0;
        currentLevelTimePreCheckPoint = 0;
        levelComplete = false;
        playerDead = false;
        currentLevelPreviousTime = 0;
    }
    public void playerKilled()
    {
        playerDead = true;
    }
    private void setCheckPointTimer()
    {
        currentLevelTimePreCheckPoint = currentLevelTime;
    }
    public void loadCheckPointTimer()
    {
        currentLevelTime = currentLevelTimePreCheckPoint;
    }
    public float returnCurrentTime()
    {
        return currentLevelTime;
    }
    private string ConvertFloatToString(float time)
    {
        int numMinutes = Mathf.FloorToInt(time / 60);
        int numSeconds = (int)Mathf.Floor(time % 60);
        return numMinutes + (numSeconds >= 10 ? ":" : ":0") + numSeconds;
    }
    public string returnCurrentTimeAsString()
    {
        if (!showTimer) return "";
        return ConvertFloatToString(currentLevelTime);
    }
    public string returnFullGameTimeAsString()
    {
        if (!showTimer) return "";
        return ConvertFloatToString(fullGameTime);
    }

    public string returnPrevTimeAsString()
    {
        if (!showTimer) return "";
        return ConvertFloatToString(currentLevelPreviousTime);
    }
    public string returnPrevGameTimeAsString()
    {
        if (!showTimer) return "";
        return ConvertFloatToString(currentGamePreviousTime);
    }
    public bool ShowTime()
    {
        return showTimer;
    }
    public void StopShowing()
    {
        showTimer = false;
        PlayerPrefs.SetInt("TimerEnabled", 1);
    }
    public void StartShowing()
    {
        showTimer = true;
        PlayerPrefs.SetInt("TimerEnabled", 0);
    }
    public void StartWithLevelOne()
    {
        startedFromLevelOne = true;
    }
    public bool StartedWithLevelOne()
    {
        return startedFromLevelOne;
    }
    public void resetFullGamePlaythrough()
    {
        startedFromLevelOne = false;
        fullGameTime = 0;
        gameComplete = false;
        currentLevelPreviousTime = 0;
        currentGamePreviousTime = 0;
    }


    //Saving times between sessions
    public static readonly string[] NormalLevelBestTimes =
{
        "FullGameNormal",
        "LevelOneNormal",
        "LevelTwoNormal",
        "LevelThreeNormal",
        "LevelFourNormal",
        "LevelFiveNormal",
        "LevelSixNormal",
        "LevelSevenNormal",
        "LevelEightNormal",
        "LevelNineNormal",
        "LevelTenNormal",
        "LevelElevenNormal",
        "LevelTwelveNormal"
    };
    public static readonly string[] HardLevelBestTimes =
    {
        "FullGameHard",
        "LevelOneHard",
        "LevelTwoHard",
        "LevelThreeHard",
        "LevelFourHard",
        "LevelFiveHard",
        "LevelSixHard",
        "LevelSevenHard",
        "LevelEightHard",
        "LevelNineHard",
        "LevelTenHard",
        "LevelElevenHard",
        "LevelTwelveHard"
    };
    public static readonly string[] ImpossibleLevelBestTimes =
    {
        "FullGameImpoossible",
        "LevelOneImpossible",
        "LevelTwoImpossible",
        "LevelThreeImpossible",
        "LevelFourImpossible",
        "LevelFiveImpossible",
        "LevelSixImpossible",
        "LevelSevenImpossible",
        "LevelEightImpossible",
        "LevelNineImpossible",
        "LevelTenImpossible",
        "LevelElevenImpossible",
        "LevelTwelveImpossible"
    };

    public void SaveCurrentLevelTime(int level)
    {
        if (difficulty == 0) SetLevelTime(NormalLevelBestTimes[level], (int)Mathf.Floor(currentLevelTime));
        else if (difficulty == 1) SetLevelTime(HardLevelBestTimes[level], (int)Mathf.Floor(currentLevelTime));
        else if (difficulty == 2) SetLevelTime(ImpossibleLevelBestTimes[level], (int)Mathf.Floor(currentLevelTime));
    }
    public void SaveFullGameTime()
    {
        if (difficulty == 0) SetGameTime(NormalLevelBestTimes[0], (int)Mathf.Floor(fullGameTime));
        else if (difficulty == 1) SetGameTime(HardLevelBestTimes[0], (int)Mathf.Floor(fullGameTime));
        else if (difficulty == 2) SetGameTime(ImpossibleLevelBestTimes[0], (int)Mathf.Floor(fullGameTime));
    }
    private void SetLevelTime(string currentLevelString, int TimeToSet)
    {
        int prevTime = PlayerPrefs.GetInt(currentLevelString);
        if (TimeToSet < prevTime || prevTime == 0)
        {
            currentLevelPreviousTime = prevTime;
            PlayerPrefs.SetInt(currentLevelString, TimeToSet);
        }
    }

    private void SetGameTime(string currentLevelString, int TimeToSet)
    {
        int prevTime = PlayerPrefs.GetInt(currentLevelString);
        if (TimeToSet < prevTime || prevTime == 0)
        {
            currentGamePreviousTime = prevTime;
            PlayerPrefs.SetInt(currentLevelString, TimeToSet);
        }
    }
    public int returnPreviousTime()
    {
        return currentLevelPreviousTime;
    }
    public int returnPreviousGameTime()
    {
        return currentGamePreviousTime;
    }
    public string GetSavedLevelTime(int level)
    {
        if (!showTimer) return "";

        int normalTime = PlayerPrefs.GetInt(NormalLevelBestTimes[level]);
        int hardTime = PlayerPrefs.GetInt(HardLevelBestTimes[level]);
        int impossibleTime = PlayerPrefs.GetInt(ImpossibleLevelBestTimes[level]);
        if (normalTime == 0 && hardTime == 0 && impossibleTime == 0) return "";

        int currTime = 0;
        if (difficulty == 0) currTime = normalTime;
        else if (difficulty == 1)  currTime = hardTime;
        else if (difficulty == 2) currTime = impossibleTime;

        if (currTime == 0) return "-:-";
        return ConvertFloatToString(currTime);
    }
    public void FinishLevel(int level)
    {
        SaveCurrentLevelTime(level);
        levelComplete = true;
    }
    public void FinishGame()
    {
        SaveFullGameTime();
        gameComplete = true;
    }
}
