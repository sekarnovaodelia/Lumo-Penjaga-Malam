using UnityEngine;

public class BoostSpawner : MonoBehaviour
{
    [SerializeField] private GameObject boostPrefab;
    [SerializeField] private float spawnInterval = 4f;
    [SerializeField] private float spawnRangeY = 3f;
    [SerializeField] private float spawnX = 12f; // kanan layar
    private float timer = 0f;
    private float lastY = 0f;

    void Update()
    {
        if (GameManager.Instance.IsGameOver) return;
        if (GameManager.Instance.IsPaused) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnBoost();
        }
    }

    void SpawnBoost()
    {
        float y;
        do {
            y = Random.Range(-spawnRangeY, spawnRangeY);
        } while (Mathf.Abs(y - lastY) < 1f);

        lastY = y;
        Instantiate(boostPrefab, new Vector3(spawnX, y, 0f), Quaternion.identity);
    }
}