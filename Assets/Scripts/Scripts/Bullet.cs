using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;

    Vector3 moveDir;

    void Start()
    {
        moveDir = transform.right;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.position += moveDir * speed * Time.deltaTime;
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
