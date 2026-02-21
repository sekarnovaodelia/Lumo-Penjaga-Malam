using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class VideoLoader : MonoBehaviour
{
    public VideoPlayer video;
    public GameObject fallback;
    public float fallbackDelay = 1.5f; // lamanya fallback tampil

    void Start()
    {
        video.Prepare();
        video.prepareCompleted += OnPrepared;
    }

    void OnPrepared(VideoPlayer vp)
    {
        vp.Play();
        StartCoroutine(HideFallback());
    }

    IEnumerator HideFallback()
    {
        yield return new WaitForSeconds(fallbackDelay);
        if (fallback != null)
            fallback.SetActive(false);
    }
}