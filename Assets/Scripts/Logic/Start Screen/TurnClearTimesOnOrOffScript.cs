using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;


public class TurnClearTimesOnOrOffScript : MonoBehaviour
{
    [Header("Components")]
    //public Text currentDifficultyText;
    public DifficultyScript difficultyScript;
    public TextMeshProUGUI myText;
    [Header("Scripts")]
    private MultiSceneVariables multiSceneVariables;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        multiSceneVariables = GameObject.FindGameObjectWithTag("MultiSceneVariables").GetComponent<MultiSceneVariables>();
        if (multiSceneVariables.ShowTime()) myText.text = "Enable Clear Times";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Change()
    {
        if (multiSceneVariables.ShowTime())
        {
            myText.text = "Enable Clear Times";
            multiSceneVariables.StopShowing();
        }
        else
        {
            myText.text = "Disable Clear Times";
            multiSceneVariables.StartShowing();
        }
        difficultyScript.updateClearTimes();
    }
}
