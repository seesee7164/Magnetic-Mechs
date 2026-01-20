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
    public float fullGameTime = 0f;
    public bool levelComplete = false;
    public bool gameComplete = false;
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
        if (levelComplete) return;
        currentLevelTime += Time.deltaTime;
        if (!startedFromLevelOne || gameComplete) return;
        fullGameTime += Time.deltaTime;
    }
    public void resetTimer()
    {
        currentLevelTime = 0;
        currentLevelTimePreCheckPoint = 0;
        levelComplete = false;
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
    public string returnCurrentTimeAsString()
    {
        if (!showTimer) return "";
        int numMinutes = Mathf.FloorToInt(currentLevelTime / 60);
        int numSeconds = (int)Mathf.Floor(currentLevelTime % 60);
        return numMinutes + (numSeconds >= 10 ? ":" : ":0") + numSeconds;
    }
    public string returnFullGameTimeAsString()
    {
        if (!showTimer) return "";
        int numMinutes = Mathf.FloorToInt(fullGameTime / 60);
        int numSeconds = (int)Mathf.Floor(fullGameTime % 60);
        return numMinutes + (numSeconds >= 10 ? ":" : ":0") + numSeconds;
    }
    public bool ShowTime()
    {
        return showTimer;
    }
    public void finishLevel(int level)
    {
        levelComplete = true;
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
    }
    public void FinishGame()
    {
        gameComplete = true;
    }
}
