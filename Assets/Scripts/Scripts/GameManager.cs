using UnityEngine;
using UnityEngine.UI;
[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private Player player;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject getReady;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip chaseMusic;
    [SerializeField] private GameObject BackgroundPanel;
    [SerializeField] private GameObject nextLevelCanvas;

    public Spawner Spawner => spawner; // Public accessor for Spawner

    public int score { get; private set; } = 0;
    public bool IsGameOver { get; private set; } = false;
    private bool waitingToStart = true;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

private void Start()
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
}


    private void Update()
    {
    if (IsGameOver || waitingToStart)
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Play();
        }
    }
    }

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

    public void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;
    }

    public event System.Action OnScoreIncreased;
    public event System.Action OnPlay;

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

        playButton.SetActive(false);
        gameOver.SetActive(false);
        getReady.SetActive(false);
        if (nextLevelCanvas != null) nextLevelCanvas.SetActive(false);

        Time.timeScale = 1f;
        player.enabled = true;

        Pipes[] pipes = FindObjectsOfType<Pipes>();

        for (int i = 0; i < pipes.Length; i++) {
            Destroy(pipes[i].gameObject);
        }

        // Restart spawner in case it was stopped by the enemy
        if (spawner != null) {
            spawner.StartSpawning();
        }

        OnPlay?.Invoke();
    }

    public void GameOver()
    {
        IsGameOver = true;
        playButton.SetActive(true);
        gameOver.SetActive(true);

        Pause();
    }

    public void HideUI()
    {
        if (playButton != null) playButton.SetActive(false);
        if (gameOver != null) gameOver.SetActive(false);
        if (getReady != null) getReady.SetActive(false);
        if (nextLevelCanvas != null) nextLevelCanvas.SetActive(false);
    }
    
    public void LevelComplete()
    {
        if (nextLevelCanvas != null)
        {
            nextLevelCanvas.SetActive(true);
        }
        Pause();
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
        OnScoreIncreased?.Invoke();
    }
}