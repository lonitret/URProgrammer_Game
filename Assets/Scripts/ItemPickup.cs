using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData item;
    [SerializeField] private int amount = 1;
    [SerializeField] private bool hideAfterPickup = true;
    [SerializeField] private string interactionText = "[E] Pick up";

    public void Interact()
    {
        if (item == null)
        {
            Debug.LogWarning($"{name}: pickup item is not assigned.");
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager is missing.");
            return;
        }

        if (InventoryManager.Instance.AddItem(item, Mathf.Max(1, amount)) && hideAfterPickup)
        {
            gameObject.SetActive(false);
        }
    }

    public string GetInteractionText()
    {
        return interactionText;
    }
}
