//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class QuestPanelUI : MonoBehaviour
//{
//    [SerializeField] private GameObject panel;
//    [SerializeField] private TextMeshProUGUI titleText;
//    [SerializeField] private TextMeshProUGUI giverText;
//    [SerializeField] private TextMeshProUGUI stepText;
//    [SerializeField] private TextMeshProUGUI statusText;
//    [SerializeField] private TextMeshProUGUI progressText;
//    [SerializeField] private Slider progressSlider;
//    [SerializeField] private bool hideWhenNoQuest = false;

//    private CanvasGroup panelCanvasGroup;

//    private void Awake()
//    {
//        if (panel == null)
//        {
//            panel = gameObject;
//        }

//        panelCanvasGroup = panel.GetComponent<CanvasGroup>();
//        if (panelCanvasGroup == null)
//        {
//            panelCanvasGroup = panel.AddComponent<CanvasGroup>();
//        }

//        if (progressSlider != null)
//        {
//            progressSlider.minValue = 0f;
//            progressSlider.maxValue = 1f;
//        }
//    }

//    private void OnEnable()
//    {
//        QuestManager.OnQuestStateChanged += UpdateView;
//    }

//    private void OnDisable()
//    {
//        QuestManager.OnQuestStateChanged -= UpdateView;
//    }

//    private void UpdateView(QuestManager.QuestUiState state)
//    {
//        SetVisible(state.HasActiveQuest || !hideWhenNoQuest);

//        if (titleText != null) titleText.text = state.Title;
//        if (giverText != null) giverText.text = state.HasActiveQuest ? $"От: {state.GiverName}" : "";
//        if (stepText != null) stepText.text = state.CurrentStep;
//        if (statusText != null) statusText.text = state.Status;

//        int totalSteps = state.TotalSteps < 0 ? 0 : state.TotalSteps;
//        int completedSteps = state.CompletedSteps;
//        if (completedSteps < 0) completedSteps = 0;
//        if (completedSteps > totalSteps) completedSteps = totalSteps;
//        if (progressText != null)
//        {
//            progressText.text = totalSteps > 0 ? $"{completedSteps}/{totalSteps}" : "";
//        }

//        if (progressSlider != null)
//        {
//            progressSlider.value = totalSteps > 0 ? completedSteps / (float)totalSteps : 0f;
//        }
//    }

//    private void SetVisible(bool isVisible)
//    {
//        if (panelCanvasGroup == null) return;

//        panelCanvasGroup.alpha = isVisible ? 1f : 0f;
//        panelCanvasGroup.interactable = isVisible;
//        panelCanvasGroup.blocksRaycasts = isVisible;
//    }
//}
