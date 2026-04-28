using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    [Header("Настройка игрового времени")]
    public float timeMultiplyer = 60f;
    public int startHour = 9;

    private float currentTime;

    [SerializeField] private TextMeshProUGUI timeText;
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

        if (timeText != null)
        {
            timeText.text = string.Format("{0:00}:{1:00}", hours, minutes);
        }

        if (hours >= 18 && GameManager.isPaused == false)
        {
            EndWorkDay();
        }
    }

    private void EndWorkDay()
    {
        Debug.Log("Рабочий день окончен!");
        // потом тут будут итоги дня (уровня)
        // GameManager.Instance.ShowEndDaySummary(); 
    }
}
