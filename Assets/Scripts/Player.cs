using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public Sprite[] sprites;
    public float strength = 5f;
    public float gravity = -9.81f;
    public float tilt = 5f;

    private SpriteRenderer spriteRenderer;
    private Vector3 direction;
    private int spriteIndex;

    public bool IsCinematic { get; set; } = false;

    [Header("Boost")]
    private float boostMultiplier = 1f;
    private float boostTimer = 0f;
    public bool IsBoosted => boostTimer > 0f;
    private float originalParallaxSpeed = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    private void OnEnable()
    {
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
        direction = Vector3.zero;
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver) return;
        if (GameManager.Instance.IsWaitingToStart) return;
        if (GameManager.Instance.IsPaused) return;
        if (IsCinematic) return;

        // ——— Countdown Boost ———
        if (boostTimer > 0f)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                boostMultiplier = 1f;
                SetGlow(false);

                Parallax parallax = FindObjectOfType<Parallax>();
                if (parallax != null)
                    parallax.animationSpeed = originalParallaxSpeed;

                if (BossManager.Instance != null)
                    BossManager.Instance.SetSlowed(false);
            }
        }

        // ——— INPUT: Keyboard (PC) ———
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Flap();
        }

        // ——— INPUT: Touch (Android) — tap sisi KIRI layar = terbang ———
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                // Skip kalau touch kena UI Button agar tidak konflik
                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    continue;

                // Seluruh layar bisa untuk terbang (bukan hanya kiri)
                // karena tombol shoot ada di kanan bawah dan sudah dihandle UI
                Flap();
                break;
            }
        }

        // ——— Gravity & Movement ———
        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;

        Vector3 rotation = transform.eulerAngles;
        rotation.z = direction.y * tilt;
        transform.eulerAngles = rotation;
    }

    /// <summary>Buat karakter terbang ke atas.</summary>
    private void Flap()
    {
        direction = Vector3.up * strength;
    }

    private void AnimateSprite()
    {
        spriteIndex++;
        if (spriteIndex >= sprites.Length)
            spriteIndex = 0;

        if (spriteIndex < sprites.Length && spriteIndex >= 0)
            spriteRenderer.sprite = sprites[spriteIndex];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
        }
        else if (other.CompareTag("Scoring"))
        {
            if (GameManager.Instance.CurrentObjective == GameManager.ObjectiveType.PassTurret)
            {
                GameManager.Instance.AddObjectiveProgress();
                other.enabled = false;
            }
        }
        else if (other.CompareTag("ScoringCannon"))
        {
            if (GameManager.Instance.CurrentObjective == GameManager.ObjectiveType.AvoidCannon)
            {
                GameManager.Instance.AddObjectiveProgress();
                other.enabled = false;
            }
        }
    }

    public void SetFall()
    {
        direction = Vector3.zero;
        direction.y = -3.5f;
    }

    public void SetGlow(bool active)
    {
        transform.localScale = active ? Vector3.one * 1.2f : Vector3.one;
    }

    public void ActivateBoost(float multiplier, float duration)
    {
        boostMultiplier = multiplier;
        boostTimer = duration;
        SetGlow(true);

        Parallax parallax = FindObjectOfType<Parallax>();
        if (parallax != null)
        {
            originalParallaxSpeed = parallax.animationSpeed;
            parallax.animationSpeed *= multiplier;
        }

        if (BossManager.Instance != null)
            BossManager.Instance.SetSlowed(true);
    }
}