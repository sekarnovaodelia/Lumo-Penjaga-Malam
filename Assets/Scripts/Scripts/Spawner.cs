using UnityEngine;
public class Spawner : MonoBehaviour
{
    public Pipes normalPipe;
    public Pipes cannonPipe;
    [Range(0f,1f)] public float cannonChance = 0.3f;
    public float spawnRate = 1f;
    public float minHeight = -1f;
    public float maxHeight = 2f;
    public float verticalGap = 3f;
    private void OnEnable()
    {
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
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
}


    public void StartSpawning()
    {
        CancelInvoke(nameof(Spawn)); // Ensure no duplicate invokes
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
    }

    public void StopSpawning()
    {
        CancelInvoke(nameof(Spawn));
    }

    public void ClearPipes()
    {
        Pipes[] pipes = FindObjectsOfType<Pipes>();
        foreach (Pipes pipe in pipes) {
            Destroy(pipe.gameObject);
        }
    }
}
