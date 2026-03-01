using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    public int healAmount = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddHealth(healAmount);
            Destroy(gameObject);
        }
    }
}