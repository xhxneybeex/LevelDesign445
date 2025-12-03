using UnityEngine;
using UnityEngine.UI;

public class UISlotSimple : MonoBehaviour
{
    public Image icon;

    public void ShowItem(Sprite sprite, bool filled)
    {
        if (!icon) icon = GetComponentInChildren<Image>();
        if (!icon) return;

        icon.enabled = filled;

        if (filled)
        {
            icon.sprite = sprite;
        }
    }
}
