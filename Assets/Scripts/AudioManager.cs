using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip menuMusic;
    public AudioClip chaseMusic;

    [Header("SFX")]
    public List<AudioSource> audioSourceList = new List<AudioSource>();

    public AudioClip hitSound;
    public AudioClip fallSound;
    public AudioClip pistolSound;
    public AudioClip cannonShootSound;
    public AudioClip passSound;

    float defaultMusicVolume;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            defaultMusicVolume = musicSource.volume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        int music = PlayerPrefs.GetInt("music", 1);
        musicSource.mute = music == 0;
    }

    // ================= MUSIC =================

    public void ToggleMusic(bool state)
    {
        musicSource.mute = !state;
        PlayerPrefs.SetInt("music", state ? 1 : 0);
    }

    public void PlayMenuMusic()
    {
        StartCoroutine(FadeMusic(menuMusic, 1f));
    }

    public void PlayChaseMusic()
    {
        StartCoroutine(FadeMusic(chaseMusic, 1f));
    }

    IEnumerator FadeMusic(AudioClip newClip, float fadeTime)
    {
        if (newClip == null) yield break;
        if (musicSource.clip == newClip) yield break;

        if (!musicSource.isPlaying)
        {
            musicSource.clip = newClip;
            musicSource.loop = true;
            musicSource.Play();
            yield break;
        }

        float t = 0f;
        float startVol = musicSource.volume;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();

        t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, defaultMusicVolume, t / fadeTime);
            yield return null;
        }

        musicSource.volume = defaultMusicVolume;
    }

    // ================= DUCKING =================

    public void DuckMusic(float duration = 0.5f, float duckVolume = 0.3f)
    {
        StartCoroutine(DuckRoutine(duration, duckVolume));
    }

    IEnumerator DuckRoutine(float duration, float duckVol)
    {
        musicSource.volume = defaultMusicVolume * duckVol;
        yield return new WaitForSecondsRealtime(duration);
        musicSource.volume = defaultMusicVolume;
    }

    // ================= SFX =================

    public void PlaySound(AudioClip clip, float volume = 1f, float randomPitch = 0f)
    {
        if (clip == null) return;

        foreach (AudioSource src in audioSourceList)
        {
            if (!src.isPlaying)
            {
                src.pitch = 1f + Random.Range(-randomPitch, randomPitch);
                src.volume = volume;
                src.PlayOneShot(clip);
                return;
            }
        }

        GameObject obj = new GameObject("AudioSource");
        obj.transform.parent = transform;

        AudioSource newSource = obj.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        newSource.loop = false;

        newSource.pitch = 1f + Random.Range(-randomPitch, randomPitch);
        newSource.volume = volume;
        newSource.PlayOneShot(clip);

        audioSourceList.Add(newSource);
    }

    // ================= HELPERS =================

    public void PlayHit()
    {
        PlaySound(hitSound, 1f);
        DuckMusic(0.4f, 0.3f);
    }

    public void PlayFall()
    {
        PlaySound(fallSound, 1f);
    }

    public void PlayPistol()
    {
        PlaySound(pistolSound, 0.8f, 0.1f);
    }

    public void PlayCannonShoot()
    {
        PlaySound(cannonShootSound, 0.8f, 0.05f);
    }

    public void PlayPass()
    {
        PlaySound(passSound, 0.7f);
    }
}