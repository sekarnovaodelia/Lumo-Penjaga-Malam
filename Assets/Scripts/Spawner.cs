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
    public bool isLevel5 = false;           // Aktifkan dari GameManager/LevelSetup
    [Range(0f, 1f)] public float movingPipeChance = 0.5f; // 50% pipa bergerak
    public float liftAmplitude = 1.5f;
    public float liftSpeed = 0.8f;

    [Header("Pickup Settings")]
public GameObject ammoPickupPrefab;
public GameObject heartPickupPrefab;

[Range(0f,1f)] public float pickupChance = 0.3f;
[Range(0f,1f)] public float heartChance = 0.5f; 

    private void OnEnable()
    {
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
        level = GameManager.Instance.CurrentLevel;
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Spawn));
    }

    private void Spawn()
{
    Pipes chosen = Random.value < cannonChance ? cannonPipe : normalPipe;
    Pipes pipes = Instantiate(chosen, transform.position, Quaternion.identity);

    pipes.transform.position += Vector3.up * Random.Range(minHeight, maxHeight);
    pipes.gap = verticalGap;

    // ================= LIFT LOGIC (Level 5) =================
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

    if (Random.value < pickupChance)
{
    GameObject prefabToSpawn = null;

    // LEVEL 7+: Heart & Ammo
    if (level >= 7)
    {
        if (Random.value < heartChance)
            prefabToSpawn = heartPickupPrefab;
        else
            prefabToSpawn = ammoPickupPrefab;
    }
    // LEVEL 4-6: Heart Only
    else if (level >= 4)
    {
        prefabToSpawn = heartPickupPrefab;
    }

    if (prefabToSpawn != null)
    {
        GameObject pickup = Instantiate(
            prefabToSpawn,
            pipes.GetGapCenter(),
            Quaternion.identity
        );

        pickup.transform.SetParent(pipes.transform);
    }
}
}

    public void StartSpawning()
    {
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
        {
            Destroy(pipe.gameObject);
        }
    }
}