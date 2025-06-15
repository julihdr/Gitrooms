using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class unloadIntro : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string mainMenuSceneName = "00_MainMenu";

    private void Start()
    {
        // Subscribe to the VideoPlayer's event for when the video has ended
        videoPlayer.loopPointReached += OnVideoEnded;
    }

    private void OnDestroy()
    {
        // Make sure to unsubscribe from the event when this object is destroyed
        videoPlayer.loopPointReached -= OnVideoEnded;
    }

    private void OnVideoEnded(VideoPlayer vp)
    {
        // Check if the VideoPlayer that triggered the event is the same as the one we're listening to
        if (vp == videoPlayer)
        {
            // Load the main menu scene
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
