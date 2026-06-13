using TMPro;
using UnityEngine;

public class QuestMarkerWorldHandler : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.65f, 0f);
    [SerializeField] private string markerSymbol = "!";
    [SerializeField] private float refreshInterval = 0.15f;

    private Canvas canvas;
    private TextMeshProUGUI markerText;
    private Transform currentTarget;
    private float refreshTimer;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        markerText = GetComponentInChildren<TextMeshProUGUI>();

        if (canvas != null)
        {
            canvas.enabled = false;
        }

        if (markerText != null)
        {
            markerText.text = markerSymbol;
        }
    }

    private void OnEnable()
    {
        RefreshMarkerTarget();
    }

    private void Update()
    {
        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer <= 0f)
        {
            refreshTimer = refreshInterval;
            RefreshMarkerTarget();
        }
    }

    private void LateUpdate()
    {
        if (currentTarget != null && canvas != null && canvas.enabled)
        {
            transform.position = currentTarget.position + offset;
        }
    }

    private void RefreshMarkerTarget()
    {
        currentTarget = FindMarkerTarget();

        if (currentTarget != null)
        {
            transform.position = currentTarget.position + offset;
            if (markerText != null) markerText.text = markerSymbol;
            if (canvas != null) canvas.enabled = true;
        }
        else if (canvas != null)
        {
            canvas.enabled = false;
        }
    }

    private Transform FindMarkerTarget()
    {
        NPCQuestGiver[] givers = FindObjectsOfType<NPCQuestGiver>();

        if (QuestManager.Instance != null && QuestManager.Instance.isQuestActive)
        {
            foreach (NPCQuestGiver giver in givers)
            {
                if (giver != null && QuestManager.Instance.IsCurrentQuestGiver(giver))
                {
                    if (QuestManager.Instance.isTaskCompleted) return giver.transform;

                    Transform objectiveTarget = giver.GetObjectiveMarkerTarget();
                    return objectiveTarget != null ? objectiveTarget : null;
                }
            }

            return null;
        }

        NPCQuestGiver best = null;
        foreach (NPCQuestGiver giver in givers)
        {
            if (giver == null || !giver.ShouldShowAvailableQuestMarker()) continue;

            if (best == null || giver.RequiredCompletedQuestsToOffer < best.RequiredCompletedQuestsToOffer)
            {
                best = giver;
            }
        }

        return best != null ? best.transform : null;
    }
}
