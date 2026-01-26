using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class TurnClearTimesOnOrOffScript : MonoBehaviour
{
    [Header("Components")]
    //public Text currentDifficultyText;
    public DifficultyScript difficultyScript;
    public Text myText;
    [Header("Scripts")]
    private MultiSceneVariables multiSceneVariables;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        multiSceneVariables = GameObject.FindGameObjectWithTag("MultiSceneVariables").GetComponent<MultiSceneVariables>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Change()
    {
        if (multiSceneVariables.ShowTime())
        {
            Debug.Log("test1");
            myText.text = "Enable Clear Times";
            multiSceneVariables.StopShowing();
        }
        else
        {
            Debug.Log("test2");
            myText.text = "Disable Clear Times";
            multiSceneVariables.StartShowing();
        }
        difficultyScript.updateClearTimes();
    }
}
