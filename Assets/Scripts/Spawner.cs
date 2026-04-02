using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Pipes normalPipe;
    public Pipes cannonPipe;
    [Range(0f, 1f)] public float cannonChance = 0.3f;
    public float spawnRate = 1f;
    public float minHeight = -1f;
    public float maxHeight = 2f;
    public float verticalGap = 3f;
    int level;

    [Header("Level 5 Lift Settings")]
    public bool isLevel5 = false;
    [Range(0f, 1f)] public float movingPipeChance = 0.5f;
    public float liftAmplitude = 1.5f;
    public float liftSpeed = 0.8f;

    [Header("Pickup Settings")]
    public GameObject ammoPickupPrefab;
    public GameObject heartPickupPrefab;
    [Range(0f, 1f)] public float pickupChance = 0.3f;
    [Range(0f, 1f)] public float heartChance = 0.5f;

    [Header("Free Mode Settings")]
    public bool isFreeMode = false; // centang di scene 11-13

    private int normalPipeCount = 0;
    private int nextCannonAt = 0;
    private bool firstCannonSpawned = false;

    private const int MIN_PIPES = 3;
    private const int MAX_PIPES = 4;

    private void OnEnable()
    {
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
        level = GameManager.Instance.CurrentLevel;
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Spawn));
    }

    private void ResetCycle()
    {
        normalPipeCount = 0;
        nextCannonAt = Random.Range(MIN_PIPES, MAX_PIPES + 1);
    }

    private void Spawn()
    {
        // Level 1-3: tidak ada cannon
        // Level 4-10: cannon normal
        // Free mode (isFreeMode = true): selalu ada cannon
        bool canSpawnCannon = (isFreeMode && level >= 12) || (level >= 4 && level <= 9); // L1-3,10,11=pipe only L4-9=cannon L12-13=cannon

        bool spawnCannon = canSpawnCannon && normalPipeCount >= nextCannonAt;

        Pipes chosen = spawnCannon ? cannonPipe : normalPipe;
        Pipes pipes = Instantiate(chosen, transform.position, Quaternion.identity);
        pipes.transform.position += Vector3.up * Random.Range(minHeight, maxHeight);
        pipes.gap = verticalGap;

        if (spawnCannon)
        {
            firstCannonSpawned = true;
            ResetCycle();
        }
        else
        {
            normalPipeCount++;
        }

        // ——— Lift Logic (Level 5) ———
        if (isLevel5)
        {
            bool shouldMove = Random.value < movingPipeChance;
            pipes.isMovingPipe = shouldMove;
            if (shouldMove)
            {
                pipes.liftAmplitude = liftAmplitude;
                pipes.liftSpeed = liftSpeed;
            }
        }

        // ——— Pickup Logic ———
        bool isHeartPipe = !spawnCannon
                           && firstCannonSpawned
                           && normalPipeCount == nextCannonAt
                           && (level >= 4 || isFreeMode);

        GameObject prefabToSpawn = null;

        if (isHeartPipe && heartPickupPrefab != null)
        {
            prefabToSpawn = heartPickupPrefab;
        }
        else if (!spawnCannon && firstCannonSpawned && Random.value < pickupChance)
        {
            if (level >= 7 || isFreeMode)
                prefabToSpawn = Random.value < 0.25f ? heartPickupPrefab : ammoPickupPrefab;
            else if (level >= 4)
                prefabToSpawn = heartPickupPrefab;
        }

        if (prefabToSpawn != null)
        {
            GameObject pickup = Instantiate(prefabToSpawn, pipes.GetGapCenter(), Quaternion.identity);
            pickup.transform.SetParent(pipes.transform);
        }
    }

    public void StartSpawning()
    {
        firstCannonSpawned = false;
        normalPipeCount    = 0;

        // Free mode selalu ada cannon, level 1-3 tidak ada
        bool hasCannon = (isFreeMode && level >= 12) || (level >= 4 && level <= 9); // L1-3,10,11=pipe only L4-9=cannon L12-13=cannon
        nextCannonAt = hasCannon ? 3 : 999;

        CancelInvoke(nameof(Spawn));
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
    }

    public void StopSpawning()
    {
        CancelInvoke(nameof(Spawn));
    }

    public void ClearPipes()
    {
        Pipes[] pipes = FindObjectsOfType<Pipes>();
        foreach (Pipes pipe in pipes)
            Destroy(pipe.gameObject);
    }
}