using System.Collections;
using UnityEngine;

public class IntroSceneScreenFade : MonoBehaviour
{
    [Header("Components")]
    private CanvasGroup blackScreen;
    [Header("Variables")]
    private float currTarget;
    private float currAlpha = 0;
    private float changePerStep = .05f;
    private bool currentlyChanging = false;
    private void Awake()
    {
        blackScreen = GetComponent<CanvasGroup>();
        currTarget = 1f;
        currAlpha = blackScreen.alpha;
    }
    public void setAlphaValue(float alpha)
    {
        currAlpha = alpha;
        blackScreen.alpha = alpha;
    }
    public void startFadeIn(float fadeDuration = 1.0f, float delay = 0.25f)
    {
        StartCoroutine(fadeIn(fadeDuration, delay, 1f, false));
    }

    private IEnumerator fadeIn(float fadeDuration = 1.0f, float delay = 0.25f, float target = 1f, bool showClearText = true)
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
        currentlyChanging = false;
    }
    public void startFadeOut(float fadeDuration = 1.0f, float delay = 0.5f, float target = 0f)
    {
        StartCoroutine(fadeOut(fadeDuration, delay, target));
    }
    private IEnumerator fadeOut(float fadeDuration, float delay, float target)
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
}
