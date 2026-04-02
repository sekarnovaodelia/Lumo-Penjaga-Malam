using UnityEngine;

/// <summary>
/// Pasang di GameObject LootLockerLifecycleManager di MainMenu.
/// Supaya tidak destroy saat scene berganti.
/// </summary>
public class LootLockerPersist : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
