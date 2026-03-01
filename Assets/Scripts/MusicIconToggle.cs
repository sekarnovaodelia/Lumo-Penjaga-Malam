using UnityEngine;
using UnityEngine.UI;

public class MusicIconToggle : MonoBehaviour
{
    public Toggle toggle;
    public Image icon;
    public Sprite musicOn;
    public Sprite musicOff;

    void Start()
    {
        // Sync toggle state with saved preference
        int music = PlayerPrefs.GetInt("music", 1);
        toggle.isOn = music == 1;

        UpdateIcon(toggle.isOn);
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool state)
    {
        icon.sprite = state ? musicOn : musicOff;
        AudioManager.Instance.ToggleMusic(state);
    }

    void UpdateIcon(bool state)
    {
        icon.sprite = state ? musicOn : musicOff;
    }
}