using UnityEngine;

public class CannonHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    float hp;

    [Header("Death Effect")]
    [SerializeField] private GameObject deathParticlePrefab; // Assign di Inspector
    [SerializeField] private float particleDuration = 2f;    // Berapa lama sebelum dihapus

    void Start()
    {
        hp = maxHealth;
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;

        if (hp <= 0f)
        {
            GameManager.Instance.AddObjectiveProgress();
            SpawnDeathParticle();
            Destroy(gameObject);
        }
    }

    void SpawnDeathParticle()
    {
        if (deathParticlePrefab == null)
        {
            Debug.LogWarning("deathParticlePrefab belum di-assign di Inspector!");
            return;
        }

        // Spawn di world space (bukan child cannon) agar tidak ikut terhapus
        GameObject fx = Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);

        // Pastikan tidak punya parent (bebas dari hierarchy cannon)
        fx.transform.SetParent(null);

        // Ambil ParticleSystem dan play manual
        ParticleSystem ps = fx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            // Gunakan durasi dari ParticleSystem jika ada, fallback ke particleDuration
            float duration = ps.main.duration + ps.main.startLifetime.constantMax;
            Destroy(fx, duration);
        }
        else
        {
            Destroy(fx, particleDuration);
        }
    }
}