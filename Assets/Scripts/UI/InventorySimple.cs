using System;
using UnityEngine;

public class InventorySimple : MonoBehaviour
{
    public int slotCount = 12;
    public bool[] filled;
    public event Action OnChanged;

    void Awake()
    {
        // sets up slots when the game starts
        filled = new bool[Mathf.Max(1, slotCount)];
        Notify();
    }

    public bool AddOne()
    {
        // finds the first empty slot and fills it
        for (int i = 0; i < filled.Length; i++)
        {
            if (!filled[i])
            {
                filled[i] = true;
                Notify();
                return true;
            }
        }

        // all slots full
        return false;
    }

    public void ClearSlot(int index)
    {
        // empties a specific slot
        if (index < 0 || index >= filled.Length) return;
        filled[index] = false;
        Notify();
    }

    void Notify()
    {
        // tells the UI that inventory changed
        OnChanged?.Invoke();
    }
}
