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
    public float[] clearTimes = new float[12];
    public bool startedFromLevelOne = false;
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
    }
    public int getCheckpoint()
    {
        return currCheckpoint;
    }
}
