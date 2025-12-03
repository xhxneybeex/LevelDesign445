using UnityEngine;
using UnityEngine.UI;

public class UISlotSimple : MonoBehaviour
{
    public Image icon;

    public void ShowSquare(Sprite squareSprite, bool filled)
    {
        if (!icon) icon = GetComponentInChildren<Image>();
        icon.enabled = filled;
        if (filled && squareSprite) icon.sprite = squareSprite;
    }
}
