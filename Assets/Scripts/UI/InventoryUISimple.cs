using System.Collections.Generic;
using UnityEngine;

public class InventoryUISimple : MonoBehaviour
{
    [Header("Refs")]
    public InventorySimple inventory;
    public Transform gridParent;    // has GridLayoutGroup
    public GameObject slotPrefab;   // prefab with an Image + UISlotSimple
    public Sprite squareSprite;     // your simple square icon

    List<UISlotSimple> uiSlots = new List<UISlotSimple>();

    void OnEnable()
    {
        if (inventory) inventory.OnChanged += Refresh;
        Build();
        Refresh();
    }

    void OnDisable()
    {
        if (inventory) inventory.OnChanged -= Refresh;
    }

    void Build()
    {
        if (!inventory || !gridParent || !slotPrefab) return;

        for (int i = gridParent.childCount - 1; i >= 0; i--)
            Destroy(gridParent.GetChild(i).gameObject);
        uiSlots.Clear();

        for (int i = 0; i < inventory.slotCount; i++)
        {
            var go = Instantiate(slotPrefab, gridParent);
            var slot = go.GetComponent<UISlotSimple>();
            if (!slot) slot = go.AddComponent<UISlotSimple>();
            uiSlots.Add(slot);
        }
    }

    void Refresh()
    {
        if (!inventory) return;
        for (int i = 0; i < uiSlots.Count; i++)
        {
            bool filled = i < inventory.filled.Length && inventory.filled[i];
            uiSlots[i].ShowSquare(squareSprite, filled);
        }
    }
}
