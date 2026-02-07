using UnityEngine;

public class IllusionEnemyMovement : MonoBehaviour
{
    [SerializeField] private float xOffset = 3f; // Distance to maintain ahead of the player
    [SerializeField] private float smoothSpeed = 2f; // How fast it adjusts Y position
    [SerializeField] private float leaveSpeed = 5f; // Speed when flying away
    
    private Player player;
    private bool isLeaving = false;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (player == null) return;

        // Check if we should leave (Score >= 25)
        if (GameManager.Instance.score >= 25 && !isLeaving)
        {
            isLeaving = true;
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StopChaseMusic();
            }

            Spawner spawner = FindObjectOfType<Spawner>();
            if (spawner != null)
            {
                spawner.StopSpawning();
                spawner.ClearPipes();
            }
        }

        if (isLeaving)
        {
            // Fly to the right
            transform.position += Vector3.right * leaveSpeed * Time.deltaTime;
            
            // Destroy when far off screen
            if (transform.position.x > player.transform.position.x + 20f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // Maintain X position relative to player
            float targetX = player.transform.position.x + xOffset;

            // Find the best Y position (Pipe Gap)
            float targetY = player.transform.position.y; // Default to player Y
            
            Pipes[] pipes = FindObjectsOfType<Pipes>();
            Pipes targetPipe = null;
            float minDistance = float.MaxValue;

            foreach (Pipes pipe in pipes)
            {
                // We refer to pipe position - X.
                // We want the pipe we are currently interacting with or approaching.
                float dist = Mathf.Abs(pipe.transform.position.x - transform.position.x);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    targetPipe = pipe;
                }
            }

            if (targetPipe != null)
            {
                targetY = targetPipe.transform.position.y;
            }

            // Move smoothly towards the target position
            Vector3 targetPosition = new Vector3(targetX, targetY, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
