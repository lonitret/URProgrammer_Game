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

        if (QuestManager.Instance != null)
        {
            if (!QuestManager.Instance.isQuestActive)
            {
                QuestManager.Instance.AcceptQuest(questText, this);
                Debug.Log("Квест принят!");
            }
            else
            {
                Debug.Log("Ты еще не закончил текущее задание!");
            }
        }
    }

    public void MarkAsCompleted()
    {
        isQuestCompleted = true;
    }
}