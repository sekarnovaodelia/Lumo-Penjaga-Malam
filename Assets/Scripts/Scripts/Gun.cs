using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform shootPoint;
    public GameObject bulletPrefab;

    public float aimRange = 20f;

    void Update()
    {
        Aim();
        Shoot();
    }

    void Aim()
    {
        GameObject target = FindClosestCanon();

        if(target == null) return;

        Vector3 dir = target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f,0f,angle);
    }

    GameObject FindClosestCanon()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("canon");

        GameObject closest = null;
        float minDist = aimRange;

        foreach(GameObject e in enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);

            if(dist < minDist)
            {
                minDist = dist;
                closest = e;
            }
        }

        return closest;
    }

    void Shoot()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Instantiate(
                bulletPrefab,
                shootPoint.position,
                shootPoint.rotation
            );
        }
    }
}
