using System.Collections;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public Transform TurretPivot;
    public Transform player;
    public Transform shootPoint;
    public GameObject bulletPrefab;

    public float aggroDistance = 30f;

    [Header("Fire Settings (override by LevelCannonSettings)")]
    public float fireDelay = 2f;
    public int burstCount = 1;
    public float spreadAngle = 10f;
    public float bulletSpeed = 20f; // kecepatan peluru

    bool isShooting;
    int level;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogError("PLAYER NOT FOUND");

        level = GameManager.Instance.CurrentLevel;
        ApplyLevelSettings();
    }

    void ApplyLevelSettings()
    {
        // Cek apakah ada LevelCannonSettings di scene — kalau ada, pakai itu
        LevelCannonSettings settings = FindObjectOfType<LevelCannonSettings>();
        if (settings != null)
        {
            fireDelay   = settings.fireDelay;
            burstCount  = settings.burstCount;
            spreadAngle = settings.spreadAngle;
            bulletSpeed = settings.bulletSpeed;
            return;
        }

        // Fallback: hardcode per level
        fireDelay   = 2f;
        burstCount  = 1;
        spreadAngle = 10f;
        bulletSpeed = 20f;

        switch (level)
        {
            case 5:
                fireDelay   = 1.5f;
                bulletSpeed = 22f;
                break;
            case 6:
                fireDelay   = 3f;
                burstCount  = 2;
                spreadAngle = 12f;
                bulletSpeed = 24f;
                break;
            case 7:
                fireDelay   = 1.2f;
                bulletSpeed = 26f;
                break;
            case 8:
                fireDelay   = 1f;
                burstCount  = 2;
                bulletSpeed = 28f;
                break;
            case 9:
                fireDelay   = 0.8f;
                burstCount  = 3;
                spreadAngle = 15f;
                bulletSpeed = 30f;
                break;
            case 10:
                fireDelay   = 0.6f;
                burstCount  = 3;
                spreadAngle = 15f;
                bulletSpeed = 32f;
                break;
        }
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver) return;
        if (GameManager.Instance.IsWaitingToStart) return;

        if (player == null || !player.gameObject.activeInHierarchy || TurretPivot == null)
        {
            StopAllCoroutines();
            isShooting = false;
            return;
        }

        AimToPlayer();

        float dist = Vector2.Distance(player.position, transform.position);

        if (dist < aggroDistance && !isShooting && !GameManager.Instance.IsGameOver)
        {
            isShooting = true;
            StartCoroutine(ShootLoop());
        }
        else if ((dist > aggroDistance || GameManager.Instance.IsGameOver) && isShooting)
        {
            isShooting = false;
            StopAllCoroutines();
        }
    }

    void AimToPlayer()
    {
        Vector3 dir = player.position - TurretPivot.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        TurretPivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    IEnumerator ShootLoop()
    {
        while (true)
        {
            if (player == null || !player.gameObject.activeInHierarchy)
                yield break;

            Vector3 dir = player.position - shootPoint.position;
            float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            for (int i = 0; i < burstCount; i++)
            {
                float angle = baseAngle;
                if (burstCount > 1)
                {
                    float offset = (i - (burstCount - 1) / 2f) * spreadAngle;
                    angle += offset;
                }

                GameObject bulletObj = Instantiate(
                    bulletPrefab,
                    shootPoint.position,
                    Quaternion.Euler(0f, 0f, angle)
                );

                // Set bullet speed langsung ke komponen Bullet
                Bullet bullet = bulletObj.GetComponent<Bullet>();
                if (bullet != null)
                    bullet.speed = bulletSpeed;
            }

            AudioManager.Instance.PlayCannonShoot();
            yield return new WaitForSeconds(fireDelay);
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}