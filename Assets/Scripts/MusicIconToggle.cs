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
        UpdateIcon(toggle.isOn);
        toggle.onValueChanged.AddListener(UpdateIcon);
    }

    void UpdateIcon(bool state)
    {
        icon.sprite = state ? musicOn : musicOff;
    }
}