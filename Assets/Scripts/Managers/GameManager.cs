using TMPro;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum RunEndReason { LevelComplete, GameOver, WorkDayEnded }

    public struct RunResult
    {
        public RunEndReason Reason;
        public string Message;
        public int Reputation;
        public float Anger;
        public float MaxAnger;
        public int CompletedQuests;
        public int RequiredQuests;
        public int Hour;
        public int Minute;
    }

    public static GameManager Instance;
    public static bool isPaused = false;
    public static bool isGameOver = false;
    public static event Action<RunResult> OnRunEnded;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject gameOverMenuUI;
    [SerializeField] private bool finishLevelAfterQuest = true;
    [SerializeField] private int questsRequiredToFinishLevel = 1;
    [SerializeField] private string levelCompleteMessage = "День пройден";
    [SerializeField] private string gameOverMessage = "Нервный срыв!";
    [SerializeField] private string dayEndedMessage = "Рабочий день окончен!";

    private int completedQuestCount;
    private bool isMinigameOpen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (gameOverMenuUI != null) gameOverMenuUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        isMinigameOpen = false;
        ApplyCursorState();
    }

    private void OnEnable()
    {
        QuestManager.OnQuestCompleted += HandleQuestCompleted;
        BlockModule.OnMinigameVisibilityChanged += HandleMinigameVisibilityChanged;
    }

    private void OnDisable()
    {
        QuestManager.OnQuestCompleted -= HandleQuestCompleted;
        BlockModule.OnMinigameVisibilityChanged -= HandleMinigameVisibilityChanged;
    }

    public void Resume()
    {
        if (isGameOver) return;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        ApplyCursorState();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResumeAllSounds();
        }
    }

    public void Pause()
    {
        if (isGameOver) return;

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        ApplyCursorState();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PauseAllSounds();
        }
    }

    public void GameOver()
    {
        EndRun(gameOverMessage, RunEndReason.GameOver);
    }

    public void CompleteLevel()
    {
        EndRun(levelCompleteMessage, RunEndReason.LevelComplete);
    }

    public void EndWorkDay()
    {
        EndRun(dayEndedMessage, RunEndReason.WorkDayEnded);
    }

    public void RestartDay()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;

        if (AudioManager.Instance != null) AudioManager.Instance.StopAllSounds();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSounds();
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void HandleQuestCompleted()
    {
        completedQuestCount++;

        if (finishLevelAfterQuest && completedQuestCount >= Mathf.Max(1, questsRequiredToFinishLevel))
        {
            CompleteLevel();
        }
    }

    private void EndRun(string message, RunEndReason reason)
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;

        if (gameOverMenuUI != null)
        {
            SetEndPanelText(message);
            gameOverMenuUI.SetActive(true);
        }

        ApplyCursorState();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSounds();
        }

        OnRunEnded?.Invoke(BuildRunResult(message, reason));
    }

    private void HandleMinigameVisibilityChanged(bool isVisible)
    {
        isMinigameOpen = isVisible;
        ApplyCursorState();
    }

    public void RefreshCursorState()
    {
        ApplyCursorState();
    }

    private void ApplyCursorState()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SetEndPanelText(string message)
    {
        if (gameOverMenuUI == null) return;

        TextMeshProUGUI[] labels = gameOverMenuUI.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "GameOverText")
            {
                label.text = message;
                return;
            }
        }
    }

    private RunResult BuildRunResult(string message, RunEndReason reason)
    {
        int hour = 0;
        int minute = 0;

        if (TimeManager.instance != null)
        {
            hour = TimeManager.instance.CurrentHour;
            minute = TimeManager.instance.CurrentMinute;
        }

        return new RunResult
        {
            Reason = reason,
            Message = message,
            Reputation = StatsManager.Instance != null ? StatsManager.Instance.reputation : 0,
            Anger = StatsManager.Instance != null ? StatsManager.Instance.currentAnger : 0f,
            MaxAnger = StatsManager.Instance != null ? StatsManager.Instance.maxAnger : 100f,
            CompletedQuests = completedQuestCount,
            RequiredQuests = Mathf.Max(1, questsRequiredToFinishLevel),
            Hour = hour,
            Minute = minute
        };
    }
}
