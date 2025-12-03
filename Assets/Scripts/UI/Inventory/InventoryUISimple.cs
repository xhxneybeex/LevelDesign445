using System.Collections.Generic;
using UnityEngine;

public class InventoryUISimple : MonoBehaviour
{
    [Header("Refs")]
    public InventorySimple inventory;
    public Transform gridParent;    // has GridLayoutGroup
    public GameObject slotPrefab;   // prefab with an Image + UISlotSimple

    [System.Serializable]
    public class ItemIcon
    {
        public ItemType type;
        public Sprite sprite;
    }

    [Header("Icons")]
    public ItemIcon[] itemIcons;    // assign in inspector (Coin → coin sprite)

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

    Sprite GetSpriteFor(ItemType type)
    {
        if (type == ItemType.None) return null;

        foreach (var entry in itemIcons)
        {
            if (entry.type == type)
                return entry.sprite;
        }

        return null;
    }

    void Refresh()
    {
        if (!inventory) return;

        for (int i = 0; i < uiSlots.Count; i++)
        {
            ItemType itemType = ItemType.None;

            if (inventory.slots != null && i < inventory.slots.Length)
                itemType = inventory.slots[i];

            Sprite sprite = GetSpriteFor(itemType);
            bool filled = itemType != ItemType.None;

            uiSlots[i].ShowItem(sprite, filled);
        }
    }
}
