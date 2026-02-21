using UnityEngine;

public class AudioBootstrap : MonoBehaviour
{
    [SerializeField] private AudioManager audioPrefab;

    void Awake()
    {
        if (AudioManager.Instance == null)
        {
            Instantiate(audioPrefab);
        }
    }
}