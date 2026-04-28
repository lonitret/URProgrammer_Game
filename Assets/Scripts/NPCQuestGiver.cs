using UnityEngine;

public class NPCQuestGiver : MonoBehaviour, IInteractable
{
    [SerializeField] private string questText = "Почини кофемашину!";
    [SerializeField] private string thanksText = "Спасибо за помощь!";

    private bool isQuestCompleted = false;

    public void Interact()
    {
        if (isQuestCompleted)
        {
            Debug.Log(thanksText);
            return;
        }

        if (QuestManager.Instance.isQuestActive && QuestManager.Instance.isTaskCompleted)
        {
            QuestManager.Instance.GiveRewardAndFinish();
            Debug.Log("Работа принята, награда получена!");
            return;
        }

        if (!QuestManager.Instance.isQuestActive)
        {
            QuestManager.Instance.AcceptQuest(questText, this);
        }
    }

    public void MarkAsCompleted()
    {
        isQuestCompleted = true;
    }
}
