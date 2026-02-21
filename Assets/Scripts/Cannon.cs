using System.Collections;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public Transform TurretPivot;
    public Transform player;
    public Transform shootPoint;
    public GameObject bulletPrefab;

    public float aggroDistance = 30f;
    public float fireDelay = 2f;

    bool isShooting;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");

        if (p != null)
        {
            player = p.transform;
        }
        else
        {
            Debug.LogError("PLAYER NOT FOUND");
        }
    }

    void Update()
    {
            // Tambah pengecekan waitingToStart / game belum mulai
    if (GameManager.Instance == null || GameManager.Instance.IsGameOver) return;
    
    // Tambahkan property ini di GameManager
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
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            AudioManager.Instance.PlayCannonShoot();
            Instantiate(
                bulletPrefab,
                shootPoint.position,
                Quaternion.Euler(0f, 0f, angle)
            );

            yield return new WaitForSeconds(fireDelay);
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }   
}
