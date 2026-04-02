using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSelect : MonoBehaviour
{
    [System.Serializable]
    public class LevelSlot
    {
        public Button button;
        public GameObject lockIcon;
    }

    public LevelSlot[] levels;

    // Index array mulai dari mana free mode (array index 10 = level 11)
    private const int FREE_MODE_START_INDEX = 10;

    IEnumerator Start()
    {
        yield return null;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();

        int unlocked = PlayerPrefs.GetInt("levelUnlocked", 1);
        unlocked = Mathf.Max(1, unlocked);

        Debug.Log("Unlocked = " + unlocked);

        for (int i = 0; i < levels.Length; i++)
        {
            bool isUnlocked;

            if (i >= FREE_MODE_START_INDEX)
            {
                // Level free mode (array index 10-12) unlock semua kalau level 10 sudah clear
                isUnlocked = unlocked > 10;
            }
            else
            {
                isUnlocked = i < unlocked;
            }

            if (levels[i].button != null)
                levels[i].button.interactable = isUnlocked;

            if (levels[i].lockIcon != null)
                levels[i].lockIcon.SetActive(!isUnlocked);
        }
    }

    public void LoadLevel(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void GoToFreeModeSelect()
    {
        SceneManager.LoadScene(15);
    }
    
}