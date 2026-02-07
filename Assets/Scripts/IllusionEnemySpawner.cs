using UnityEngine;
public class IllusionEnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnDistanceX = 5f; // Distance in front of the player
    [SerializeField] private float spawnY = 0f;         // Vertical spawn position
    
    private void Start()
    {
        // Subscribe to events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlay += ResetSpawner;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlay -= ResetSpawner;
        }
    }

    private void ResetSpawner()
    {
        // Clean up any remaining illusion enemies (if any)
        IllusionEnemyMovement[] enemies = FindObjectsOfType<IllusionEnemyMovement>();
        foreach (var enemy in enemies) {
            Destroy(enemy.gameObject);
        }

        SpawnEnemy();
    }
    private void SpawnEnemy()
    {
        Vector3 spawnPos = Vector3.zero;
        
        // Find the player to determine relative spawn position
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            spawnPos = player.transform.position;
            spawnPos.x += spawnDistanceX;
        }
        // Align Y with the nearest pipe's gap
        Pipes[] allPipes = FindObjectsOfType<Pipes>();
        Pipes nearestPipe = null;
        float minDistance = float.MaxValue;
        foreach (Pipes pipe in allPipes)
        {
            float distance = Mathf.Abs(pipe.transform.position.x - spawnPos.x);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPipe = pipe;
            }
        }
        if (nearestPipe != null)
        {
            spawnPos.y = nearestPipe.transform.position.y;
        }
        else
        {
            spawnPos.y = spawnY; // Fallback if no pipes found
        }
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        
        // Start Chase Music
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartChaseMusic();
        }
    }
}