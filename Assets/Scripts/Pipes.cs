using UnityEngine;

public class Pipes : MonoBehaviour
{
    public Transform top;
    public Transform bottom;
    public float speed = 5f;
    public float gap = 3f;

    // === Lift Movement ===
    [Header("Lift Movement (Level 5)")]
    public bool isMovingPipe = false;       // Di-set oleh Spawner
    public float liftAmplitude = 1.5f;     // Seberapa jauh naik/turun
    public float liftSpeed = 0.8f;         // Kecepatan sinus (lambat)

    private float leftEdge;
    private float startY;                  // Posisi Y awal untuk referensi sinus

    private void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 1f;
        top.position += Vector3.up * gap / 2;
        bottom.position += Vector3.down * gap / 2;

        startY = transform.position.y;     // Simpan Y awal
    }

    private void Update()
    {
        // Gerak horizontal (selalu)
        transform.position += speed * Time.deltaTime * Vector3.left;

        // Gerak vertikal sinus (hanya jika isMovingPipe = true)
        if (isMovingPipe)
        {
            float newY = startY + Mathf.Sin(Time.time * liftSpeed) * liftAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GetGapCenter()
{
    return (top.position + bottom.position) / 2f;
}
}