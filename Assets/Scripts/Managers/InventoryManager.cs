using System;
using System.Collections.Generic;
using UnityEngine;

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
        OnInventoryChanged?.Invoke();
        return true;
    }

}