using UnityEngine;
using TMPro;

public class WorldHintHandler : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 1.2f, 0);
    private Canvas canvas;
    private TextMeshProUGUI hintText;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        hintText = GetComponentInChildren<TextMeshProUGUI>();
        canvas.enabled = false;
    }

    void OnEnable() => InteractionDetector.OnInteractableFound += HandleHint;
    void OnDisable() => InteractionDetector.OnInteractableFound -= HandleHint;

    private void HandleHint(Transform target, string text)
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            hintText.text = text;
            canvas.enabled = true;
        }
        else
        {
            canvas.enabled = false;
        }
    }
}