using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairMinigameUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject sequenceGroup;
    [SerializeField] private GameObject timingGroup;
    [SerializeField] private GameObject wireGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI sequenceText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Slider timingSlider;
    [SerializeField] private Button[] leftWireButtons;
    [SerializeField] private Button[] rightWireButtons;
    [SerializeField] private TextMeshProUGUI[] leftWireLabels;
    [SerializeField] private TextMeshProUGUI[] rightWireLabels;

    private CanvasGroup panelCanvasGroup;
    private readonly Color selectedWireTint = new Color(1.25f, 1.25f, 1.25f);
    private readonly Color connectedWireTint = new Color(0.6f, 0.6f, 0.6f, 0.55f);

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

        if (timingSlider != null)
        {
            timingSlider.minValue = 0f;
            timingSlider.maxValue = 1f;
        }
    }

    private void OnEnable()
    {
        BlockModule.OnMinigameVisibilityChanged += SetVisible;
        BlockModule.OnMinigameUpdated += UpdateView;
    }

    private void OnDisable()
    {
        BlockModule.OnMinigameVisibilityChanged -= SetVisible;
        BlockModule.OnMinigameUpdated -= UpdateView;
    }

    private void SetVisible(bool isVisible)
    {
        if (panelCanvasGroup == null) return;

        panelCanvasGroup.alpha = isVisible ? 1f : 0f;
        panelCanvasGroup.interactable = isVisible;
        panelCanvasGroup.blocksRaycasts = isVisible;
    }

    private void UpdateView(BlockModule.MinigameUiState state)
    {
        bool isSequence = state.Type == BlockModule.RepairMinigameType.SequenceKeys;
        bool isTiming = state.Type == BlockModule.RepairMinigameType.TimingBar;
        bool isWire = state.Type == BlockModule.RepairMinigameType.WireConnect;

        if (sequenceGroup != null) sequenceGroup.SetActive(isSequence);
        if (timingGroup != null) timingGroup.SetActive(isTiming);
        if (wireGroup != null) wireGroup.SetActive(isWire);
        if (titleText != null) titleText.text = state.Title;
        if (timerText != null) timerText.text = state.TimeLeft.ToString();
        if (instructionText != null) instructionText.text = state.Instruction;
        if (sequenceText != null) sequenceText.text = state.Sequence;
        if (feedbackText != null) feedbackText.text = state.Feedback;
        if (timingSlider != null) timingSlider.value = state.MarkerPosition;

        if (isWire)
        {
            UpdateWireButtons(state);
        }
    }

    public void SelectLeftWire(int index)
    {
        BlockModule.ClickLeftWire(index);
    }

    public void SelectRightWire(int index)
    {
        BlockModule.ClickRightWire(index);
    }

    private void UpdateWireButtons(BlockModule.MinigameUiState state)
    {
        UpdateWireSide(leftWireButtons, leftWireLabels, state.LeftWireLabels, state.ConnectedWires, state.SelectedLeftWire, true);
        UpdateWireSide(rightWireButtons, rightWireLabels, state.RightWireLabels, null, -1, false);
    }

    private void UpdateWireSide(
        Button[] buttons,
        TextMeshProUGUI[] labels,
        string[] wireLabels,
        bool[] connected,
        int selectedIndex,
        bool isLeftSide)
    {
        if (buttons == null) return;

        for (int i = 0; i < buttons.Length; i++)
        {
            bool hasWire = wireLabels != null && i < wireLabels.Length;
            buttons[i].gameObject.SetActive(hasWire);

            if (!hasWire) continue;

            bool isConnected = connected != null && i < connected.Length && connected[i];
            buttons[i].interactable = !isConnected || !isLeftSide;

            ApplyWireColor(buttons[i], wireLabels[i], isConnected, i == selectedIndex);

            if (labels != null && i < labels.Length && labels[i] != null)
            {
                labels[i].text = wireLabels[i];
            }
        }
    }

    private void ApplyWireColor(Button button, string wireLabel, bool isConnected, bool isSelected)
    {
        Image image = button.GetComponent<Image>();
        if (image == null) return;

        Color baseColor = GetWireColor(wireLabel);

        if (isConnected)
        {
            image.color = new Color(
                baseColor.r * connectedWireTint.r,
                baseColor.g * connectedWireTint.g,
                baseColor.b * connectedWireTint.b,
                connectedWireTint.a);
            return;
        }

        if (isSelected)
        {
            image.color = new Color(
                Mathf.Clamp(baseColor.r * selectedWireTint.r, 0f, 1f),
                Mathf.Clamp(baseColor.g * selectedWireTint.g, 0f, 1f),
                Mathf.Clamp(baseColor.b * selectedWireTint.b, 0f, 1f),
                1f);
            return;
        }

        image.color = baseColor;
    }

    private Color GetWireColor(string wireLabel)
    {
        switch (wireLabel)
        {
            case "Красный": return new Color(0.9f, 0.18f, 0.18f);
            case "Синий": return new Color(0.18f, 0.38f, 0.95f);
            case "Желтый": return new Color(1f, 0.82f, 0.15f);
            case "Зеленый": return new Color(0.2f, 0.75f, 0.28f);
            case "Белый": return new Color(0.92f, 0.92f, 0.88f);
            case "Черный": return new Color(0.12f, 0.12f, 0.14f);
            default: return Color.white;
        }
    }
}
