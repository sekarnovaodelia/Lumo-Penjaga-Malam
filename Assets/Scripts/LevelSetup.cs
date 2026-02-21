using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    public GameManager.ObjectiveType objectiveType;
    public int targetAmount;

    void Start()
    {
        GameManager.Instance.SetObjective(objectiveType, targetAmount);
    }
}
