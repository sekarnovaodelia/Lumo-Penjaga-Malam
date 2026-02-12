using UnityEngine;

public class CannonHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    float hp;

    void Start()
    {
        hp = maxHealth;
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;

        if(hp <= 0f)
            Destroy(gameObject);
    }
}
