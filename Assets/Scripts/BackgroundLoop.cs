using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    public float speed = 2f;

    BackgroundLoop otherBg;
    float width;

    void Start()
    {
        width = GetComponent<SpriteRenderer>().bounds.size.x;

        // cari background pasangan
        BackgroundLoop[] all = FindObjectsOfType<BackgroundLoop>();
        foreach (var bg in all)
        {
            if (bg != this)
                otherBg = bg;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // kalau sudah lewat kiri background lain
        if (transform.position.x <= otherBg.transform.position.x - width)
        {
            Vector3 newPos = otherBg.transform.position;
            newPos.x += width;
            transform.position = newPos;
        }
    }
}
