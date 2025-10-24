using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryRoot;      // your Panel
    public bool pauseGameWhileOpen = false;
    public MonoBehaviour[] disableWhenOpen; 

    public static bool InventoryOpen { get; private set; }

    void Start() => SetOpen(false, true);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            SetOpen(!InventoryOpen, true);
    }

    public void SetOpen(bool open, bool applyTimeScale)
    {
        InventoryOpen = open;

        if (inventoryRoot) inventoryRoot.SetActive(open);

        if (disableWhenOpen != null)
            foreach (var mb in disableWhenOpen) if (mb) mb.enabled = !open;

        if (open)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (applyTimeScale && pauseGameWhileOpen) Time.timeScale = 0f;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (applyTimeScale) Time.timeScale = 1f;
        }
    }
}
