using UnityEngine;

public class BoostPickup : MonoBehaviour
{
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float boostDuration = 3f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveRange = 2f;
    [SerializeField] private float scrollSpeed = 1f; // samain dengan animationSpeed di Parallax

    private float startY;

    void Start()
    {
        startY = transform.position.y;

        // ambil otomatis speed dari Parallax kalau ada
        Parallax parallax = FindObjectOfType<Parallax>();
        if (parallax != null)
            scrollSpeed = parallax.animationSpeed;
    }

    void Update()
    {
        // gerak ke kiri ikut parallax
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        // naik turun
        float newY = startY + Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = new Vector3(
            transform.position.x, newY, transform.position.z);

        // destroy kalau udah keluar layar kiri
        if (transform.position.x < -20f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<Player>()?.ActivateBoost(boostMultiplier, boostDuration);
        Destroy(gameObject);
    }
}