using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel;

    IEnumerator Start()
    {
        yield return null;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();

        settingsPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>Hapus semua progress level. Pasang ke button Reset Progress.</summary>
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("levelUnlocked");
        PlayerPrefs.Save();
        Debug.Log("Progress level direset.");
    }
}