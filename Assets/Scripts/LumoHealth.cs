using System.Collections;
using UnityEngine;

public class LumoHealth : MonoBehaviour
{
    public int maxHealth = 3;
    int currentHealth;

    public float knockbackForce = 6f;
    public float invincibleTime = 0.5f;

    bool isInvincible;
    Rigidbody2D rb;
    SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg, Vector2 hitSource)
    {
        if (isInvincible) return;

        currentHealth -= dmg;

        Knockback(hitSource);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibleBlink());
    }

    void Knockback(Vector2 hitSource)
    {
        Vector2 dir = ((Vector2)transform.position - hitSource).normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
    }

    IEnumerator InvincibleBlink()
    {
        isInvincible = true;

        float t = 0f;

        while (t < invincibleTime)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.08f);
            t += 0.08f;
        }

        sr.enabled = true;
        isInvincible = false;
    }

void Die()
{
    GameManager.Instance.GameOver();
    gameObject.SetActive(false);
}
}
