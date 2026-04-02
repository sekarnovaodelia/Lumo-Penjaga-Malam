using UnityEngine;
using System.Collections;
using LootLocker.Requests;

public class LootLockerSession : MonoBehaviour
{
    public static LootLockerSession Instance;

    public bool IsSessionActive { get; private set; } = false;
    public bool IsConnecting    { get; private set; } = false;

    public event System.Action OnSessionSuccess;
    public event System.Action OnSessionFailed;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("leaderboardEnabled", 0) == 1)
            StartCoroutine(StartSession());
    }

    public IEnumerator StartSession()
    {
        IsConnecting    = false;
        IsSessionActive = false;
        IsConnecting    = true;

        string playerName = PlayerPrefs.GetString("playerName", "Player");
        bool done    = false;
        bool success = false;

        Debug.Log("[LootLocker] Mencoba connect sebagai: " + playerName);

        LootLockerSDKManager.StartGuestSession(playerName, (response) =>
        {
            IsConnecting = false;
            if (response.success)
            {
                IsSessionActive = true;
                success = true;
                Debug.Log("[LootLocker] Berhasil! Player ID: " + response.player_id);
                // Nama disimpan via metadata saat submit score — tidak perlu SetPlayerName
            }
            else
            {
                IsSessionActive = false;
                Debug.LogError("[LootLocker] Gagal: " + response.errorData);
            }
            done = true;
        });

        yield return new WaitUntil(() => done);

        if (success) OnSessionSuccess?.Invoke();
        else         OnSessionFailed?.Invoke();
    }

    public void EndSession()
    {
        IsSessionActive = false;
        IsConnecting    = false;
    }
}