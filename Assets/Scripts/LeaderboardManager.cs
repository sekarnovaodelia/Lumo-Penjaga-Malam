using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using LootLocker.Requests;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    [Header("UI")]
    public GameObject leaderboardPanel;
    public GameObject loadingIndicator;
    public TMP_Text errorText;
    public ScrollRect scrollRect;

    [Header("Spinner")]
    public RectTransform spinnerImage;
    public float spinSpeed = 600f;

    [Header("Tab Buttons")]
    public Button btnTabPass;
    public Button btnTabAvoid;
    public Button btnTabDestroy;

    [Header("Entry")]
    public GameObject entryPrefab;
    public Transform entryContainer;
    public float entryHeight = 80f;
    public float entrySpacing = 10f;

    [Header("Tab Colors")]
    public Color activeTabColor   = new Color(1f, 0.8f, 0f);
    public Color inactiveTabColor = new Color(0.5f, 0.5f, 0.5f);

    private List<GameObject> spawnedEntries = new List<GameObject>();
    private Coroutine spinCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
        if (loadingIndicator != null) loadingIndicator.SetActive(false);
        if (spinnerImage     != null) spinnerImage.gameObject.SetActive(false);
        if (errorText        != null) errorText.gameObject.SetActive(false);

        if (scrollRect == null && entryContainer != null)
            scrollRect = entryContainer.GetComponentInParent<ScrollRect>();

        if (LeaderboardService.Instance == null)
        {
            Debug.LogError("[LeaderboardManager] LeaderboardService.Instance NULL!");
            return;
        }

        btnTabPass.onClick.AddListener(()    => SwitchTab(LeaderboardService.Instance.leaderboardKeyPass,    btnTabPass));
        btnTabAvoid.onClick.AddListener(()   => SwitchTab(LeaderboardService.Instance.leaderboardKeyAvoid,   btnTabAvoid));
        btnTabDestroy.onClick.AddListener(() => SwitchTab(LeaderboardService.Instance.leaderboardKeyDestroy, btnTabDestroy));
    }

    public void OpenLeaderboard()
    {
        leaderboardPanel.SetActive(true);

        if (PlayerPrefs.GetInt("leaderboardEnabled", 0) == 0)
        {
            ShowError("Leaderboard dinonaktifkan.\nAktifkan Online Mode dulu.");
            return;
        }

        if (LootLockerSession.Instance == null || !LootLockerSession.Instance.IsSessionActive)
        {
            ShowError("Tidak terhubung ke server.\nAktifkan Online Mode dulu.");
            return;
        }

        HideError();
        SwitchTab(LeaderboardService.Instance.leaderboardKeyPass, btnTabPass);
    }

    public void CloseLeaderboard()
    {
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
        StopSpinner();
    }

    void SwitchTab(string key, Button activeBtn)
    {
        SetTabColor(btnTabPass,    btnTabPass    == activeBtn);
        SetTabColor(btnTabAvoid,   btnTabAvoid   == activeBtn);
        SetTabColor(btnTabDestroy, btnTabDestroy == activeBtn);
        LoadLeaderboard(key);
    }

    void SetTabColor(Button btn, bool isActive)
    {
        var colors = btn.colors;
        colors.normalColor = isActive ? activeTabColor : inactiveTabColor;
        btn.colors = colors;
    }

    void LoadLeaderboard(string key)
    {
        ClearEntries();
        HideError();
        StartSpinner();

        LeaderboardService.Instance.GetScoreList(key, 10, (response) =>
        {
            StopSpinner();

            if (response == null || !response.success) { ShowError("Gagal memuat data."); return; }
            if (response.items == null || response.items.Length == 0) { ShowError("Belum ada skor."); return; }

            int rank = 1;
            foreach (var item in response.items)
            {
                string displayName = !string.IsNullOrEmpty(item.metadata) ? item.metadata : item.member_id;
                SpawnEntry(rank, displayName, item.score);
                rank++;
            }

            ResizeContainer();
            StartCoroutine(ResetScrollNextFrame());
        });
    }

    void ResizeContainer()
    {
        if (entryContainer == null) return;
        int count = spawnedEntries.Count;
        float totalHeight = count * entryHeight + (count - 1) * entrySpacing;
        RectTransform rt = entryContainer as RectTransform;
        if (rt != null) rt.sizeDelta = new Vector2(rt.sizeDelta.x, totalHeight);
    }

    IEnumerator ResetScrollNextFrame()
    {
        yield return null;
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    void ClearEntries()
    {
        foreach (GameObject go in spawnedEntries)
            if (go != null) Destroy(go);
        spawnedEntries.Clear();
    }

    void SpawnEntry(int rank, string playerName, int score)
    {
        if (entryPrefab == null || entryContainer == null) return;

        GameObject entry = Instantiate(entryPrefab, entryContainer);
        entry.SetActive(true);

        RectTransform rt = entry.GetComponent<RectTransform>();
        if (rt != null)
        {
            float yPos = -85f - (rank - 1) * 65f;
            rt.anchorMin        = new Vector2(0, 1);
            rt.anchorMax        = new Vector2(1, 1);
            rt.pivot            = new Vector2(0.5f, 0.5f);
            rt.offsetMin        = new Vector2(0, rt.offsetMin.y);
            rt.offsetMax        = new Vector2(0, rt.offsetMax.y);
            rt.sizeDelta        = new Vector2(rt.sizeDelta.x, entryHeight);
            rt.anchoredPosition = new Vector2(0, yPos);
        }

        spawnedEntries.Add(entry);

        LeaderboardEntry le = entry.GetComponent<LeaderboardEntry>();
        if (le != null) le.SetData(rank, playerName, score);
    }

    // ================= SPINNER =================

    void StartSpinner()
    {
        if (loadingIndicator != null) loadingIndicator.SetActive(true);
        if (spinnerImage     != null) spinnerImage.gameObject.SetActive(true);
        if (spinCoroutine    != null) StopCoroutine(spinCoroutine);
        if (spinnerImage     != null) spinCoroutine = StartCoroutine(SpinRoutine());
    }

    void StopSpinner()
    {
        if (spinCoroutine    != null) { StopCoroutine(spinCoroutine); spinCoroutine = null; }
        if (spinnerImage     != null) spinnerImage.gameObject.SetActive(false);
        if (loadingIndicator != null) loadingIndicator.SetActive(false);
    }

    IEnumerator SpinRoutine()
    {
        while (true)
        {
            spinnerImage.Rotate(0f, 0f, -spinSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
    }

    // ================= ERROR =================

    void ShowError(string msg)
    {
        ClearEntries();
        if (errorText == null) return;
        errorText.text = msg;
        errorText.gameObject.SetActive(true);
    }

    void HideError()
    {
        if (errorText != null) errorText.gameObject.SetActive(false);
    }
}