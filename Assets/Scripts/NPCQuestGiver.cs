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

    [Header("Настройки штрафа за ОТКАЗ")]
    [SerializeField] private int repPenalty = 5;
    [SerializeField] private float angerPenalty = 10f;

    [Header("Если квест: Принести предмет")]
    [SerializeField] private ItemData requiredItem;

    [Header("Если квест: Починить объект")]
    [SerializeField] private Actor targetActor;

    private bool isCompleted = false;

    private void Start()
    {
        startQuestDialogue.npcName = npcName;
        declineQuestDialogue.npcName = npcName;
        progressQuestDialogue.npcName = npcName;
        completeQuestDialogue.npcName = npcName;
        postQuestDialogue.npcName = npcName;
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
            CheckQuestConditions();
            return;
        }

        AudioManager.Instance.PlaySFX(SoundType.NPCVoice);

        DialogueManager.Instance.StartDialogue(startQuestDialogue,
            onComplete: () =>
            {
                QuestManager.Instance.AcceptQuest(startQuestDialogue.sentences[0], this);

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
                if (InventoryManager.Instance.HasItem(requiredItem))
                {
                    InventoryManager.Instance.RemoveItem(requiredItem, 1);
                    QuestManager.Instance.isTaskCompleted = true;
                    QuestManager.Instance.GiveRewardAndFinish();
                    DialogueManager.Instance.StartDialogue(completeQuestDialogue);
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
                    DialogueManager.Instance.StartDialogue(completeQuestDialogue);
                }
                else
                {
                    DialogueManager.Instance.StartDialogue(progressQuestDialogue);
                }
                break;

            case QuestType.TalkOnly:
                QuestManager.Instance.isTaskCompleted = true;
                QuestManager.Instance.GiveRewardAndFinish();
                DialogueManager.Instance.StartDialogue(completeQuestDialogue);
                break;
        }
    }

    public void MarkAsCompleted()
    {
        isCompleted = true;
    }
}