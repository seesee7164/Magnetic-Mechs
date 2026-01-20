using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControlScreenFade : MonoBehaviour
{
    [Header("Components")]
    //private Image blackScreen;
    private CanvasGroup blackScreen;
    public GameObject clearTimeText;
    private MultiSceneVariables multiSceneVariables;
    [Header("Variables")]
    private float currTarget;
    private float currAlpha = 1;
    private float changePerStep = .05f;
    private bool currentlyChanging = false;
    private void Awake()
    {
        multiSceneVariables = GameObject.FindGameObjectWithTag("MultiSceneVariables").GetComponent<MultiSceneVariables>();
        blackScreen = GetComponent<CanvasGroup>();
        currTarget = 1f;
        setAlphaValue(1f);
        startFadeOut();
    }
    public void setAlphaValue(float alpha)
    {
        blackScreen.alpha = alpha;
    }
    public void startFadeIn(float fadeDuration = 1.0f, float delay = 0.25f)
    {
        StartCoroutine(fadeIn(fadeDuration, delay));
    }
    private IEnumerator fadeIn(float fadeDuration = 1.0f, float delay = 0.25f, float target = 1f)
    {
        while (currentlyChanging)
        {
            yield return new WaitForSeconds(delay);
        }
        currentlyChanging = true;
        yield return new WaitForSeconds(delay);
        currTarget = target;
        float timeBetweenSteps = changePerStep * fadeDuration;
        while (currAlpha < currTarget)
        {
            currAlpha += changePerStep;
            setAlphaValue(currAlpha);
            yield return new WaitForSeconds(timeBetweenSteps);
        }
        HandleClearTimeText();
        currentlyChanging = false;
    }
    public void startFadeOut()
    {
        StartCoroutine(fadeOut());
    }
    private IEnumerator fadeOut(float fadeDuration = 1.0f, float delay = 0.5f, float target = 0f)
    {
        while (currentlyChanging)
        {
            yield return new WaitForSeconds(delay);
        }
        currentlyChanging = true;
        yield return new WaitForSeconds(delay);
        currTarget = target;
        float timeBetweenSteps = changePerStep * fadeDuration;
        while (currAlpha > currTarget)
        {
            currAlpha -= changePerStep;
            setAlphaValue(currAlpha);
            yield return new WaitForSeconds(timeBetweenSteps);
        }
        currentlyChanging = false;
    }
    private void HandleClearTimeText()
    {
        clearTimeText.SetActive(true);
        clearTimeText.GetComponent<Text>().text = "Clear Time: " + multiSceneVariables.returnCurrentTimeAsString();
    }
}
