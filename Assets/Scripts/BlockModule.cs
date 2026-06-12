using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockModule : InteractiveModule
{
    public enum RepairMinigameType { SequenceKeys, TimingBar, WireConnect }

    public struct MinigameUiState
    {
        public RepairMinigameType Type;
        public string Title;
        public string Instruction;
        public string Sequence;
        public string Feedback;
        public int TimeLeft;
        public float MarkerPosition;
        public string[] LeftWireLabels;
        public string[] RightWireLabels;
        public bool[] ConnectedWires;
        public int SelectedLeftWire;
    }

    public static event Action<bool> OnMinigameVisibilityChanged;
    public static event Action<MinigameUiState> OnMinigameUpdated;
    private static event Action<int> OnLeftWireClicked;
    private static event Action<int> OnRightWireClicked;

    [SerializeField] private float repairTime = 3f;
    [SerializeField] private InteractiveModule nextModule;

    [Header("Мини-игра ремонта")]
    [SerializeField] private bool useMinigame = true;
    [SerializeField] private RepairMinigameType minigameType = RepairMinigameType.SequenceKeys;
    [SerializeField] private float minigameTimeLimit = 8f;
    [SerializeField] private float failAngerPenalty = 8f;

    [Header("Последовательность клавиш")]
    [SerializeField] private int sequenceLength = 4;
    [Header("Провода")]
    [SerializeField] private int wirePairCount = 4;

    [Header("Ползунок")]
    [SerializeField] private float timingMarkerSpeed = 1.4f;
    [SerializeField, Range(0.05f, 0.5f)] private float timingGreenZoneSize = 0.22f;

    [Header("Нужный предмет")]
    [SerializeField] private ItemData requiredItem;
    [SerializeField] private bool consumeRequiredItem = true;

    [Header("Награда")]
    [SerializeField] private int repReward = 10;
    [SerializeField] private float angerDown = 5f;

    private enum RepairKey { W, A, S, D }

    private readonly RepairKey[] keyPool = { RepairKey.W, RepairKey.A, RepairKey.S, RepairKey.D };
    private RepairKey[] currentSequence;
    private int sequenceIndex;
    private float minigameTimer;
    private float timingMarkerPosition;
    private float timingDirection = 1f;
    private bool isRepairing;
    private bool isMinigameRunning;
    private string titleText = "Ремонт";
    private string instructionText = "";
    private string feedbackMessage = "";
    private readonly string[] wirePool = { "Красный", "Синий", "Желтый", "Зеленый", "Белый", "Черный" };
    private string[] leftWires;
    private string[] rightWires;
    private bool[] connectedWires;
    private int selectedLeftWire = -1;

    private void OnEnable()
    {
        OnLeftWireClicked += HandleLeftWireClicked;
        OnRightWireClicked += HandleRightWireClicked;
    }

    private void OnDisable()
    {
        OnLeftWireClicked -= HandleLeftWireClicked;
        OnRightWireClicked -= HandleRightWireClicked;
    }

    public static void ClickLeftWire(int index)
    {
        OnLeftWireClicked?.Invoke(index);
    }

    public static void ClickRightWire(int index)
    {
        OnRightWireClicked?.Invoke(index);
    }

    public override void Interact()
    {
        if (!isActive || isRepairing || isMinigameRunning) return;

        if (!HasRequiredItem())
        {
            Debug.Log("Не хватает нужного предмета.");
            return;
        }

        if (useMinigame)
        {
            StartCoroutine(RunMinigame());
        }
        else
        {
            ConsumeRequiredItem();
            StartCoroutine(UnlockCoroutine());
        }
    }

    private IEnumerator RunMinigame()
    {
        isMinigameRunning = true;
        minigameTimer = minigameTimeLimit;
        feedbackMessage = "";
        SetMinigamePanel(true);

        bool success = false;

        switch (minigameType)
        {
            case RepairMinigameType.SequenceKeys:
                PrepareSequence();
                ShowSequenceMode();
                yield return StartCoroutine(RunSequenceMinigame(result => success = result));
                break;

            case RepairMinigameType.TimingBar:
                PrepareTimingBar();
                ShowTimingMode();
                yield return StartCoroutine(RunTimingBarMinigame(result => success = result));
                break;

            case RepairMinigameType.WireConnect:
                PrepareWires();
                ShowWireMode();
                yield return StartCoroutine(RunWireConnectMinigame(result => success = result));
                break;
        }

        isMinigameRunning = false;

        if (success)
        {
            ConsumeRequiredItem();
            MarkQuestTaskAsDone();
            feedbackMessage = "Готово";
            UpdateMinigameUi();
            yield return new WaitForSeconds(0.35f);
            SetMinigamePanel(false);
            StartCoroutine(UnlockCoroutine());
        }
        else
        {
            feedbackMessage = "Провал";
            UpdateMinigameUi();
            Debug.Log("Ремонт провален.");

            if (StatsManager.Instance != null)
            {
                StatsManager.Instance.ChangeAnger(failAngerPenalty);
            }

            yield return new WaitForSeconds(0.5f);
            SetMinigamePanel(false);
        }
    }

    private IEnumerator RunSequenceMinigame(Action<bool> onFinished)
    {
        while (minigameTimer > 0f && sequenceIndex < currentSequence.Length)
        {
            minigameTimer -= Time.deltaTime;

            RepairKey expectedKey = currentSequence[sequenceIndex];
            if (WasPressed(expectedKey))
            {
                sequenceIndex++;
                feedbackMessage = "Верно";
            }
            else if (WasAnyRepairKeyPressed())
            {
                feedbackMessage = "Не та клавиша";
                UpdateMinigameUi();
                yield return new WaitForSeconds(0.35f);
                onFinished?.Invoke(false);
                yield break;
            }

            UpdateMinigameUi();
            yield return null;
        }

        onFinished?.Invoke(sequenceIndex >= currentSequence.Length);
    }

    private IEnumerator RunTimingBarMinigame(Action<bool> onFinished)
    {
        while (minigameTimer > 0f)
        {
            minigameTimer -= Time.deltaTime;
            timingMarkerPosition += timingDirection * timingMarkerSpeed * Time.deltaTime;

            if (timingMarkerPosition >= 1f)
            {
                timingMarkerPosition = 1f;
                timingDirection = -1f;
            }
            else if (timingMarkerPosition <= 0f)
            {
                timingMarkerPosition = 0f;
                timingDirection = 1f;
            }

            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                bool hit = Mathf.Abs(timingMarkerPosition - 0.5f) <= timingGreenZoneSize * 0.5f;
                feedbackMessage = hit ? "Попадание" : "Мимо";
                UpdateMinigameUi();
                yield return new WaitForSeconds(0.25f);
                onFinished?.Invoke(hit);
                yield break;
            }

            UpdateMinigameUi();
            yield return null;
        }

        onFinished?.Invoke(false);
    }

    private IEnumerator RunWireConnectMinigame(Action<bool> onFinished)
    {
        while (minigameTimer > 0f && !AreAllWiresConnected())
        {
            minigameTimer -= Time.deltaTime;
            UpdateMinigameUi();
            yield return null;
        }

        onFinished?.Invoke(AreAllWiresConnected());
    }

    private void PrepareSequence()
    {
        int length = Mathf.Max(1, sequenceLength);
        currentSequence = new RepairKey[length];
        sequenceIndex = 0;

        for (int i = 0; i < currentSequence.Length; i++)
        {
            currentSequence[i] = keyPool[UnityEngine.Random.Range(0, keyPool.Length)];
        }
    }

    private void PrepareTimingBar()
    {
        timingMarkerPosition = 0f;
        timingDirection = 1f;
    }

    private void PrepareWires()
    {
        int count = (int)Mathf.Clamp(wirePairCount, 2, wirePool.Length);
        leftWires = new string[count];
        rightWires = new string[count];
        connectedWires = new bool[count];
        selectedLeftWire = -1;

        for (int i = 0; i < count; i++)
        {
            leftWires[i] = wirePool[i];
            rightWires[i] = wirePool[i];
        }

        for (int i = 0; i < rightWires.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, rightWires.Length);
            string temp = rightWires[i];
            rightWires[i] = rightWires[randomIndex];
            rightWires[randomIndex] = temp;
        }
    }

    private IEnumerator UnlockCoroutine()
    {
        isRepairing = true;
        Deactivate();

        Debug.Log("Начат ремонт.");
        yield return new WaitForSeconds(repairTime);

        MarkQuestTaskAsDone();

        Debug.Log("Объект приведен в порядок.");

        if (nextModule != null)
        {
            nextModule.Activate();
        }

        isRepairing = false;
    }

    public override bool IsBroken()
    {
        return isActive;
    }

    private void ShowSequenceMode()
    {
        titleText = "Ремонт";
        instructionText = "Нажми клавиши по порядку";
        UpdateMinigameUi();
    }

    private void ShowTimingMode()
    {
        titleText = "Ремонт";
        instructionText = "Нажми Space в зеленой зоне";
        UpdateMinigameUi();
    }

    private void ShowWireMode()
    {
        titleText = "Провода";
        instructionText = "Выбери провод слева, затем такой же справа";
        UpdateMinigameUi();
    }

    private void UpdateMinigameUi()
    {
        OnMinigameUpdated?.Invoke(new MinigameUiState
        {
            Type = minigameType,
            Title = titleText,
            Instruction = instructionText,
            Sequence = currentSequence != null ? BuildSequenceText() : "",
            Feedback = feedbackMessage,
            TimeLeft = Mathf.CeilToInt(minigameTimer),
            MarkerPosition = timingMarkerPosition,
            LeftWireLabels = leftWires,
            RightWireLabels = rightWires,
            ConnectedWires = connectedWires,
            SelectedLeftWire = selectedLeftWire
        });
    }

    private string BuildSequenceText()
    {
        string result = "";

        for (int i = 0; i < currentSequence.Length; i++)
        {
            string keyText = KeyToText(currentSequence[i]);
            result += i < sequenceIndex ? $"<color=#75D982>[{keyText}]</color> " : $"{keyText} ";
        }

        return result;
    }

    private void SetMinigamePanel(bool isVisible)
    {
        OnMinigameVisibilityChanged?.Invoke(isVisible);
    }

    private bool HasRequiredItem()
    {
        return requiredItem == null
            || (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(requiredItem));
    }

    private void ConsumeRequiredItem()
    {
        if (requiredItem == null || !consumeRequiredItem || InventoryManager.Instance == null) return;

        InventoryManager.Instance.RemoveItem(requiredItem, 1);
    }

    private void MarkQuestTaskAsDone()
    {
        if (QuestManager.Instance != null && QuestManager.Instance.isQuestActive)
        {
            QuestManager.Instance.MarkTaskAsDone(repReward, angerDown);
        }
    }

    private bool WasPressed(RepairKey key)
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return false;

        switch (key)
        {
            case RepairKey.W: return keyboard.wKey.wasPressedThisFrame;
            case RepairKey.A: return keyboard.aKey.wasPressedThisFrame;
            case RepairKey.S: return keyboard.sKey.wasPressedThisFrame;
            case RepairKey.D: return keyboard.dKey.wasPressedThisFrame;
            default: return false;
        }
    }

    private bool WasAnyRepairKeyPressed()
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null
            && (keyboard.wKey.wasPressedThisFrame
                || keyboard.aKey.wasPressedThisFrame
                || keyboard.sKey.wasPressedThisFrame
                || keyboard.dKey.wasPressedThisFrame);
    }

    private string KeyToText(RepairKey key)
    {
        switch (key)
        {
            case RepairKey.W: return "W";
            case RepairKey.A: return "A";
            case RepairKey.S: return "S";
            case RepairKey.D: return "D";
            default: return "?";
        }
    }

    private void HandleLeftWireClicked(int index)
    {
        if (!isMinigameRunning || minigameType != RepairMinigameType.WireConnect) return;
        if (leftWires == null || connectedWires == null || index < 0 || index >= leftWires.Length) return;
        if (connectedWires[index]) return;

        selectedLeftWire = index;
        feedbackMessage = "";
        UpdateMinigameUi();
    }

    private void HandleRightWireClicked(int index)
    {
        if (!isMinigameRunning || minigameType != RepairMinigameType.WireConnect) return;
        if (selectedLeftWire < 0)
        {
            feedbackMessage = "Сначала выбери провод слева";
            UpdateMinigameUi();
            return;
        }

        if (rightWires == null || index < 0 || index >= rightWires.Length) return;

        bool isCorrect = leftWires[selectedLeftWire] == rightWires[index];
        if (isCorrect)
        {
            connectedWires[selectedLeftWire] = true;
            selectedLeftWire = -1;
            feedbackMessage = "Соединено";
        }
        else
        {
            selectedLeftWire = -1;
            feedbackMessage = "Не тот провод";
        }

        UpdateMinigameUi();
    }

    private bool AreAllWiresConnected()
    {
        if (connectedWires == null || connectedWires.Length == 0) return false;

        for (int i = 0; i < connectedWires.Length; i++)
        {
            if (!connectedWires[i]) return false;
        }

        return true;
    }
}
