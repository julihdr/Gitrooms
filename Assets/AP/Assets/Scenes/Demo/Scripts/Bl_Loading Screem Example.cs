using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class bl_LoadingScreenExample : MonoBehaviour
{
    public string MainMenuSceneName = "MainMenu";
    public GameObject CanvasToUnload;
    public GameObject videoPlayertoload; // Reference to the Main Menu Canvas
    public VideoPlayer videoPlayer;
    public float fadeDuration = 1f;
    public GameObject fadeImage;
    private bool loaded = false;

    private void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Play();
            StartCoroutine(DelayFade());
        }
    }

    private IEnumerator DelayFade()
    {
        float delayTime = 54f; // Original delay time
        while (!loaded)
        {
            delayTime -= Time.deltaTime;
            if (delayTime <= 0f)
            {
                if (CanvasToUnload != null)
                {
                    StartCoroutine(FadeAndUnloadCanvas());
                }
                else
                {
                    StartCoroutine(FadeAndLoadMainMenuScene());
                }
                loaded = true;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                SkipCutscene();
                break;
            }
            yield return null;
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (!loaded)
        {
            if (CanvasToUnload != null)
            {
                StartCoroutine(FadeAndUnloadCanvas());
            }
            else
            {
                StartCoroutine(FadeAndLoadMainMenuScene());
            }
            loaded = true;
        }
    }

    private IEnumerator FadeAndUnloadCanvas()
    {
        Image fadeImageComponent = fadeImage.GetComponent<Image>();
        if (fadeImageComponent == null)
        {
            Debug.LogError("The fadeImage GameObject does not have an Image component attached.");
            yield break;
        }

        // Fade out
        float elapsedTime = 0f;
        Color initialColor = fadeImageComponent.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);

        while (elapsedTime < fadeDuration)
        {
            fadeImageComponent.color = Color.Lerp(initialColor, targetColor, (elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeImageComponent.color = targetColor;

        // Wait for a short delay before destroying the canvas
        float delayTime = 0.5f; // Adjust this value as needed
        yield return new WaitForSeconds(delayTime);

        // Destroy the canvas
        Destroy(CanvasToUnload);

        // Fade back to alpha 0
        elapsedTime = 0f;
        initialColor = fadeImageComponent.color;
        targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f); // Fade to fully transparent

        while (elapsedTime < fadeDuration)
        {
            fadeImageComponent.color = Color.Lerp(initialColor, targetColor, (elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeImageComponent.color = targetColor;

        // Load the Main Menu Canvas
        if (videoPlayertoload != null)
        {
            videoPlayertoload.SetActive(true);
        }
        else
        {
            Debug.LogError("MainMenuCanvas reference is null.");
        }
    }

    private IEnumerator FadeAndLoadMainMenuScene()
    {
        Image fadeImageComponent = fadeImage.GetComponent<Image>();
        if (fadeImageComponent == null)
        {
            Debug.LogError("The fadeImage GameObject does not have an Image component attached.");
            yield break;
        }

        // Fade out
        float elapsedTime = 0f;
        Color initialColor = fadeImageComponent.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f); // Fade to fully transparent

        while (elapsedTime < fadeDuration)
        {
            fadeImageComponent.color = Color.Lerp(initialColor, targetColor, (elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeImageComponent.color = targetColor;

        // Wait for a short delay before loading the scene
        float delayTime = 0.5f; // Adjust this value as needed
        yield return new WaitForSeconds(delayTime);

        SceneManager.LoadScene(MainMenuSceneName);
    }

    private void SkipCutscene()
    {
        if (CanvasToUnload != null)
        {
            StopCoroutine(DelayFade());
            StartCoroutine(FadeAndUnloadCanvas());
        }
        else
        {
            SceneManager.LoadScene(MainMenuSceneName);
        }
    }
}
