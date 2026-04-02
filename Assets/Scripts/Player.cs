using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Player : MonoBehaviour
{
    public Sprite[] sprites;
    public float strength = 5f;
    public float gravity = -9.81f;
    public float tilt = 5f;

    private SpriteRenderer spriteRenderer;
    private Vector3 direction;
    private int spriteIndex;
    private bool isDead = false;

    public bool IsCinematic { get; set; } = false;

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
        isDead = false;
        spriteRenderer.color = Color.white;
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
        direction = Vector3.zero;
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver)
        {
            if (isDead)
            {
                direction.y += gravity * Time.deltaTime;
                transform.position += direction * Time.deltaTime;
            }
            return;
        }

        if (GameManager.Instance.IsWaitingToStart) return;
        if (GameManager.Instance.IsPaused) return;
        if (IsCinematic) return;

        if (Input.GetKeyDown(KeyCode.Space)) Flap();

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    continue;
                Flap();
                break;
            }
        }

        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;

        Vector3 rotation = transform.eulerAngles;
        rotation.z = direction.y * tilt;
        transform.eulerAngles = rotation;
    }

    private void Flap()
    {
        direction = Vector3.up * strength;
    }

    private void AnimateSprite()
    {
        if (isDead) return;
        spriteIndex++;
        if (spriteIndex >= sprites.Length) spriteIndex = 0;
        if (spriteIndex < sprites.Length && spriteIndex >= 0)
            spriteRenderer.sprite = sprites[spriteIndex];
    }

    public void FlashHit()
    {
        StartCoroutine(HitFlashRoutine());
    }

    IEnumerator HitFlashRoutine()
    {
        // Flash merah 3x
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.08f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.08f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            if (!isDead) GameManager.Instance.GameOver();
        }
        else if (other.CompareTag("Ground"))
        {
            if (isDead)
            {
                direction = Vector3.zero;
                enabled = false;
            }
            else
            {
                GameManager.Instance.GameOver();
            }
        }
        else if (other.CompareTag("Scoring"))
        {
            if (isDead) return;
            if (GameManager.Instance.CurrentObjective == GameManager.ObjectiveType.PassTurret)
            {
                GameManager.Instance.AddObjectiveProgress();
                other.enabled = false;
            }
            else if (GameManager.Instance.IsFreeMode)
            {
                other.enabled = false;
            }
        }
        else if (other.CompareTag("ScoringCannon"))
        {
            if (isDead) return;
            if (GameManager.Instance.CurrentObjective == GameManager.ObjectiveType.None ||
                GameManager.Instance.CurrentObjective == GameManager.ObjectiveType.AvoidCannon)
            {
                GameManager.Instance.AddObjectiveProgress();
                other.enabled = false;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ground") && isDead)
            direction = Vector3.zero;
    }

    public void SetFall()
    {
        isDead = true;
        direction = Vector3.zero;
        direction.y = -3.5f;
    }
}