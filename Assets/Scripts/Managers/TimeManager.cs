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
    private bool workDayEnded;
    public static event Action<int, int> OnTimeChanged;
    public static event Action OnWorkDayEnded;
    public int CurrentHour => Mathf.FloorToInt(currentTime / 3600) % 24;
    public int CurrentMinute => Mathf.FloorToInt((currentTime % 3600) / 60);
    void Awake() => instance = this;
    private void Start()
    {
        currentTime = startHour * 3600;
        UpdateUI();
    }
    private void Update()
    {
        if (GameManager.isPaused || GameManager.isGameOver || workDayEnded) return;
        currentTime += Time.deltaTime * timeMultiplyer;
        UpdateUI();
    }
    private void UpdateUI()
    {
        int hours = CurrentHour;
        int minutes = CurrentMinute;
        OnTimeChanged?.Invoke(hours, minutes);
        if (hours >= endHour)
        {
            EndWorkDay();
        }
    }
    private void EndWorkDay()
    {
        if (workDayEnded) return;
        workDayEnded = true;
        Debug.Log("Рабочий день окончен!");
        OnWorkDayEnded?.Invoke();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndWorkDay();
        }
        this.enabled = false;
    }
}
