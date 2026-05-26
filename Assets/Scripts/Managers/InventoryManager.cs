using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public List<InventorySlot> slots = new List<InventorySlot>();
    public int inventorySize = 20;

    public static InventoryManager Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public static event Action OnInventoryChanged;

    [System.Serializable]
    public class InventorySlot
    {
        public ItemData item;
        public int count;

        public InventorySlot(ItemData newItem, int amount)
        {
            item = newItem;
            count = amount;
        }
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null) return false;

        foreach (var slot in slots)
        {
            if (slot.item == item)
            {
                slot.count += amount;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        if (slots.Count < inventorySize)
        {
            slots.Add(new InventorySlot(item, amount));
            OnInventoryChanged?.Invoke();
            return true;
        }

        Debug.Log("Инвентарь заполнен!");
        return false;
    }

    public bool HasItem(ItemData item)
    {
        foreach (var slot in slots)
        {
            if (slot.item == item && slot.count > 0) return true;
        }
        return false;
    }

    public void RemoveItem(ItemData item, int amount = 1)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].count -= amount;
                if (slots[i].count <= 0)
                {
                    slots.RemoveAt(i);
                }
                OnInventoryChanged?.Invoke();
                return;
            }
        }
    }

    public void OnUseItem(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UseSelectedItem();
        }
    }

    public void UseSelectedItem()
    {
        foreach (var slot in slots)
        {
            if (slot.item != null && slot.item.isCoffee)
            {
                Debug.Log($"Выпили {slot.item.itemName}. Стресс снижен на {slot.item.stressRelief}!");

                if (StatsManager.Instance != null)
                {
                    StatsManager.Instance.ChangeAnger(-slot.item.stressRelief);
                }

                RemoveItem(slot.item, 1);
                return;
            }
        }

        Debug.Log("У вас нет кофе в инвентаре, чтобы его выпить!");
    }
}