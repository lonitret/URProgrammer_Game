using TMPro;
using UnityEngine;

public class ResultScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI summaryText;
    [SerializeField] private TextMeshProUGUI reputationText;
    [SerializeField] private TextMeshProUGUI angerText;
    [SerializeField] private TextMeshProUGUI questsText;
    [SerializeField] private TextMeshProUGUI timeText;

    private CanvasGroup panelCanvasGroup;

    private void Awake()
    {
        if (panel == null)
        {
            panel = gameObject;
        }

        panelCanvasGroup = panel.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = panel.AddComponent<CanvasGroup>();
        }

        SetVisible(false);
    }

    private void OnEnable()
    {
        GameManager.OnRunEnded += ShowResult;
    }

    private void OnDisable()
    {
        GameManager.OnRunEnded -= ShowResult;
    }

    private void ShowResult(GameManager.RunResult result)
    {
        SetVisible(true);

        if (titleText != null) titleText.text = result.Message;
        if (summaryText != null) summaryText.text = BuildSummary(result);
        if (reputationText != null) reputationText.text = $"Репутация: {result.Reputation}";
        if (angerText != null) angerText.text = $"Стресс: {result.Anger:0} / {result.MaxAnger:0}";
        if (questsText != null) questsText.text = $"Квесты: {result.CompletedQuests} / {result.RequiredQuests}";
        if (timeText != null) timeText.text = $"Время: {result.Hour:00}:{result.Minute:00}";
    }

    private string BuildSummary(GameManager.RunResult result)
    {
        switch (result.Reason)
        {
            case GameManager.RunEndReason.LevelComplete:
                return "Рабочие задачи закрыты. Можно выдохнуть.";

            case GameManager.RunEndReason.GameOver:
                return "Стресс дошел до предела. День пошел не по плану.";

            case GameManager.RunEndReason.WorkDayEnded:
                return "Рабочий день закончился. Не все задачи успели дождаться героя.";

            default:
                return "";
        }
    }

    private void SetVisible(bool isVisible)
    {
        if (panelCanvasGroup == null) return;

        panelCanvasGroup.alpha = isVisible ? 1f : 0f;
        panelCanvasGroup.interactable = isVisible;
        panelCanvasGroup.blocksRaycasts = isVisible;
    }
}
