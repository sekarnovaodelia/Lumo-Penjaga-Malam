using UnityEngine;
using TMPro;

/// <summary>
/// Pasang script ini di prefab satu baris leaderboard.
/// Prefab harus punya 3 TMP_Text child: rankText, nameText, scoreText
/// </summary>
public class LeaderboardEntry : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text nameText;
    public TMP_Text scoreText;

    public void SetData(int rank, string playerName, int score)
    {
        if (rankText  != null) rankText.text  = "#" + rank;
        if (nameText  != null) nameText.text  = playerName;
        if (scoreText != null) scoreText.text = score.ToString();
    }
}
