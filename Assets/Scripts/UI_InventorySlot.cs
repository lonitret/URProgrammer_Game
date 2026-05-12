using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_InventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI countText;
    public GameObject selectionFrame;

    public void Render(InventoryManager.InventorySlot slot)
    {
        if (slot != null && slot.item != null)
        {
            icon.sprite = slot.item.icon;
            icon.enabled = true;
            countText.text = slot.count > 1 ? slot.count.ToString() : "";
        }
        else
        {
            icon.enabled = false;
            countText.text = "";
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionFrame != null)
            selectionFrame.SetActive(isSelected);
    }
}