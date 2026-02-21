using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;

    Vector3 moveDir;

float leftEdge;

void Start()
{
    moveDir = transform.right;

    leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 1f;

    Destroy(gameObject, 5f);
}

void Update()
{
    transform.position += moveDir * speed * Time.deltaTime;

    if (transform.position.x < leftEdge)
    {
        Destroy(gameObject);
    }
}


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
