using UnityEngine;

public class DarkChildController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private GameObject obstaclePrefab; // Falling shadow/rock
    
    private Vector3 targetPosition;
    private bool isMoving = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
             // Optional: Add Rigidbody2D if not present, or just move via Transform for simple cutscenes
             // gameObject.AddComponent<Rigidbody2D>(); 
             // rb = GetComponent<Rigidbody2D>();
             // rb.gravityScale = 0; 
        }
    }

    public void MoveTo(Vector3 position, float speed = -1)
    {
        targetPosition = position;
        if (speed > 0) moveSpeed = speed;
        isMoving = true;
    }

    public void Jump()
    {
        if (rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
        else
        {
            // Simple jump simulation if no RB
            StartCoroutine(SimpleJumpRoutine());
        }
    }

    private System.Collections.IEnumerator SimpleJumpRoutine()
    {
        float startY = transform.position.y;
        float peakY = startY + 2f;
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Parabola approximation
            float yOffset = 4f * (peakY - startY) * (t - t*t); 
            // This is just a visual bounce, combining with horizontal movement might need better logic
            // For now, let's just move Up
            transform.position = new Vector3(transform.position.x, startY + Mathf.Sin(t * Mathf.PI) * 2f, transform.position.z);
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, startY, transform.position.z);
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
    }

    public void DropObstacle()
    {
        if (obstaclePrefab != null)
        {
            Instantiate(obstaclePrefab, transform.position, Quaternion.identity);
        }
    }

    public void SetVisible(bool visible)
    {
        GetComponent<Renderer>().enabled = visible;
        // Also handle children if any (e.g. particle systems)
        foreach(Renderer r in GetComponentsInChildren<Renderer>()) {
            r.enabled = visible;
        }
    }
}
