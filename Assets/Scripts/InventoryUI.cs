using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform container;

    private int selectedSlotIndex = 0;
    private List<UI_InventorySlot> uiSlots = new List<UI_InventorySlot>();

    void Start()
    {
        InitializeUI();
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame) SelectSlot(0);
        if (keyboard.digit2Key.wasPressedThisFrame) SelectSlot(1);
        if (keyboard.digit3Key.wasPressedThisFrame) SelectSlot(2);
        if (keyboard.digit4Key.wasPressedThisFrame) SelectSlot(3);
        if (keyboard.digit5Key.wasPressedThisFrame) SelectSlot(4);
        //if (keyboard.digit6Key.wasPressedThisFrame) SelectSlot(5);
    }

    private void InitializeUI()
    {
        foreach (Transform child in container) Destroy(child.gameObject);

        int size = InventoryManager.Instance.inventorySize;
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(slotPrefab, container);
            UI_InventorySlot uiSlot = obj.GetComponent<UI_InventorySlot>();
            uiSlots.Add(uiSlot);
            uiSlot.Render(null);
            uiSlot.SetSelected(i == selectedSlotIndex);
        }
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= uiSlots.Count) return;

        uiSlots[selectedSlotIndex].SetSelected(false);
        selectedSlotIndex = index;
        uiSlots[selectedSlotIndex].SetSelected(true);
    }

    public void Refresh()
    {
        var slotsData = InventoryManager.Instance.slots;
        for (int i = 0; i < uiSlots.Count; i++)
        {
            if (i < slotsData.Count)
                uiSlots[i].Render(slotsData[i]);
            else
                uiSlots[i].Render(null);
        }
    }
}