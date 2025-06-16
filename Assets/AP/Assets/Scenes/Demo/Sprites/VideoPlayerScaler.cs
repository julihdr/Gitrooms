using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerScaler : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;
    private RectTransform rawImageRectTransform;

    void Start()
    {
        rawImageRectTransform = rawImage.GetComponent<RectTransform>();
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        AdjustVideoScale();
    }

    void AdjustVideoScale()
    {
        int videoWidth = (int)videoPlayer.texture.width;
        int videoHeight = (int)videoPlayer.texture.height;
        float videoAspect = (float)videoWidth / (float)videoHeight;

        float screenAspect = (float)Screen.width / (float)Screen.height;

        if (videoAspect > screenAspect)
        {
            // Video is wider than the screen (relative to height)
            float scaleHeight = screenAspect / videoAspect;
            rawImageRectTransform.sizeDelta = new Vector2(Screen.width, Screen.height * scaleHeight);
        }
        else
        {
            // Video is taller than the screen (relative to width)
            float scaleWidth = videoAspect / screenAspect;
            rawImageRectTransform.sizeDelta = new Vector2(Screen.width * scaleWidth, Screen.height);
        }

        rawImage.texture = videoPlayer.texture;
    }
}
