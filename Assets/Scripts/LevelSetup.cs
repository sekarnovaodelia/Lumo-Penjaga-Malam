using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    public enum FreeModeType
    {
        PassTurret,
        AvoidCannon,
        DestroyCannon
    }

    [Header("Objective")]
    public bool freeMode = false;

    [Header("Free Mode Settings (aktif jika freeMode = true)")]
    public FreeModeType freeModeType = FreeModeType.PassTurret;

    [Header("Normal Mode Settings (aktif jika freeMode = false)")]
    public GameManager.ObjectiveType objectiveType;
    public int targetAmount;

    void Start()
    {
        if (freeMode)
        {
            GameManager.ObjectiveType type = freeModeType switch
            {
                FreeModeType.PassTurret    => GameManager.ObjectiveType.PassTurret,
                FreeModeType.AvoidCannon   => GameManager.ObjectiveType.AvoidCannon,
                FreeModeType.DestroyCannon => GameManager.ObjectiveType.DestroyCannon,
                _                          => GameManager.ObjectiveType.PassTurret
            };

            GameManager.Instance.SetFreeMode(true);
            GameManager.Instance.SetObjective(type, int.MaxValue);
            return;
        }

        GameManager.Instance.SetFreeMode(false);
        GameManager.Instance.SetObjective(objectiveType, targetAmount);
    }
}