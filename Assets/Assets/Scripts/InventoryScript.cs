using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript
{
    // Step 1: Fixed-size Array instead of a dynamic List
    private ItemScript[] itemList;

    public InventoryScript()
    {
        // Initialize the array with exactly 4 empty slots
        itemList = new ItemScript[4];
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

    // Update the getter to return the array instead of a List
    public ItemScript[] GetItemList()
    {
        return itemList;
    }
}