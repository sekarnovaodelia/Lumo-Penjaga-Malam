using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // langsung kasih velocity saat spawn
        rb.velocity = transform.right * speed;

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
            Destroy(gameObject);
        }
    }
}
