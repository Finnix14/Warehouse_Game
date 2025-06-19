// FN_InventoryItem.cs
using UnityEngine;

// Attach this to invisible pickups (no Rigidbody, not dragged)
public class FN_InventoryItem : MonoBehaviour
{
    [Header("Inventory Settings")]
    public string st_itemID = "";           // e.g., "Keycard"
    public int i_amount = 1;
    public bool bl_destroyOnPickup = true;

    [Header("Pickup Settings")]
    public bool bl_canPickup = true;

    public void Pickup()
    {
        if (!bl_canPickup || string.IsNullOrEmpty(st_itemID)) return;

        FN_InventoryManager.Instance.AddItem(st_itemID, i_amount);

        Debug.Log($"[InventoryItem] Picked up {i_amount}x {st_itemID}");

        if (bl_destroyOnPickup)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}
