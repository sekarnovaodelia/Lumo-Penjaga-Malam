using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance { get; private set; }

    [Header("Boss Settings")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float runSpeed = 1.5f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatRange = 2f;
    [SerializeField] private float stopDuration = 5f;
    private int currentHealth;

    [Header("Math UI")]
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TMP_Text[] answerTexts;
    [SerializeField] private float questionRange = 5f;

    [Header("Boss UI")]
    [SerializeField] private GameObject bossHealthBar;
    [SerializeField] private Image bossHealthFill;
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private TMP_Text timerText;

    private enum BossState { Running, Stunned, Dead }
    private BossState state = BossState.Running;

    private SpriteRenderer spriteRenderer;
    private float startY;
    private float stunTimer;
    private bool questionActive = false;
    private int correctAnswer;
    private Transform playerTransform;
    private float bossTargetX;
    private bool isSlowed = false;

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        startY = transform.position.y;
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossHealthBar.SetActive(false);
        questionPanel.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        bossTargetX = transform.position.x; // mulai dari posisi awal
    }

    void Update()
    {
        if (state == BossState.Dead) return;

        if (state == BossState.Running)
        {
            // target X terus bertambah — boss selalu menjauh
            float currentSpeed = isSlowed ? runSpeed * 0.3f : runSpeed;
            bossTargetX += currentSpeed * Time.deltaTime;

            // tapi dibatasi max jarak dari player
            float maxX = playerTransform.position.x + 10f;
            float minX = playerTransform.position.x + 3f;
            bossTargetX = Mathf.Clamp(bossTargetX, minX, maxX);

            // gerak ke target
            float newX = Mathf.MoveTowards(
                transform.position.x, bossTargetX, currentSpeed * Time.deltaTime);

            // naik turun
            float newY = startY + Mathf.Sin(Time.time * floatSpeed) * floatRange;

            transform.position = new Vector3(newX, newY, transform.position.z);

            float dist = Vector3.Distance(
                transform.position, playerTransform.position);
            if (dist <= questionRange && !questionActive)
                ShowQuestion();
        }
        else if (state == BossState.Stunned)
        {
            stunTimer -= Time.deltaTime;
            if (timerText != null)
               timerText.text = "[ " + Mathf.CeilToInt(stunTimer) + "s ]";

            if (stunTimer <= 0f)
                BossEscape();
        }
    }

    // ===== SOAL =====
    void ShowQuestion()
    {
        questionActive = true;
        Time.timeScale = 0f;
        questionPanel.SetActive(true);
        GenerateQuestion();
    }

    void GenerateQuestion()
    {
        int difficulty = Mathf.Clamp((maxHealth - currentHealth) + 1, 1, 4);
        int a, b;
        string question;

        switch (difficulty)
        {
            case 1:
                a = Random.Range(1, 10); b = Random.Range(1, 10);
                correctAnswer = a + b;
                question = $"{a} + {b} = ?";
                break;
            case 2:
                a = Random.Range(10, 30); b = Random.Range(1, a);
                correctAnswer = a - b;
                question = $"{a} - {b} = ?";
                break;
            case 3:
                a = Random.Range(2, 10); b = Random.Range(2, 10);
                correctAnswer = a * b;
                question = $"{a} × {b} = ?";
                break;
            default:
                b = Random.Range(2, 8);
                correctAnswer = Random.Range(2, 10);
                a = correctAnswer * b;
                question = $"{a} ÷ {b} = ?";
                break;
        }

        questionText.text = question;
        GenerateChoices();
    }

    void GenerateChoices()
    {
        int[] choices = new int[4];
        choices[0] = correctAnswer;

        for (int i = 1; i < 4; i++)
        {
            int wrong;
            do {
                int offset = Random.Range(-5, 6);
                if (offset == 0) offset = 1;
                wrong = correctAnswer + offset;
            } while (System.Array.IndexOf(choices, wrong) != -1 || wrong < 0);
            choices[i] = wrong;
        }

        for (int i = 0; i < choices.Length; i++)
        {
            int rand = Random.Range(i, choices.Length);
            (choices[i], choices[rand]) = (choices[rand], choices[i]);
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int choice = choices[i];
            answerTexts[i].text = choice.ToString();
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswer(choice));
        }
    }

    void OnAnswer(int chosen)
    {
        questionPanel.SetActive(false);
        Time.timeScale = 1f;
        questionActive = false;

        if (chosen == correctAnswer)
            StunBoss();
        else
            GameManager.Instance.TakeDamage(1);
    }

    // ===== STATE BOSS =====
    void StunBoss()
    {
        state = BossState.Stunned;
        stunTimer = stopDuration;

        // boss diem — gelapkan sprite sedikit biar keliatan bedanya
        spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f);

        bossHealthBar.SetActive(true);
        bossNameText.text = "THE IRON TURRET";

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = "⏱ " + Mathf.CeilToInt(stopDuration) + "s";
        }
    }

    void BossEscape()
    {
        state = BossState.Running;
        questionActive = false;

        // balik warna normal
        spriteRenderer.color = Color.white;

        currentHealth = Mathf.Min(currentHealth + 1, maxHealth);
        UpdateHealthBar();
        startY = transform.position.y;

        if (timerText != null) timerText.gameObject.SetActive(false);
    }

    // ===== KENA TEMBAK =====
    void OnTriggerEnter2D(Collider2D other)
    {
        if (state != BossState.Stunned) return; // hanya mau damage saat diem
        if (!other.CompareTag("Bullet")) return;

        Destroy(other.gameObject);
        TakeDamage(1);
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        UpdateHealthBar();
        if (currentHealth <= 0) BossDead();
    }

    void UpdateHealthBar()
    {
        if (bossHealthFill != null)
            bossHealthFill.fillAmount = (float)currentHealth / maxHealth;
    }

    void BossDead()
    {
        state = BossState.Dead;
        bossHealthBar.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);

        GameManager.Instance.AddObjectiveProgress();
        Destroy(gameObject, 0.5f);
    }

    public void SetSlowed(bool slowed)
    {
        isSlowed = slowed;
    }
}