using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 4;

    void OnTriggerEnter2D(Collider2D other)
    {
        Gun gun = other.GetComponentInChildren<Gun>();

        if (gun != null)
        {
            gun.AddAmmo(ammoAmount);
            Destroy(gameObject);
        }
    }
}