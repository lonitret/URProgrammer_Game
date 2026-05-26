using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    public float currentAnger = 0f;
    public float maxAnger = 100f;
    public int reputation = 0;

    public static event Action<float, float> OnAngerChanged;
    public static event Action<int> OnReputationChanged;

    void Awake() => Instance = this;

    void Start()
    {
        OnAngerChanged?.Invoke(currentAnger, maxAnger);
        OnReputationChanged?.Invoke(reputation);
    }

    public void ChangeAnger(float amount)
    {
        currentAnger = Mathf.Clamp(currentAnger + amount, 0, maxAnger);
        OnAngerChanged?.Invoke(currentAnger, maxAnger);

        if (currentAnger >= maxAnger) GameOver();
    }

    public void ChangeReputation(int amount)
    {
        reputation += amount;
        OnReputationChanged?.Invoke(reputation);
    }

    public void SetAnger(float value)
    {
        currentAnger = Mathf.Clamp(value, 0, maxAnger);
        if (currentAnger >= maxAnger) GameOver();
    }

    private void GameOver()
    {
        Debug.Log("Нервный срыв! Игра окончена.");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogError("GameManager не найден на сцене.");
        }
    }
}
