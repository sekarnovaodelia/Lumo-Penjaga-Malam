using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;

    [Header("Health UI")]
    [SerializeField] private Image[] hearts;
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("UI")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject getReady;
    [SerializeField] private GameObject BackgroundPanel;
    [SerializeField] private GameObject nextLevelCanvas;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip chaseMusic;

    public event System.Action OnPlay;   // <<< penting buat IllusionEnemy

    public int score { get; private set; } = 0;
    public bool IsGameOver { get; private set; } = false;
    private bool waitingToStart = true;

    void Awake()
    {
        if (Instance != null) DestroyImmediate(gameObject);
        else Instance = this;
    }

    void Start()
    {
        playButton.SetActive(true);
        getReady.SetActive(true);

        waitingToStart = true;
        Time.timeScale = 0f;
        player.enabled = false;

        if (scoreText != null)
            scoreText.gameObject.SetActive(false);

        if (nextLevelCanvas != null)
            nextLevelCanvas.SetActive(false);

        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        if (IsGameOver || waitingToStart)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                Play();
        }
    }

    // ========================= PLAY =========================

    public void Play()
    {
        waitingToStart = false;

        if (scoreText != null)
            scoreText.gameObject.SetActive(true);

        if (BackgroundPanel != null)
            BackgroundPanel.SetActive(false);

        IsGameOver = false;
        score = 0;
        scoreText.text = score.ToString();

        currentHealth = maxHealth;
        UpdateHealthUI();

        playButton.SetActive(false);
        gameOver.SetActive(false);
        getReady.SetActive(false);

        if (nextLevelCanvas != null)
            nextLevelCanvas.SetActive(false);

        Time.timeScale = 1f;

        player.gameObject.SetActive(true);
        player.enabled = true;

        // Hapus pipe lama
        Pipes[] pipes = FindObjectsOfType<Pipes>();
        for (int i = 0; i < pipes.Length; i++)
            Destroy(pipes[i].gameObject);

        // Hapus bullet lama
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        for (int i = 0; i < bullets.Length; i++)
            Destroy(bullets[i]);

        if (spawner != null)
            spawner.StartSpawning();

        OnPlay?.Invoke();   // <<< penting buat enemy reset
    }

    // ========================= DAMAGE =========================

    public void TakeDamage(int dmg)
    {
        if (IsGameOver) return;

        currentHealth -= dmg;
        UpdateHealthUI();

        if (currentHealth <= 0)
            GameOver();
    }

    void UpdateHealthUI()
    {
        if (hearts == null) return;

        for (int i = 0; i < hearts.Length; i++)
            hearts[i].enabled = i < currentHealth;
    }

    // ========================= GAME OVER =========================

    public void GameOver()
    {
        IsGameOver = true;

        playButton.SetActive(true);
        gameOver.SetActive(true);

        StopChaseMusic();

        Time.timeScale = 0f;
        player.enabled = false;
    }

    // ========================= AUDIO =========================

    public void StartChaseMusic()
    {
        if (audioSource != null && chaseMusic != null)
        {
            audioSource.clip = chaseMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void StopChaseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // ========================= SCORE =========================

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }
}
