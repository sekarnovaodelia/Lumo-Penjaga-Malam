using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    [System.Serializable]
    public class LevelSlot
    {
        public Button button;
        public GameObject lockIcon;
    }

    public LevelSlot[] levels;

    void Start()
    {
        int unlocked = PlayerPrefs.GetInt("levelUnlocked", 1);
        unlocked = Mathf.Max(1, unlocked);

        Debug.Log("Unlocked = " + unlocked);

        for (int i = 0; i < levels.Length; i++)
        {
            bool isUnlocked = i < unlocked;

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
}