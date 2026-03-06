using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip menuMusic;
    public AudioClip levelMusic;

    [Header("SFX")]
    public List<AudioSource> audioSourceList = new List<AudioSource>();

    public AudioClip hitSound;
    public AudioClip fallSound;
    public AudioClip pistolSound;
    public AudioClip cannonShootSound;
    public AudioClip passSound;

    [Header("Special Music")]
    public AudioClip winMusic;

    float defaultMusicVolume;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            defaultMusicVolume = musicSource.volume;

            // Subscribe ke event scene loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Otomatis play menu music saat masuk scene MainMenu atau LevelSelect
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu" || scene.name == "LevelSelect")
        {
            PlayMenuMusic(forceRestart: true);
        }
    }

    void Start()
    {
        int music = PlayerPrefs.GetInt("music", 1);
        AudioListener.volume = music == 1 ? 1f : 0f;
    }

    // ================= MUSIC =================

    public void ToggleMusic(bool state)
    {
        AudioListener.volume = state ? 1f : 0f;
        PlayerPrefs.SetInt("music", state ? 1 : 0);
    }

    public void PlayMenuMusic(bool forceRestart = false)
    {
        StartCoroutine(FadeMusic(menuMusic, 1f, forceRestart));
    }

    public void PlayLevelMusic()
    {
        StartCoroutine(FadeMusic(levelMusic, 1f));
    }

    public void PlayWinMusic()
    {
        StartCoroutine(FadeMusic(winMusic, 1f));
    }

    public void StopMusic()
    {
        StopAllCoroutines();
        musicSource.Stop();
        musicSource.clip = null;
        musicSource.volume = defaultMusicVolume;
    }

    public void StopAllSFX()
    {
        foreach (AudioSource src in audioSourceList)
        {
            if (src != null) src.Stop();
        }
    }

    IEnumerator FadeMusic(AudioClip newClip, float fadeTime, bool forceRestart = false)
    {
        if (newClip == null) yield break;

        // Skip kalau sudah play clip yang sama DAN tidak dipaksa restart
        if (!forceRestart && musicSource.clip == newClip && musicSource.isPlaying)
            yield break;

        if (!musicSource.isPlaying || forceRestart)
        {
            musicSource.Stop();
            musicSource.clip = newClip;
            musicSource.loop = true;
            musicSource.volume = defaultMusicVolume;
            musicSource.Play();
            yield break;
        }

        // Fade out
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

        // Fade in
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

    public void PlayFall()      { PlaySound(fallSound, 1f); }
    public void PlayPistol()    { PlaySound(pistolSound, 0.8f, 0.1f); }
    public void PlayCannonShoot() { PlaySound(cannonShootSound, 0.8f, 0.05f); }
    public void PlayPass()      { PlaySound(passSound, 0.7f); }
}