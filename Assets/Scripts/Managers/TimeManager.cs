using System;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    [Header("Настройка игрового времени")]
    public float timeMultiplyer = 60f;
    public int startHour = 9;
    public int endHour = 18;

    private float currentTime;

    public static event Action<int, int> OnTimeChanged;
    public static event Action OnWorkDayEnded;

    void Awake() => instance = this;

    private void Start()
    {
        currentTime = startHour * 3600;
    }

    private void Update()
    {
        currentTime += Time.deltaTime * timeMultiplyer;
        UpdateUI();
    }

    private void UpdateUI()
    {
        int hours = Mathf.FloorToInt(currentTime / 3600) % 24;
        int minutes = Mathf.FloorToInt((currentTime % 3600) / 60);

        OnTimeChanged?.Invoke(hours, minutes);

        if (hours >= endHour && (GameManager.Instance == null || !GameManager.isPaused))
        {
            EndWorkDay();
        }
    }

    private void EndWorkDay()
    {
        Debug.Log("Рабочий день окончен!");
        OnWorkDayEnded?.Invoke();
        this.enabled = false;
    }
}
