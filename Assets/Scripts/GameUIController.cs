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
        QuestManager.OnQuestStateChanged += UpdateQuest;
        TimeManager.OnTimeChanged += UpdateTimeDisplay;
    }

    void OnDisable()
    {
        StatsManager.OnAngerChanged -= UpdateAnger;
        StatsManager.OnReputationChanged -= UpdateReputation;
        QuestManager.OnQuestStateChanged -= UpdateQuest;
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

    private void UpdateReputation(int amount)
    {
        if (reputationText != null)
        {
            reputationText.text = $"Репутация: {amount}";
        }
    }

    private void UpdateQuest(QuestManager.QuestUiState state)
    {
        if (questText == null) return;

        if (!state.HasActiveQuest)
        {
            questText.text = $"Нет активных задач\n{state.CurrentStep}";
            return;
        }

        questText.text =
            $"{state.Title}\n" +
            $"\nОт: {state.GiverName}\n" +
            $"\nСтатус: {state.Status}\n" +
            $"\nШаг: {state.CurrentStep}\n" +
            $"\nПрогресс: {state.CompletedSteps}/{state.TotalSteps}";
    }

    private void UpdateTimeDisplay(int hours, int minutes)
    {
        if (timeText != null)
        {
            timeText.text = string.Format("{0:00}:{1:00}", hours, minutes);
        }
    }
}
