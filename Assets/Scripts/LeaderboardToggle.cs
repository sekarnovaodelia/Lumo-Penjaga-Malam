using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class LeaderboardToggle : MonoBehaviour
{
    [Header("Toggle")]
    public Toggle toggle;

    [Header("Visual Toggle (2 Image)")]
    public GameObject imageOn;
    public GameObject imageOff;

    [Header("Spinner")]
    public RectTransform spinnerImage;
    public float spinSpeed = 300f;

    [Header("Error Text")]
    public TMP_Text errorText; // muncul saat gagal connect atau gagal off
    public float errorDuration = 2f; // berapa detik teks error tampil

    private bool isConnecting = false;
    private Coroutine spinCoroutine;
    private Coroutine errorCoroutine;

    void Start()
    {
        if (spinnerImage != null) spinnerImage.gameObject.SetActive(false);
        if (errorText    != null) errorText.gameObject.SetActive(false);

        if (LootLockerSession.Instance != null)
        {
            LootLockerSession.Instance.OnSessionSuccess += OnConnectSuccess;
            LootLockerSession.Instance.OnSessionFailed  += OnConnectFailed;
        }

        toggle.onValueChanged.AddListener(OnToggleClicked);

        if (LootLockerSession.Instance != null && LootLockerSession.Instance.IsSessionActive)
        {
            toggle.SetIsOnWithoutNotify(true);
            UpdateVisual(true);
        }
        else if (LootLockerSession.Instance != null && LootLockerSession.Instance.IsConnecting)
        {
            toggle.SetIsOnWithoutNotify(false);
            UpdateVisual(false);
            isConnecting = true;
            StartSpinner();
        }
        else
        {
            toggle.SetIsOnWithoutNotify(false);
            UpdateVisual(false);
        }
    }

    void OnDestroy()
    {
        if (LootLockerSession.Instance != null)
        {
            LootLockerSession.Instance.OnSessionSuccess -= OnConnectSuccess;
            LootLockerSession.Instance.OnSessionFailed  -= OnConnectFailed;
        }
    }

    // ================= TOGGLE CLICKED =================

    void OnToggleClicked(bool isOn)
    {
        if (isConnecting)
        {
            toggle.SetIsOnWithoutNotify(false);
            UpdateVisual(false);
            return;
        }

        if (isOn)
        {
            // Mau ON → paksa OFF dulu, spinner, lalu connect
            toggle.SetIsOnWithoutNotify(false);
            UpdateVisual(false);
            HideError();
            StartCoroutine(ConnectRoutine());
        }
        else
        {
            // Mau OFF → spinner dulu, coba disconnect
            StartCoroutine(DisconnectRoutine());
        }
    }

    // ================= CONNECT ROUTINE =================

    IEnumerator ConnectRoutine()
    {
        isConnecting = true;
        StartSpinner();

        bool internet = false;
        yield return StartCoroutine(CheckInternet(result => internet = result));

        if (!internet)
        {
            isConnecting = false;
            StopSpinner();
            ShowError("Tidak ada koneksi internet");
            yield break;
        }

        if (LootLockerSession.Instance != null)
            yield return StartCoroutine(LootLockerSession.Instance.StartSession());
        else
        {
            isConnecting = false;
            StopSpinner();
            ShowError("Gagal terhubung ke server");
        }
    }

    // ================= DISCONNECT ROUTINE =================

    IEnumerator DisconnectRoutine()
    {
        isConnecting = true;
        StartSpinner();

        // Simulasi proses disconnect (1 frame cukup)
        yield return null;

        bool success = true;
        try
        {
            LootLockerSession.Instance?.EndSession();
            PlayerPrefs.SetInt("leaderboardEnabled", 0);
            PlayerPrefs.Save();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[LeaderboardToggle] Gagal disconnect: " + e.Message);
            success = false;
        }

        isConnecting = false;
        StopSpinner();

        if (success)
        {
            toggle.SetIsOnWithoutNotify(false);
            UpdateVisual(false);
        }
        else
        {
            // Gagal off → balik ON, tampilkan error
            toggle.SetIsOnWithoutNotify(true);
            UpdateVisual(true);
            ShowError("Gagal menonaktifkan");
        }
    }

    // ================= INTERNET CHECK =================

    IEnumerator CheckInternet(System.Action<bool> callback)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            callback(false);
            yield break;
        }

        UnityWebRequest ping = UnityWebRequest.Head("https://www.google.com");
        ping.timeout = 5;
        yield return ping.SendWebRequest();
        callback(ping.result == UnityWebRequest.Result.Success);
        ping.Dispose();
    }

    // ================= CALLBACKS =================

    void OnConnectSuccess()
    {
        isConnecting = false;
        StopSpinner();
        HideError();
        toggle.SetIsOnWithoutNotify(true);
        UpdateVisual(true);
        PlayerPrefs.SetInt("leaderboardEnabled", 1);
        PlayerPrefs.Save();
    }

    void OnConnectFailed()
    {
        isConnecting = false;
        StopSpinner();
        toggle.SetIsOnWithoutNotify(false);
        UpdateVisual(false);
        PlayerPrefs.SetInt("leaderboardEnabled", 0);
        PlayerPrefs.Save();
        ShowError("Gagal terhubung ke server");
    }

    // ================= VISUAL =================

    void UpdateVisual(bool isOn)
    {
        if (imageOn  != null) imageOn.SetActive(isOn);
        if (imageOff != null) imageOff.SetActive(!isOn);
    }

    // ================= ERROR TEXT =================

    void ShowError(string msg)
    {
        if (errorText == null) return;
        errorText.text = msg;
        errorText.gameObject.SetActive(true);

        if (errorCoroutine != null) StopCoroutine(errorCoroutine);
        errorCoroutine = StartCoroutine(HideErrorAfterDelay());
    }

    void HideError()
    {
        if (errorCoroutine != null) { StopCoroutine(errorCoroutine); errorCoroutine = null; }
        if (errorText != null) errorText.gameObject.SetActive(false);
    }

    IEnumerator HideErrorAfterDelay()
    {
        yield return new WaitForSecondsRealtime(errorDuration);
        if (errorText != null) errorText.gameObject.SetActive(false);
    }

    // ================= SPINNER =================

    void StartSpinner()
    {
        if (spinnerImage == null) return;
        spinnerImage.gameObject.SetActive(true);
        if (spinCoroutine != null) StopCoroutine(spinCoroutine);
        spinCoroutine = StartCoroutine(SpinRoutine());
    }

    void StopSpinner()
    {
        if (spinCoroutine != null) { StopCoroutine(spinCoroutine); spinCoroutine = null; }
        if (spinnerImage  != null) spinnerImage.gameObject.SetActive(false);
    }

    IEnumerator SpinRoutine()
    {
        while (true)
        {
            spinnerImage.Rotate(0f, 0f, -spinSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
    }
}