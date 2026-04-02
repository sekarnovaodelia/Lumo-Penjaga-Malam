using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject namaPanel;
    public GameObject aboutPanel;

    public TMP_InputField inputNama;

    IEnumerator Start()
    {
        yield return null;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();

        settingsPanel.SetActive(false);

        // cek apakah sudah pernah isi nama
        if (!PlayerPrefs.HasKey("playerName"))
        {
            namaPanel.SetActive(true);
        }
        else
        {
            namaPanel.SetActive(false);
        }
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

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("levelUnlocked");
        PlayerPrefs.Save();
        Debug.Log("Progress level direset.");
    }

    // simpan nama pertama kali
    public void SaveName()
    {
        string name = inputNama.text;

        if (string.IsNullOrEmpty(name))
            return;

        PlayerPrefs.SetString("playerName", name);
        PlayerPrefs.Save();

        namaPanel.SetActive(false);
    }

    // buka panel ubah nama dari tombol akun
    public void OpenNamePanel()
    {
        namaPanel.SetActive(true);

        // isi textbox dengan nama lama
        inputNama.text = PlayerPrefs.GetString("playerName", "");
    }

        public void CloseNamaPanel()
    {
        namaPanel.SetActive(false);
    }

    public void OpenAboutPanel()
    {
        aboutPanel.SetActive(true);
    }

    public void CloseAboutPanel()
    {
        aboutPanel.SetActive(false);
    }   
}