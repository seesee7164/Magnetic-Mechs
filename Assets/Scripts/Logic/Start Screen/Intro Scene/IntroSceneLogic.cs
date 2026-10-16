using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneLogic : MonoBehaviour
{
    [Header("Components")]
    public IntroSceneScreenFade introSceneScreenFade;
    [Header("Variables")]
    private bool skippable = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadMainMenuAfterDelay());
    }
    IEnumerator LoadMainMenuAfterDelay()
    {
        yield return new WaitForSeconds(.5f);
        skippable = true;
        yield return new WaitForSeconds(10f);
        LoadMainMenu();
    }
    private void LoadMainMenu()
    {
        introSceneScreenFade.startFadeIn(1, 0);
        StartCoroutine(LoadAfterDelay());
    }
    IEnumerator LoadAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Main Menu");
    }
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    public void LoadMainMenuEarly()
    {
        if (!skippable) return;
        else LoadMainMenu();
    }
}
