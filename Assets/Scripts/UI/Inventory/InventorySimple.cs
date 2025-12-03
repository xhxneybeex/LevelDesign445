using System;
using UnityEngine;

public class InventorySimple : MonoBehaviour
{
    public int slotCount = 12;
    public ItemType[] slots;
    public event Action OnChanged;

    void Awake()
    {
        slotCount = Mathf.Max(1, slotCount);

        if (slots == null || slots.Length != slotCount)
        {
            ItemType[] oldSlots = slots;
            slots = new ItemType[slotCount];

            if (oldSlots != null)
            {
                for (int i = 0; i < Mathf.Min(oldSlots.Length, slots.Length); i++)
                {
                    slots[i] = oldSlots[i];
                }
            }
        }

        Notify();
    }

    public bool AddItem(ItemType item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == ItemType.None)
            {
                slots[i] = item;
                Notify();
                return true;
            }
        }
        return false;
    }

    public void ClearSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        slots[index] = ItemType.None;
        Notify();
    }

    public bool HasItem(ItemType item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == item) return true;
        }
        return false;
    }

    // 🔹 made public so OTHER scripts can trigger a refresh safely
    public void Notify()
    {
        OnChanged?.Invoke();
    }
}
