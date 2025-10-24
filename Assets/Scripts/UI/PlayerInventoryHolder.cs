using UnityEngine;

[RequireComponent(typeof(InventorySimple))]
public class PlayerInventoryHolder : MonoBehaviour
{
    public InventorySimple Inventory { get; private set; }
    void Awake() => Inventory = GetComponent<InventorySimple>();
}
