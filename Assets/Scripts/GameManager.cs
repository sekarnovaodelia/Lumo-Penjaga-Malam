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

    private bool isPaused = false;
    public bool IsPaused => isPaused;

    public event System.Action OnPlay;

    // ================= OBJECTIVE =================

    public enum ObjectiveType
    {
        None,
        PassTurret,
        AvoidCannon,
        DestroyCannon
    }

    [Header("Objective UI")]
    [SerializeField] private TMP_Text objectiveText;

    public int score => objectiveProgress;

    private ObjectiveType currentObjective = ObjectiveType.None;
    public ObjectiveType CurrentObjective => currentObjective;

    private int objectiveTarget = 0;
    private int objectiveProgress = 0;

    public bool IsGameOver { get; private set; }
    private bool waitingToStart = true;
    public bool IsWaitingToStart => waitingToStart;

    // =======================================================

    void ClearProjectiles()
    {
        foreach (Bullet b in FindObjectsOfType<Bullet>())
            DestroyImmediate(b.gameObject);

        foreach (Cannon c in FindObjectsOfType<Cannon>())
            c.StopAllCoroutines();
    }

    void Awake()
    {
        if (Instance != null) DestroyImmediate(gameObject);
        else Instance = this;
    }

    void Start()
    {
        getReady.SetActive(true);

        waitingToStart = true;
        Time.timeScale = 0f;
        player.enabled = false;

        if (pauseButton != null) pauseButton.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        levelPassedPanel.SetActive(false);

        // Hide health & objective at start
        SetHealthUIVisible(false);
        SetObjectiveUIVisible(false);

        currentHealth = maxHealth;
        UpdateHealthUI();

        AudioManager.Instance.PlayMenuMusic();
    }

    void Update()
    {
        if (waitingToStart && Input.GetKeyDown(KeyCode.Space))
        {
            Play();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGameOver) return;
            if (waitingToStart) return;

            if (isPaused) Resume();
            else Pause();
        }
    }

    // ================= PLAY =================

    public void Play()
    {
        waitingToStart = false;
        IsGameOver = false;
        isPaused = false;

        if (BackgroundPanel != null) BackgroundPanel.SetActive(false);

        ClearProjectiles();
        objectiveProgress = 0;
        UpdateObjectiveUI();

        currentHealth = maxHealth;
        UpdateHealthUI();

        getReady.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        levelPassedPanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);

        // Show health & objective when playing
        SetHealthUIVisible(true);
        SetObjectiveUIVisible(true);

        Time.timeScale = 1f;

        player.gameObject.SetActive(true);
        player.enabled = true;

        AudioManager.Instance.PlayChaseMusic();

        foreach (Pipes p in FindObjectsOfType<Pipes>())
            Destroy(p.gameObject);

        foreach (Bullet b in FindObjectsOfType<Bullet>())
            Destroy(b.gameObject);

        if (spawner != null)
            spawner.StartSpawning();

        OnPlay?.Invoke();
    }

    // ================= PAUSE =================

    public void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);
        Time.timeScale = 0f;
        AudioManager.Instance.PlayMenuMusic();
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);
        Time.timeScale = 1f;
        AudioManager.Instance.PlayChaseMusic();
    }

    // ================= DAMAGE =================

    public void TakeDamage(int dmg)
    {
        if (IsGameOver) return;

        currentHealth -= dmg;
        UpdateHealthUI();

        AudioManager.Instance.PlayHit();

        if (currentHealth <= 0)
            GameOver();
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
        {
            if (heart != null)
                heart.gameObject.SetActive(visible);
        }
    }

    void SetObjectiveUIVisible(bool visible)
    {
        if (objectiveText != null)
            objectiveText.gameObject.SetActive(visible);
    }

    // ================= GAME OVER =================

    public void GameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        player.SetFall();
        AudioManager.Instance.PlayFall();
        gameOverPanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);
        SetHealthUIVisible(false);

        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(1f);

        player.enabled = false;
        Time.timeScale = 0f;
    }

    // ================= OBJECTIVE =================

    public void SetObjective(ObjectiveType type, int target)
    {
        currentObjective = type;
        objectiveTarget = target;
        objectiveProgress = 0;
        UpdateObjectiveUI();
    }

    public void AddObjectiveProgress()
    {
        if (IsGameOver || currentObjective == ObjectiveType.None)
            return;

        objectiveProgress++;
        UpdateObjectiveUI();

        if (objectiveProgress >= objectiveTarget)
            LevelComplete();
    }

    void UpdateObjectiveUI()
    {
        if (objectiveText == null) return;

        string label = currentObjective switch
        {
            ObjectiveType.PassTurret => "Lewati Turret",
            ObjectiveType.AvoidCannon => "Hindari Meriam",
            ObjectiveType.DestroyCannon => "Hancurkan Meriam",
            _ => ""
        };

        objectiveText.text = label + ": " + objectiveProgress + " / " + objectiveTarget;
    }

    public void StartChaseMusic()
    {
        AudioManager.Instance.PlayChaseMusic();
    }

    public void StopChaseMusic()
    {
        AudioManager.Instance.PlayMenuMusic();
    }

    // ================= LEVEL COMPLETE =================

    void LevelComplete()
    {
        Time.timeScale = 0f;
        player.enabled = false;

        AudioManager.Instance.PlayPass();
        AudioManager.Instance.DuckMusic(1f, 0.2f);

        if (pauseButton != null) pauseButton.SetActive(false);
        levelPassedPanel.SetActive(true);

        // Save progress — unlock the next level
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int savedLevel = PlayerPrefs.GetInt("levelUnlocked", 1);
        if (currentLevel + 1 > savedLevel)
        {
            PlayerPrefs.SetInt("levelUnlocked", currentLevel + 1);
            PlayerPrefs.Save();
        }
    }

    // ================= SCENE MANAGEMENT =================

    public void Restart()
    {
        Bullet[] bullets = FindObjectsOfType<Bullet>(true);
        foreach (Bullet b in bullets)
            Destroy(b.gameObject);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GoToMainMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}