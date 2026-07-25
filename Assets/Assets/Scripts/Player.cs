using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private UI_Inventory uiInventory;
    [SerializeField] private SkillManager skillManager;
    private InventoryScript inventory;

    private void Start()
    {
        inventory = new InventoryScript();
        uiInventory.SetInventory(inventory);
    }

    public bool PickUpItem(ItemScript.ItemType itemType)
    {
        // Create a new item instance with the type passed from the collectible
        ItemScript newItem = new ItemScript { itemType = itemType, amount = 1 };

        // Try to add it to the inventory backend
        bool success = inventory.AddItem(newItem);
        if (success)
        {
            // Pull the Trigger! Tell the UI to instantly redraw itself
            uiInventory.RefreshInventoryItems();
        }
        return success;
    }

    /// <summary>
    /// Cek apakah inventory player penuh.
    /// </summary>
    public bool IsInventoryFull()
    {
        return inventory != null && inventory.IsFull();
    }

    /// <summary>
    /// Simpan inventory untuk dibawa ke scene berikutnya.
    /// Dipanggil oleh TeleportPlate sebelum LoadScene.
    /// </summary>
    public void SaveInventory()
    {
        if (inventory != null)
        {
            inventory.SaveForNextScene();
        }
    }

    private void Update()
    {
        KeyCode[] hotkeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
        ItemScript[] items = inventory.GetItemList();

        for (int i = 0; i < hotkeys.Length && i < items.Length; i++)
        {
            if (Input.GetKeyDown(hotkeys[i]) && items[i] != null)
            {
                // Only consume the item if the skill was successfully activated
                if (skillManager.ActivateSkill(items[i].itemType))
                {
                    items[i] = null;
                    uiInventory.RefreshInventoryItems();
                }
                break;
            }
        }
    }
}