using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCQuestGiver : MonoBehaviour, IInteractable
{
    public enum QuestType { BringItem, FixObject, TalkOnly }

    public static event Action<Transform, string> OnQuestMarkerChanged;

    private static readonly List<NPCQuestGiver> questGivers = new List<NPCQuestGiver>();

    [Header("Тип задания этого NPC")]
    [SerializeField] private QuestType questType;

    [Header("Имя и диалоги NPC")]
    [SerializeField] private string npcName = "Коллега";
    [SerializeField] private Dialogue startQuestDialogue;
    [SerializeField] private Dialogue declineQuestDialogue;
    [SerializeField] private Dialogue progressQuestDialogue;
    [SerializeField] private Dialogue completeQuestDialogue;
    [SerializeField] private Dialogue postQuestDialogue;

    [Header("Текст для UI задания")]
    [SerializeField] private string questTitleOverride;
    [SerializeField] private string firstStepOverride;
    [SerializeField] private string returnStepOverride;

    [Header("Маркер цели задания")]
    [SerializeField] private Transform firstObjectiveMarkerTarget;
    [SerializeField] private Transform nextObjectiveMarkerTarget;
    [SerializeField] private ItemData itemNeededToUseNextObjectiveMarker;

    [Header("Сценарная блокировка")]
    [SerializeField] private int requiredCompletedQuestsToOffer = 0;
    [SerializeField] private Dialogue lockedQuestDialogue;

    [Header("Настройки штрафа за отказ")]
    [SerializeField] private int repPenalty = 8;
    [SerializeField] private float angerPenalty = 12f;

    [Header("Награда за выполнение")]
    [SerializeField] private int repReward = 15;
    [SerializeField] private float angerDown = 8f;

    [Header("Если квест: принести предмет")]
    [SerializeField] private ItemData requiredItem;

    [Header("Если квест: починить объект")]
    [SerializeField] private Actor targetActor;
    [SerializeField] private bool unlocksCoffeeAfterCompletion = true;

    private bool isCompleted = false;

    public string NpcName => npcName;
    public int RequiredCompletedQuestsToOffer => requiredCompletedQuestsToOffer;

    private void OnEnable()
    {
        if (!questGivers.Contains(this)) questGivers.Add(this);
        QuestManager.OnQuestStateChanged += HandleQuestStateChanged;
        RefreshQuestMarker();
    }

    private void OnDisable()
    {
        questGivers.Remove(this);
        QuestManager.OnQuestStateChanged -= HandleQuestStateChanged;
        RefreshQuestMarker();
    }

    private void Start()
    {
        SetupDialogueName(startQuestDialogue);
        SetupDialogueName(declineQuestDialogue);
        SetupDialogueName(progressQuestDialogue);
        SetupDialogueName(completeQuestDialogue);
        SetupDialogueName(postQuestDialogue);
        SetupDialogueName(lockedQuestDialogue);
        RefreshQuestMarker();
    }

    public void Interact()
    {
        if (isCompleted)
        {
            DialogueManager.Instance.StartDialogue(postQuestDialogue);
            AudioManager.Instance.PlaySFX(SoundType.NPCVoice);
            return;
        }

        if (QuestManager.Instance != null && QuestManager.Instance.isQuestActive)
        {
            if (!QuestManager.Instance.IsCurrentQuestGiver(this))
            {
                Debug.Log("Сначала нужно закончить активное задание.");
                return;
            }

            CheckQuestConditions();
            return;
        }

        if (!CanOfferQuest())
        {
            AudioManager.Instance.PlaySFX(SoundType.NPCVoice);
            DialogueManager.Instance.StartDialogue(GetLockedDialogue());
            return;
        }

        AudioManager.Instance.PlaySFX(SoundType.NPCVoice);
        DialogueManager.Instance.StartDialogue(startQuestDialogue,
            onComplete: () =>
            {
                string questDescription = GetQuestTitle();
                bool shouldUnlockCoffee = questType == QuestType.FixObject
                    && targetActor != null
                    && unlocksCoffeeAfterCompletion;

                QuestManager.Instance.AcceptQuest(
                    questDescription,
                    this,
                    shouldUnlockCoffee,
                    targetActor,
                    GetFirstStepText(),
                    GetReturnStepText());

                if (questType == QuestType.FixObject && targetActor != null)
                {
                    BlockModule block = targetActor.GetComponentInChildren<BlockModule>();
                    if (block != null) block.Activate();
                }
            },
            onDeclined: () =>
            {
                if (StatsManager.Instance != null)
                {
                    StatsManager.Instance.ChangeReputation(-repPenalty);
                    StatsManager.Instance.ChangeAnger(angerPenalty);
                    Debug.Log($"Отказ от квеста! Репутация: -{repPenalty}, стресс: +{angerPenalty}");
                }

                DialogueManager.Instance.StartDialogue(declineQuestDialogue);
            }
        );
    }

    private void CheckQuestConditions()
    {
        AudioManager.Instance.PlaySFX(SoundType.NPCVoice);

        switch (questType)
        {
            case QuestType.BringItem:
                if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(requiredItem))
                {
                    InventoryManager.Instance.RemoveItem(requiredItem, 1);
                    QuestManager.Instance.CompleteQuestImmediately(repReward, angerDown);
                    DialogueManager.Instance.StartDialogue(completeQuestDialogue, QuestManager.Instance.NotifyQuestTurnedIn);
                }
                else
                {
                    DialogueManager.Instance.StartDialogue(progressQuestDialogue);
                }
                break;

            case QuestType.FixObject:
                if (QuestManager.Instance.isTaskCompleted)
                {
                    QuestManager.Instance.GiveRewardAndFinish();
                    DialogueManager.Instance.StartDialogue(completeQuestDialogue, QuestManager.Instance.NotifyQuestTurnedIn);
                }
                else
                {
                    DialogueManager.Instance.StartDialogue(progressQuestDialogue);
                }
                break;

            case QuestType.TalkOnly:
                QuestManager.Instance.CompleteQuestImmediately(repReward, angerDown);
                DialogueManager.Instance.StartDialogue(completeQuestDialogue, QuestManager.Instance.NotifyQuestTurnedIn);
                break;
        }
    }

    public void MarkAsCompleted()
    {
        isCompleted = true;
        RefreshQuestMarker();
    }

    public string GetInteractionText()
    {
        if (QuestManager.Instance != null && QuestManager.Instance.isQuestActive)
        {
            if (!QuestManager.Instance.IsCurrentQuestGiver(this)) return "";
            if (QuestManager.Instance.isTaskCompleted) return "[E] Сдать задание";

            return "[E] Поговорить";
        }

        if (!CanOfferQuest()) return "[E] Позже";

        return "[E] Поговорить";
    }

    public bool ShouldShowAvailableQuestMarker()
    {
        return !isCompleted && CanOfferQuest();
    }

    public Transform GetObjectiveMarkerTarget()
    {
        if (itemNeededToUseNextObjectiveMarker != null
            && InventoryManager.Instance != null
            && InventoryManager.Instance.HasItem(itemNeededToUseNextObjectiveMarker))
        {
            if (nextObjectiveMarkerTarget != null) return nextObjectiveMarkerTarget;
            if (targetActor != null) return targetActor.transform;
        }

        if (firstObjectiveMarkerTarget != null) return firstObjectiveMarkerTarget;
        if (targetActor != null) return targetActor.transform;

        return null;
    }

    public static void RefreshQuestMarker()
    {
        NPCQuestGiver target = FindMarkerTarget();
        OnQuestMarkerChanged?.Invoke(target != null ? target.transform : null, target != null ? "!" : "");
    }

    private static NPCQuestGiver FindMarkerTarget()
    {
        if (QuestManager.Instance != null && QuestManager.Instance.isQuestActive)
        {
            if (!QuestManager.Instance.isTaskCompleted) return null;

            foreach (NPCQuestGiver giver in questGivers)
            {
                if (giver != null && !giver.isCompleted && QuestManager.Instance.IsCurrentQuestGiver(giver))
                {
                    return giver;
                }
            }

            return null;
        }

        NPCQuestGiver best = null;
        foreach (NPCQuestGiver giver in questGivers)
        {
            if (giver == null || giver.isCompleted || !giver.CanOfferQuest()) continue;

            if (best == null || giver.requiredCompletedQuestsToOffer < best.requiredCompletedQuestsToOffer)
            {
                best = giver;
            }
        }

        return best;
    }

    private bool CanOfferQuest()
    {
        return QuestManager.Instance == null
            || QuestManager.Instance.CompletedQuestCount >= requiredCompletedQuestsToOffer;
    }

    private void HandleQuestStateChanged(QuestManager.QuestUiState state)
    {
        RefreshQuestMarker();
    }

    private string GetQuestTitle()
    {
        if (!string.IsNullOrWhiteSpace(questTitleOverride)) return questTitleOverride;
        if (startQuestDialogue != null && startQuestDialogue.sentences != null && startQuestDialogue.sentences.Length > 0)
        {
            return startQuestDialogue.sentences[0];
        }

        return $"Задание от {npcName}";
    }

    private string GetFirstStepText()
    {
        if (!string.IsNullOrWhiteSpace(firstStepOverride)) return firstStepOverride;

        switch (questType)
        {
            case QuestType.BringItem:
                return "Найди нужный предмет и вернись к NPC";
            case QuestType.FixObject:
                return "Найди и почини нужный объект";
            case QuestType.TalkOnly:
                return "Поговори с NPC";
            default:
                return "Выполни задание";
        }
    }

    private string GetReturnStepText()
    {
        if (!string.IsNullOrWhiteSpace(returnStepOverride)) return returnStepOverride;
        return $"Вернись к {npcName}";
    }

    private Dialogue GetLockedDialogue()
    {
        if (lockedQuestDialogue != null && lockedQuestDialogue.sentences != null && lockedQuestDialogue.sentences.Length > 0)
        {
            return lockedQuestDialogue;
        }

        return new Dialogue
        {
            npcName = npcName,
            sentences = new[]
            {
                "Сначала закончи предыдущую задачу. Потом приходи, у меня тоже есть просьба."
            }
        };
    }

    private void SetupDialogueName(Dialogue dialogue)
    {
        if (dialogue != null)
        {
            dialogue.npcName = npcName;
        }
    }
}
