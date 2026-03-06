using UnityEngine;

/// <summary>
/// Taruh script ini di GameObject kosong di setiap scene level.
/// Cannon akan otomatis baca nilai dari sini.
/// Kalau tidak ada script ini di scene, Cannon pakai nilai hardcode per level.
/// </summary>
public class LevelCannonSettings : MonoBehaviour
{
    [Header("Cannon Settings untuk Level ini")]
    public float fireDelay   = 2f;    // jeda antar tembakan (detik)
    public float bulletSpeed = 20f;   // kecepatan peluru
    public int   burstCount  = 1;     // jumlah peluru per tembakan
    public float spreadAngle = 10f;   // sudut spread antar peluru (kalau burst > 1)
}