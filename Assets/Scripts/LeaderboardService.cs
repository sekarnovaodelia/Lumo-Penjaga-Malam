using UnityEngine;
using LootLocker.Requests;

/// <summary>
/// Logic only — DontDestroyOnLoad dari MainMenu.
/// </summary>
public class LeaderboardService : MonoBehaviour
{
    public static LeaderboardService Instance;

    [Header("Leaderboard Keys")]
    public string leaderboardKeyPass    = "leaderboard_pass";
    public string leaderboardKeyAvoid   = "leaderboard_avoid";
    public string leaderboardKeyDestroy = "leaderboard_destroy";

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    // ================= SUBMIT — kirim nama sebagai metadata =================

    public void SubmitScore(string key, int score)
    {
        if (PlayerPrefs.GetInt("leaderboardEnabled", 0) == 0) return;
        if (LootLockerSession.Instance == null || !LootLockerSession.Instance.IsSessionActive)
        {
            Debug.LogWarning("[LeaderboardService] Session tidak aktif, skip submit.");
            return;
        }

        // metadata = nama player, tampil di leaderboard
        string playerName = PlayerPrefs.GetString("playerName", "Player");

        // Buat request langsung dengan metadata — lebih reliable daripada pakai overload SDK
        var request = new LootLocker.Requests.LootLockerSubmitScoreRequest
        {
            member_id = "",
            score     = score,
            metadata  = playerName
        };

        LootLocker.LootLockerAPIManager.SubmitScore("", request, key, (response) =>
        {
            if (response.success)
                Debug.Log("[LeaderboardService] Score " + score + " submit ke " + key + " | metadata: " + playerName);
            else
                Debug.LogError("[LeaderboardService] Gagal submit: " + response.errorData);
        });
    }

    public string GetKeyForObjective(GameManager.ObjectiveType type)
    {
        return type switch
        {
            GameManager.ObjectiveType.PassTurret    => leaderboardKeyPass,
            GameManager.ObjectiveType.AvoidCannon   => leaderboardKeyAvoid,
            GameManager.ObjectiveType.DestroyCannon => leaderboardKeyDestroy,
            _                                       => leaderboardKeyPass
        };
    }

    // ================= GET SCORES =================

    public void GetScoreList(string key, int count, System.Action<LootLockerGetScoreListResponse> callback)
    {
        if (LootLockerSession.Instance == null || !LootLockerSession.Instance.IsSessionActive)
        {
            callback?.Invoke(null);
            return;
        }

        LootLockerSDKManager.GetScoreList(key, count, 0, callback);
    }
}