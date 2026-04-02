using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private Spawner spawner;
    [SerializeField] private int levelIndex = 1;
    public int CurrentLevel => levelIndex;

    [Header("Health UI")]
    [SerializeField] private Image[] hearts;
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("UI Panels")]
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject getReady;
    [SerializeField] private GameObject BackgroundPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject levelPassedPanel;
    [SerializeField] private GameObject congratulationsPanel;

    [Header("Free Mode Score Panel")]
    [SerializeField] private GameObject scorePanelFreeMode;
    [SerializeField] private TMP_Text finalScoreText;

    private bool isPaused = false;
    public bool IsPaused => isPaused;

    public event System.Action OnPlay;
    public event System.Action OnGameOver;
    public event System.Action OnLevelComplete;

    public enum ObjectiveType { None, PassTurret, AvoidCannon, DestroyCannon }

    [Header("Objective UI")]
    [SerializeField] private TMP_Text objectiveText;

    public int score => objectiveProgress;

    private ObjectiveType currentObjective = ObjectiveType.None;
    public ObjectiveType CurrentObjective => currentObjective;

    private int objectiveTarget   = 0;
    private int objectiveProgress = 0;

    public bool IsGameOver       { get; private set; }
    public bool IsWaitingToStart => waitingToStart;
    private bool waitingToStart  = true;

    public bool IsFreeMode { get; private set; }

    private const int LAST_LEVEL = 10;

    void ClearProjectiles()
    {
        foreach (Bullet b in FindObjectsOfType<Bullet>())   DestroyImmediate(b.gameObject);
        foreach (Cannon c in FindObjectsOfType<Cannon>())   c.StopAllCoroutines();
    }

    void Awake()
    {
        if (Instance != null) DestroyImmediate(gameObject);
        else Instance = this;
    }

    void Start()
    {
        if (getReady != null) getReady.SetActive(true);

        waitingToStart = true;
        Time.timeScale = 0f;
        if (player != null) player.enabled = false;

        if (pauseButton          != null) pauseButton.SetActive(false);
        if (pausePanel           != null) pausePanel.SetActive(false);
        if (gameOverPanel        != null) gameOverPanel.SetActive(false);
        if (levelPassedPanel     != null) levelPassedPanel.SetActive(false);
        if (congratulationsPanel != null) congratulationsPanel.SetActive(false);
        if (scorePanelFreeMode   != null) scorePanelFreeMode.SetActive(false);

        SetHealthUIVisible(false);
        SetObjectiveUIVisible(false);

        currentHealth = maxHealth;
        UpdateHealthUI();

        AudioManager.Instance.PlayMenuMusic();
    }

    void Update()
    {
        if (waitingToStart)
        {
            if (Input.GetKeyDown(KeyCode.Space)) { Play(); return; }
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) { Play(); return; }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGameOver || waitingToStart) return;
            if (isPaused) Resume(); else Pause();
        }
    }

    // ================= PLAY =================

    public void Play()
    {
        waitingToStart = false;
        IsGameOver     = false;
        isPaused       = false;

        if (BackgroundPanel      != null) BackgroundPanel.SetActive(false);
        if (getReady             != null) getReady.SetActive(false);
        if (pausePanel           != null) pausePanel.SetActive(false);
        if (gameOverPanel        != null) gameOverPanel.SetActive(false);
        if (levelPassedPanel     != null) levelPassedPanel.SetActive(false);
        if (congratulationsPanel != null) congratulationsPanel.SetActive(false);
        if (scorePanelFreeMode   != null) scorePanelFreeMode.SetActive(false);

        ClearProjectiles();
        objectiveProgress = 0;
        UpdateObjectiveUI();

        currentHealth = maxHealth;
        UpdateHealthUI();

        if (pauseButton != null) pauseButton.SetActive(true);

        SetHealthUIVisible(true);
        SetObjectiveUIVisible(true);

        Time.timeScale = 1f;

        if (player != null)
        {
            player.gameObject.SetActive(true);
            player.enabled = true;
        }

        AudioManager.Instance.PlayLevelMusic(levelIndex);

        foreach (Pipes  p in FindObjectsOfType<Pipes>())  Destroy(p.gameObject);
        foreach (Bullet b in FindObjectsOfType<Bullet>()) Destroy(b.gameObject);

        if (spawner != null) spawner.StartSpawning();

        OnPlay?.Invoke();
    }

    // ================= PAUSE =================

    public void Pause()
    {
        isPaused = true;
        if (pausePanel  != null) pausePanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);
        Time.timeScale = 0f;
        AudioManager.Instance.PlayMenuMusic();
    }

    public void Resume()
    {
        isPaused = false;
        if (pausePanel  != null) pausePanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);
        Time.timeScale = 1f;
        AudioManager.Instance.PlayLevelMusic(levelIndex);
    }

    // ================= DAMAGE =================

    public void TakeDamage(int dmg)
    {
        if (IsGameOver) return;
        currentHealth -= dmg;
        UpdateHealthUI();
        AudioManager.Instance.PlayHit();
        if (player != null) player.FlashHit();
        if (currentHealth <= 0) GameOver();
    }

    void UpdateHealthUI()
    {
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].enabled = i < currentHealth;
    }

    void SetHealthUIVisible(bool visible)
    {
        if (hearts == null) return;
        foreach (Image heart in hearts)
            if (heart != null) heart.gameObject.SetActive(visible);
    }

    public void AddHealth(int amount)
    {
        if (IsGameOver) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthUI();
    }

    void SetObjectiveUIVisible(bool visible)
    {
        if (objectiveText != null) objectiveText.gameObject.SetActive(visible);
    }

    // ================= GAME OVER =================

    public void GameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        if (player  != null) player.SetFall();
        AudioManager.Instance.PlayFall();

        if (pauseButton != null) pauseButton.SetActive(false);
        SetHealthUIVisible(false);
        SetObjectiveUIVisible(false);

        if (spawner != null) spawner.StopSpawning();

        try { OnGameOver?.Invoke(); }
        catch (System.Exception e) { Debug.LogWarning("[GameManager] OnGameOver error: " + e.Message); }

        if (IsFreeMode)
            StartCoroutine(ShowFreeModeScorePanel());
        else
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            StartCoroutine(GameOverRoutine());
        }
    }

    IEnumerator ShowFreeModeScorePanel()
    {
        yield return new WaitForSecondsRealtime(1.2f);

        if (player != null) player.enabled = false;
        Time.timeScale = 0f;

        if (scorePanelFreeMode != null)
        {
            scorePanelFreeMode.SetActive(true);

            if (finalScoreText != null)
            {
                string label = currentObjective switch
                {
                    ObjectiveType.PassTurret    => "Lewati Turret",
                    ObjectiveType.AvoidCannon   => "Hindari Meriam",
                    ObjectiveType.DestroyCannon => "Hancurkan Meriam",
                    _                           => "Score"
                };
                finalScoreText.text = label + "\n" + objectiveProgress;
            }
        }
        else
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
        }
    }

    IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(1f);
        if (player != null) player.enabled = false;
        Time.timeScale = 0f;
    }

    // ================= FREE MODE =================

    public void SetFreeMode(bool value) { IsFreeMode = value; }

    // ================= OBJECTIVE =================

    public void SetObjective(ObjectiveType type, int target)
    {
        currentObjective  = type;
        objectiveTarget   = target;
        objectiveProgress = 0;
        UpdateObjectiveUI();
    }

    public void AddObjectiveProgress()
    {
        if (IsGameOver) return;
        objectiveProgress++;
        UpdateObjectiveUI();
        AudioManager.Instance.PlayScoring();

        if (!IsFreeMode && currentObjective != ObjectiveType.None && objectiveProgress >= objectiveTarget)
            LevelComplete();
    }

    void UpdateObjectiveUI()
    {
        if (objectiveText == null) return;

        string label = currentObjective switch
        {
            ObjectiveType.PassTurret    => "Lewati Turret",
            ObjectiveType.AvoidCannon   => "Hindari Meriam",
            ObjectiveType.DestroyCannon => "Hancurkan Meriam",
            _                           => "Score"
        };

        objectiveText.text = IsFreeMode
            ? label + ": " + objectiveProgress
            : label + ": " + objectiveProgress + " / " + objectiveTarget;
    }

    public void StartChaseMusic() { AudioManager.Instance.PlayLevelMusic(levelIndex); }
    public void StopChaseMusic()  { AudioManager.Instance.PlayMenuMusic(); }

    // ================= LEVEL COMPLETE =================

    void LevelComplete()
    {
        Time.timeScale = 0f;
        if (player != null) player.enabled = false;

        SetHealthUIVisible(false);
        SetObjectiveUIVisible(false);
        if (pauseButton != null) pauseButton.SetActive(false);
        if (spawner != null) spawner.StopSpawning();

        AudioManager.Instance.StopMusic();

        if (levelIndex >= LAST_LEVEL)
        {
            AudioManager.Instance.PlayWinMusic();
            ShowCongratulations();
        }
        else
        {
            AudioManager.Instance.PlayPass();
            if (levelPassedPanel != null) levelPassedPanel.SetActive(true);
        }

        OnLevelComplete?.Invoke();

        int savedLevel = PlayerPrefs.GetInt("levelUnlocked", 1);
        if (levelIndex + 1 > savedLevel)
        {
            PlayerPrefs.SetInt("levelUnlocked", levelIndex + 1);
            PlayerPrefs.Save();
        }
    }

    // ================= CONGRATULATIONS =================

    void ShowCongratulations()
    {
        if (congratulationsPanel != null) congratulationsPanel.SetActive(true);
        else if (levelPassedPanel != null) levelPassedPanel.SetActive(true);
    }

    public void CongratulationsToMainMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    // ================= LEADERBOARD =================

    public void OpenLeaderboard()
    {
        if (LeaderboardManager.Instance == null) return;
        LeaderboardManager.Instance.OpenLeaderboard();
    }

    public void CloseLeaderboard()
    {
        if (LeaderboardManager.Instance == null) return;
        LeaderboardManager.Instance.CloseLeaderboard();
    }

    // ================= SCENE MANAGEMENT =================

    public void Restart()
    {
        foreach (Bullet b in FindObjectsOfType<Bullet>(true)) Destroy(b.gameObject);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.StopAllSFX();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GoToMainMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void GoToFreeModeSelect()
    {
        SceneManager.LoadScene(15);
    }
}