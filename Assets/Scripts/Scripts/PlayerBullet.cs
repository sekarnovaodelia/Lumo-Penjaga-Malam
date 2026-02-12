using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 35f;
    public float damage = 25f;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;

        Destroy(gameObject,5f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CannonHealth cannon = other.GetComponent<CannonHealth>();

        if(cannon != null)
        {
            cannon.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
