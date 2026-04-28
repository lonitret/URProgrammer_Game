using UnityEngine;
using System.Collections;

public class BlockModule : InteractiveModule
{
    [SerializeField] private float repairTime = 3f;
    [SerializeField] private InteractiveModule nextModule;

    [Header("Настройки награды")]
    [SerializeField] private int repReward = 10;
    [SerializeField] private float angerDown = 5f;

    public override void Interact()
    {
        if (!isActive) return;

        Debug.Log("Начат ремонт");
        StartCoroutine(UnlockCoroutine());
    }

    private IEnumerator UnlockCoroutine()
    {
        Deactivate();
        yield return new WaitForSeconds(repairTime);

        if (QuestManager.Instance != null && QuestManager.Instance.isQuestActive)
        {
            QuestManager.Instance.MarkTaskAsDone(repReward, angerDown);
        }

        Debug.Log("Объект приведен в порядок");

        if (nextModule != null)
            nextModule.Activate();
        else
            Debug.LogError("Next Module не назначен!");
    }


}