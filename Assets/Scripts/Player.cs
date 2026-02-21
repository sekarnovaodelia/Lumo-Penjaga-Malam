using UnityEngine;

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
    if (GameManager.Instance.IsWaitingToStart) return; // player beku, input dihandle GameManager
    if (GameManager.Instance.IsPaused) return;         // tambah ini juga biar pause beneran beku

    if (IsCinematic) return;

    if (Input.GetKeyDown(KeyCode.Space))
    {
        direction = Vector3.up * strength;
    }

    direction.y += gravity * Time.deltaTime;
    transform.position += direction * Time.deltaTime;

    Vector3 rotation = transform.eulerAngles;
    rotation.z = direction.y * tilt;
    transform.eulerAngles = rotation;
}


    private void AnimateSprite()
    {
        spriteIndex++;

        if (spriteIndex >= sprites.Length) {
            spriteIndex = 0;
        }

        if (spriteIndex < sprites.Length && spriteIndex >= 0) {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle")) {
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
        if (active)
        {
            transform.localScale = Vector3.one * 1.2f; // Simply scale up for "glow" effect
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
}
