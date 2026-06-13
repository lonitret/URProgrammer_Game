using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public struct TutorialStep
    {
        public string title;
        [TextArea(2, 5)] public string body;
    }

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private TextMeshProUGUI nextButtonText;

    [Header("Settings")]
    [SerializeField] private bool showOnStart = true;
    [SerializeField] private bool pauseWhileOpen = true;
    [SerializeField]
    private TutorialStep[] steps =
    {
        new TutorialStep
        {
            title = "Добро пожаловать",
            body = "Ты стажер-программист в офисе. Твоя задача - помогать коллегам, чинить технику и не довести стресс до предела."
        },
        new TutorialStep
        {
            title = "Управление",
            body = "WASD - движение. E - взаимодействие с коллегами и объектами. Escape - пауза."
        },
        new TutorialStep
        {
            title = "Задания",
            body = "Подойди к коллеге и нажми E, чтобы взять задание. Пока задание активно, другие квесты и чужие объекты будут недоступны."
        },
        new TutorialStep
        {
            title = "Предметы и стресс",
            body = "Следи за репутацией и уровнем стресса. Кофе можно использовать из инвентаря, когда он появится по кнопке F."
        }
    };

    private int currentStepIndex;
    private float previousTimeScale = 1f;
    private bool isOpen;

    private void Awake()
    {
        if (panel != null) panel.SetActive(false);
        if (nextButton != null) nextButton.onClick.AddListener(ShowNextStep);
        if (skipButton != null) skipButton.onClick.AddListener(CloseTutorial);
    }

    private void Start()
    {
        if (showOnStart)
        {
            OpenTutorial();
        }
    }

    private void OnDestroy()
    {
        if (nextButton != null) nextButton.onClick.RemoveListener(ShowNextStep);
        if (skipButton != null) skipButton.onClick.RemoveListener(CloseTutorial);
    }

    public void OpenTutorial()
    {
        if (panel == null || steps == null || steps.Length == 0) return;

        isOpen = true;
        currentStepIndex = 0;
        previousTimeScale = Time.timeScale;

        if (pauseWhileOpen)
        {
            Time.timeScale = 0f;
        }

        panel.SetActive(true);
        UpdateView();
    }

    private void ShowNextStep()
    {
        if (!isOpen) return;

        if (currentStepIndex >= steps.Length - 1)
        {
            CloseTutorial();
            return;
        }

        currentStepIndex++;
        UpdateView();
    }

    private void CloseTutorial()
    {
        if (!isOpen) return;

        isOpen = false;
        if (panel != null) panel.SetActive(false);

        if (pauseWhileOpen && !GameManager.isGameOver && !GameManager.isPaused)
        {
            Time.timeScale = previousTimeScale;
        }
    }

    private void UpdateView()
    {
        TutorialStep step = steps[currentStepIndex];

        if (titleText != null) titleText.text = step.title;
        if (bodyText != null) bodyText.text = step.body;
        if (progressText != null) progressText.text = $"{currentStepIndex + 1}/{steps.Length}";
        if (nextButtonText != null)
        {
            nextButtonText.text = currentStepIndex >= steps.Length - 1 ? "Начать" : "Далее";
        }
    }
}
