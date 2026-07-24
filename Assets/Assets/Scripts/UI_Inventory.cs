using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    private InventoryScript inventory;

    [Header("UI References")]
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;

    public void SetInventory(InventoryScript inventory)
    {
        this.inventory = inventory;
        RefreshInventoryItems();
    }

    public void RefreshInventoryItems()
    {
        // Pengaman jika belum ter-assign
        if (itemSlotContainer == null || itemSlotTemplate == null)
        {
            return;
        }

        // Clean the slate before drawing to prevent duplicates
        foreach (Transform child in itemSlotContainer)
        {
            // Skip the master template so it doesn't get destroyed
            if (child == itemSlotTemplate) continue;

            Destroy(child.gameObject);
        }

        int x = 0;
        int y = 0;
        float itemSlotCellSize = 80f;

        // Step 4: Get the fixed Array from the backend
        ItemScript[] itemList = inventory.GetItemList();

        // Always loop exactly the length of the array (4 times)
        for (int i = 0; i < itemList.Length; i++)
        {
            ItemScript item = itemList[i];

            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);

            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, -y * itemSlotCellSize);

            Image image = itemSlotRectTransform.Find("Images").GetComponent<Image>();

            // The Visual Safety Check
            if (item != null)
            {
                // The slot is full, show the item graphic
                image.gameObject.SetActive(true);
                image.sprite = item.GetSprite();
            }
            else
            {
                // The slot is empty (null), hide the item graphic
                image.gameObject.SetActive(false);
            }

            x++;
            if (x > 4)
            {
                x = 0;
                y++;
            }
        }
    }
}