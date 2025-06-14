using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Canvas canvasToUnload;
    private bool canvasUnloaded = false;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;

        if (PlayerPrefs.HasKey("CanvasUnloaded"))
        {
            canvasUnloaded = PlayerPrefs.GetInt("CanvasUnloaded") == 1;
            if (canvasUnloaded)
            {
                canvasToUnload.gameObject.SetActive(false);
            }
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (vp == videoPlayer && !canvasUnloaded)
        {
            // Unload the canvas
            canvasToUnload.gameObject.SetActive(false);
            canvasUnloaded = true;
            PlayerPrefs.SetInt("CanvasUnloaded", 1);
        }
    }
}
