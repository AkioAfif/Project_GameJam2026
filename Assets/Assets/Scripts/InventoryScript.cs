using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript
{
    // Step 1: Fixed-size Array instead of a dynamic List
    private ItemScript[] itemList;

    // ==================== PERSISTENCE ====================
    // Static variable untuk menyimpan inventory antar scene
    private static ItemScript.ItemType?[] savedInventory = null;

    public InventoryScript()
    {
        // Initialize the array with exactly 4 empty slots
        itemList = new ItemScript[4];

        // Jika ada data inventory yang disimpan dari scene sebelumnya, restore
        if (savedInventory != null)
        {
            for (int i = 0; i < savedInventory.Length && i < itemList.Length; i++)
            {
                if (savedInventory[i].HasValue)
                {
                    itemList[i] = new ItemScript
                    {
                        itemType = savedInventory[i].Value,
                        amount = 1
                    };
                    Debug.Log($"[Inventory] Restored item {savedInventory[i].Value} di slot {i}.");
                }
            }
            savedInventory = null; // Reset setelah di-restore
            Debug.Log("[Inventory] Inventory berhasil di-restore dari scene sebelumnya.");
        }
    }

    public bool AddItem(ItemScript item)
    {
        // Step 2: The Smart Pick-Up Logic
        for (int i = 0; i < itemList.Length; i++)
        {
            // Check if the current slot is empty
            if (itemList[i] == null)
            {
                // Place the item in this empty slot
                itemList[i] = item;
                Debug.Log($"Added {item.itemType} to inventory slot {i}.");

                // Return true = item successfully added
                return true;
            }
        }

        // If the loop finishes checking all 4 slots and never returns, it means we are full.
        Debug.Log("Inventory is full! Cannot pick up item.");
        return false;
    }

    /// <summary>
    /// Cek apakah inventory penuh (semua 4 slot terisi).
    /// </summary>
    public bool IsFull()
    {
        for (int i = 0; i < itemList.Length; i++)
        {
            if (itemList[i] == null)
                return false;
        }
        return true;
    }

    // Update the getter to return the array instead of a List
    public ItemScript[] GetItemList()
    {
        return itemList;
    }

    /// <summary>
    /// Simpan inventory saat ini ke static variable untuk dibawa ke scene berikutnya.
    /// Panggil sebelum LoadScene().
    /// </summary>
    public void SaveForNextScene()
    {
        savedInventory = new ItemScript.ItemType?[itemList.Length];
        int savedCount = 0;

        for (int i = 0; i < itemList.Length; i++)
        {
            if (itemList[i] != null)
            {
                savedInventory[i] = itemList[i].itemType;
                savedCount++;
            }
            else
            {
                savedInventory[i] = null;
            }
        }

        Debug.Log($"[Inventory] {savedCount} item(s) disimpan untuk scene berikutnya.");
    }

    /// <summary>
    /// Reset saved inventory (misal saat mulai game baru dari awal).
    /// </summary>
    public static void ResetSavedInventory()
    {
        savedInventory = null;
    }
}