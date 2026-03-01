using UnityEngine;
using System.Collections;
using TMPro;

public class Gun : MonoBehaviour
{
    public Transform shootPoint;
    public GameObject bulletPrefab;

    public float aimRange = 20f;

    [Header("Ammo System")]
    public int magazineSize = 4;
    public int currentAmmo;
    public int totalAmmo = 0;

    public float reloadTime = 1.5f;
    bool isReloading = false;

    [Header("UI")]
    public TMP_Text ammoText;
    public TMP_Text reloadText;

    void Start()
    {
        currentAmmo = magazineSize; // mulai penuh
        UpdateUI();

        if (reloadText != null)
            reloadText.gameObject.SetActive(false);
    }

    void Update()
    {
        Aim();
        Shoot();
    }

    void UpdateUI()
    {
        if (ammoText != null)
            ammoText.text = "Peluru: " + currentAmmo + " | Total: " + totalAmmo;
    }

    void Shoot()
    {
        if (GameManager.Instance.IsWaitingToStart) return;
        if (GameManager.Instance.IsGameOver) return;
        if (isReloading) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (currentAmmo > 0)
            {
                Fire();
            }
            else if (totalAmmo > 0)
            {
                StartCoroutine(Reload());
            }
        }
    }

    void Fire()
    {
        currentAmmo--;
        UpdateUI();

        AudioManager.Instance.PlayPistol();
        Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

        if (currentAmmo <= 0 && totalAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
{
    isReloading = true;

    if (reloadText != null)
        reloadText.gameObject.SetActive(true);

    float timer = reloadTime;

    while (timer > 0f)
    {
        timer -= Time.deltaTime;

        if (reloadText != null)
            reloadText.text = "Reloading: " + timer.ToString("F1");

        yield return null;
    }

    int ammoNeeded = magazineSize - currentAmmo;
    int ammoToLoad = Mathf.Min(ammoNeeded, totalAmmo);

    currentAmmo += ammoToLoad;
    totalAmmo -= ammoToLoad;

    UpdateUI();

    if (reloadText != null)
        reloadText.gameObject.SetActive(false);

    isReloading = false;
}

    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
        UpdateUI();

        // kalau mag kosong langsung auto reload
        if (currentAmmo == 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    void Aim()
    {
        GameObject target = FindClosestCanon();
        if (target == null) return;

        Vector3 dir = target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    GameObject FindClosestCanon()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("canon");

        GameObject closest = null;
        float minDist = aimRange;

        foreach (GameObject e in enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                closest = e;
            }
        }

        return closest;
    }
}