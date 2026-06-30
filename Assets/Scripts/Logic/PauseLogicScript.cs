using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PauseLogicScript : MonoBehaviour
{
    //holds all of the functions used for the pause screen
    public void TryAgain()
    {
        LogicScript.logicSingleton.TryAgain();
    }
    public void Pause()
    {
        LogicScript.logicSingleton.SetPausePressed();
    }
    public void StartLevelSelect()
    {
        LogicScript.logicSingleton.StartLevelSelect();
    }
    public void ShowSettingsMenu()
    {
        Debug.Log("PauseLogicScript:ShowSettingsMenu");
        LogicScript.logicSingleton.ShowSettingsMenu();
    }
    public void ShowPauseMenu()
    {
        LogicScript.logicSingleton.ShowPauseMenu();
    }
    public void HideMenus() {
        LogicScript.logicSingleton.HideMenus();
    }
    public void Quit()
    {
        LogicScript.logicSingleton.Quit();
    }
}
