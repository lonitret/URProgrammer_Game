using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIController : MonoBehaviour
{
    [Header("Anger UI")]
    public Slider angerSlider;
    public Image dangerOverlay;

    [Header("Stats UI")]
    public TextMeshProUGUI reputationText;
    public TextMeshProUGUI questText;

    [Header("Time UI")]
    public TextMeshProUGUI timeText;

    void OnEnable()
    {
        StatsManager.OnAngerChanged += UpdateAnger;
        StatsManager.OnReputationChanged += UpdateReputation;
        QuestManager.OnQuestUpdated += UpdateQuest;
        TimeManager.OnTimeChanged += UpdateTimeDisplay;
    }

    void OnDisable()
    {
        StatsManager.OnAngerChanged -= UpdateAnger;
        StatsManager.OnReputationChanged -= UpdateReputation;
        QuestManager.OnQuestUpdated -= UpdateQuest;
        TimeManager.OnTimeChanged -= UpdateTimeDisplay;
    }

    private void UpdateAnger(float current, float max)
    {
        if (angerSlider) angerSlider.value = current / max;
        if (dangerOverlay)
        {
            var c = dangerOverlay.color;
            c.a = current / max;
            dangerOverlay.color = c;
        }
    }

    private void UpdateReputation(int amount) => reputationText.text = $"Репутация: {amount}";
    private void UpdateQuest(string desc) => questText.text = desc;

    private void UpdateTimeDisplay(int hours, int minutes)
    {
        if (timeText != null)
        {
            timeText.text = string.Format("{0:00}:{1:00}", hours, minutes);
        }
    }
}