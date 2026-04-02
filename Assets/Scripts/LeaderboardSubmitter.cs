using UnityEngine;

public class LeaderboardSubmitter : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver += SubmitOnGameOver;
        else
            Debug.LogError("[Submitter] GameManager.Instance NULL saat Start!");
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver -= SubmitOnGameOver;
    }

    void SubmitOnGameOver()
    {
        try
        {
            if (!GameManager.Instance.IsFreeMode) return;
            if (LeaderboardService.Instance == null)
            {
                Debug.LogWarning("[Submitter] LeaderboardService NULL, skip submit.");
                return;
            }

            int score  = GameManager.Instance.score;
            string key = LeaderboardService.Instance.GetKeyForObjective(GameManager.Instance.CurrentObjective);

            Debug.Log("[Submitter] Submit score: " + score + " | key: " + key);
            LeaderboardService.Instance.SubmitScore(key, score);
        }
        catch (System.Exception e)
        {
            // Error di sini tidak akan blocking GameManager.GameOver()
            Debug.LogWarning("[Submitter] Error saat submit (diabaikan): " + e.Message);
        }
    }
}