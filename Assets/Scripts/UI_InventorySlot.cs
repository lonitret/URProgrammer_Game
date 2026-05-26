using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_InventorySlot : MonoBehaviour
{
    public Image iconDisplay;
    public TextMeshProUGUI countText;
    public GameObject selectionFrame;

    public void Render(InventoryManager.InventorySlot slot)
    {
        if (slot != null && slot.item != null && slot.count > 0)
        {
            iconDisplay.sprite = slot.item.icon;
            iconDisplay.enabled = true;

            if (countText != null)
            {
                countText.text = slot.count > 1 ? slot.count.ToString() : "";
                countText.gameObject.SetActive(slot.count > 1);
            }
        }
        else
        {
            if (iconDisplay != null) iconDisplay.enabled = false;
            if (countText != null) countText.gameObject.SetActive(false);
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionFrame != null)
            selectionFrame.SetActive(isSelected);
    }
}