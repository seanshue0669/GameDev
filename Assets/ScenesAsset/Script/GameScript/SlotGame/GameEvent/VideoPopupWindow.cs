using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class VideoPopupWindow : MonoBehaviour
{
    private static VideoPopupWindow _instance;
    public static VideoPopupWindow Instance => _instance ??= new VideoPopupWindow();

    [Header("UI Elements")]
    public GameObject popupPanel;

    public VideoClip[] Clips;
    public VideoPlayer videoPlayer;
    private int videoIndex;
    private void Awake() => videoPlayer = GetComponent<VideoPlayer>();

    private bool isPlaying = false;

    public void Init()
    {
        popupPanel.SetActive(false);
        EventSystem.Instance.RegisterEvent<int>("PlayVideo", "PlayWindow", VideoPlay);
    }
    
    private void VideoPlay(int videoClip)
    {
        StartCoroutine(VideoPlayCoroutine(videoClip));
    }

    private IEnumerator VideoPlayCoroutine(int videoClip)
    {
        if (popupPanel.activeSelf)
        {
            videoPlayer.Stop();
            isPlaying = false;
            popupPanel.SetActive(false);
        }
        else
        {
            popupPanel.SetActive(true);

            switch (videoClip)
            {
                case 1:
                    videoPlayer.clip = Clips[0];
                    break;
                case 2:
                    videoPlayer.clip = Clips[1];
                    break;
            }

            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
            {
                Debug.Log("Preparing video...");
                yield return null;
            }

            videoPlayer.Play();
            isPlaying = true;

            while (isPlaying && videoPlayer.isPlaying)
            {
                yield return null;
            }

            videoPlayer.Stop();
            popupPanel.SetActive(false);
        }
    }
}