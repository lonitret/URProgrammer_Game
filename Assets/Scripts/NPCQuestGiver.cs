using UnityEngine;

public class NPCQuestGiver : MonoBehaviour, IInteractable
{
    public enum QuestType { BringItem, FixObject, TalkOnly }

    [Header("Тип задания этого NPC")]
    [SerializeField] private QuestType questType;

    [Header("Имя и Диалоги NPC")]
    [SerializeField] private string npcName = "Коллега";
    [SerializeField] private Dialogue startQuestDialogue;
    [SerializeField] private Dialogue declineQuestDialogue;
    [SerializeField] private Dialogue progressQuestDialogue;
    [SerializeField] private Dialogue completeQuestDialogue;
    [SerializeField] private Dialogue postQuestDialogue;

    [Header("Настройки штрафа за отказ")]
    [SerializeField] private int repPenalty = 5;
    [SerializeField] private float angerPenalty = 10f;

    [Header("Награда за выполнение")]
    [SerializeField] private int repReward = 10;
    [SerializeField] private float angerDown = 5f;

    [Header("Если квест: принести предмет")]
    [SerializeField] private ItemData requiredItem;

    [Header("Если квест: починить объект")]
    [SerializeField] private Actor targetActor;
    [SerializeField] private bool unlocksCoffeeAfterCompletion = true;

    private bool isCompleted = false;

    public string NpcName => npcName;

    private void Start()
    {
        SetupDialogueName(startQuestDialogue);
        SetupDialogueName(declineQuestDialogue);
        SetupDialogueName(progressQuestDialogue);
        SetupDialogueName(completeQuestDialogue);
        SetupDialogueName(postQuestDialogue);
    }

    public void Interact()
    {
        if (isCompleted)
        {
            DialogueManager.Instance.StartDialogue(postQuestDialogue);
            AudioManager.Instance.PlaySFX(SoundType.NPCVoice);
            return;
        }

        if (QuestManager.Instance.isQuestActive)
        {
            if (!QuestManager.Instance.IsCurrentQuestGiver(this))
            {
                Debug.Log("Сначала нужно закончить активное задание.");
                return;
            }

            CheckQuestConditions();
            return;
        }

        AudioManager.Instance.PlaySFX(SoundType.NPCVoice);
        DialogueManager.Instance.StartDialogue(startQuestDialogue,
            onComplete: () =>
            {
                string questDescription = startQuestDialogue != null && startQuestDialogue.sentences.Length > 0
                    ? startQuestDialogue.sentences[0]
                    : $"Задание от {npcName}";
                bool shouldUnlockCoffee = questType == QuestType.FixObject
                    && targetActor != null
                    && unlocksCoffeeAfterCompletion;

                QuestManager.Instance.AcceptQuest(questDescription, this, shouldUnlockCoffee, targetActor);

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
                    Debug.Log($"Отказ от квеста! Репутация: -{repPenalty}, Гнев: +{angerPenalty}");
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
    }

    private void SetupDialogueName(Dialogue dialogue)
    {
        if (dialogue != null)
        {
            dialogue.npcName = npcName;
        }
    }

}
