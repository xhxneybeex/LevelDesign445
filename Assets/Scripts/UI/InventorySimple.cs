using System;
using UnityEngine;

public class InventorySimple : MonoBehaviour
{
    public int slotCount = 12;
    public bool[] filled;
    public event Action OnChanged;

    void Awake()
    {
        filled = new bool[Mathf.Max(1, slotCount)];
        Notify();
    }

    // fill next empty slot, return true if success
    public bool AddOne()
    {
        for (int i = 0; i < filled.Length; i++)
        {
            if (!filled[i])
            {
                filled[i] = true;
                Notify();
                return true;
            }
        }
        return false; // inventory full
    }

    public void ClearSlot(int index)
    {
        if (index < 0 || index >= filled.Length) return;
        filled[index] = false;
        Notify();
    }

    void Notify() => OnChanged?.Invoke();
}
