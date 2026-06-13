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

        if (titleText != null) titleText.text = BuildTitle(result);
        if (summaryText != null) summaryText.text = BuildSummary(result);
        if (reputationText != null) reputationText.text = $"Репутация: {result.Reputation}";
        if (angerText != null) angerText.text = $"Стресс: {result.Anger:0} / {result.MaxAnger:0}";
        if (questsText != null) questsText.text = $"Квесты: {result.CompletedQuests} / {result.RequiredQuests}";
        if (timeText != null) timeText.text = $"Время: {result.Hour:00}:{result.Minute:00}";
    }

    private string BuildTitle(GameManager.RunResult result)
    {
        switch (result.Reason)
        {
            case GameManager.RunEndReason.LevelComplete:
                return $"{result.Message}\n{BuildWorkdayGrade(result)}";

            case GameManager.RunEndReason.GameOver:
                return "Нервный срыв";

            case GameManager.RunEndReason.WorkDayEnded:
                return "Рабочий день окончен";

            default:
                return result.Message;
        }
    }

    private string BuildSummary(GameManager.RunResult result)
    {
        switch (result.Reason)
        {
            case GameManager.RunEndReason.LevelComplete:
                return BuildLevelCompleteSummary(result);

            case GameManager.RunEndReason.GameOver:
                return "Стресс дошел до предела. Коллеги явно просили слишком много, а кофе было слишком мало.";

            case GameManager.RunEndReason.WorkDayEnded:
                return "Рабочий день закончился раньше, чем все задачи были закрыты. Завтра офис снова вспомнит, что ты же программист.";

            default:
                return "";
        }
    }

    private string BuildLevelCompleteSummary(GameManager.RunResult result)
    {
        string stressComment = GetStressComment(result);
        string reputationComment = GetReputationComment(result);
        string timeComment = GetTimeComment(result);

        return $"{stressComment}\n{reputationComment}\n{timeComment}";
    }

    private string BuildWorkdayGrade(GameManager.RunResult result)
    {
        int score = CalculateScore(result);

        if (score >= 90) return "Оценка дня:\nОтличный специалист";
        if (score >= 70) return "Оценка дня:\nНадежный коллега";
        if (score >= 50) return "Оценка дня:\nДень пережит";

        return "Оценка дня:\nНужен отпуск";
    }

    private int CalculateScore(GameManager.RunResult result)
    {
        int score = 40;
        score += Mathf.Clamp(result.CompletedQuests * 15, 0, 45);
        score += Mathf.Clamp(result.Reputation, 0, 30);
        score -= Mathf.RoundToInt(result.Anger * 0.4f);

        return Mathf.Clamp(score, 0, 100);
    }

    private string GetStressComment(GameManager.RunResult result)
    {
        float angerPercent = result.MaxAnger > 0f ? result.Anger / result.MaxAnger : 0f;

        if (angerPercent <= 0.25f) return "Стресс под контролем:\nты почти выглядишь как человек, который выспался.";
        if (angerPercent <= 0.55f) return "Стресс заметен, но рабочий день не победил тебя окончательно.";

        return "Стресс высокий:\nеще один вопрос про принтер, и монитор мог бы не выжить.";
    }

    private string GetReputationComment(GameManager.RunResult result)
    {
        if (result.Reputation >= 40) return "Репутация выросла:\nколлеги теперь уверены, что ты умеешь вообще все.";
        if (result.Reputation >= 20) return "Репутация в плюсе:\nофис доволен, хотя спасибо сказали не все.";

        return "Репутация скромная:\nзадания закрыты, но легендой офиса ты станешь позже.";
    }

    private string GetTimeComment(GameManager.RunResult result)
    {
        if (result.Hour < 11) return "Темп хороший:\nзадачи закрыты до того, как офис успел придумать новые.";
        if (result.Hour < 14) return "Темп нормальный:\nрабочий день прошел без лишней паники.";

        return "Темп спокойный:\nглавное, что день все-таки закончился победой.";
    }

    private void SetVisible(bool isVisible)
    {
        if (panelCanvasGroup == null) return;

        panelCanvasGroup.alpha = isVisible ? 1f : 0f;
        panelCanvasGroup.interactable = isVisible;
        panelCanvasGroup.blocksRaycasts = isVisible;
    }
}
