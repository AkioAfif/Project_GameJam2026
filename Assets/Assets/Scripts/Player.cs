using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private UI_Inventory uiInventory;
    [SerializeField] private SkillManager skillManager;
    private InventoryScript inventory;

    private void Start()
    {
        inventory = new InventoryScript();
        if (uiInventory != null)
        {
            uiInventory.SetInventory(inventory);
        }
    }

    /// <summary>
    /// Mengambil nomor level dari nama scene (misal MazeLvl1 -> 1, MazeLvl6 -> 6, Level 15 -> 15)
    /// </summary>
    public static int GetLevelFromSceneName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return 1;
        System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(sceneName, @"\d+");
        if (match.Success && int.TryParse(match.Value, out int level))
        {
            return level;
        }
        return 1; // Fallback jika tidak ditemukan angka di nama scene
    }

    /// <summary>
    /// Mengambil nomor level saat ini dari scene yang sedang aktif
    /// </summary>
    public static int GetCurrentLevel()
    {
        return GetLevelFromSceneName(SceneManager.GetActiveScene().name);
    }

    public bool PickUpItem(ItemScript.ItemType itemType)
    {
        int currentLevel = GetCurrentLevel();

        // Level 1 sampai 5: PowerUp langsung terpakai secara instan (tidak masuk item bar)
        if (currentLevel < 6)
        {
            if (skillManager != null)
            {
                bool activated = skillManager.ActivateSkill(itemType);
                Debug.Log($"[Player] Level {currentLevel}: PowerUp {itemType} langsung terpakai secara otomatis!");
                return activated;
            }
            return false;
        }

        // Level 6+: PowerUp disimpan di Item Bar
        if (currentLevel == 6 && uiInventory != null)
        {
            uiInventory.CheckAndShowLevel6Tutorial();
        }

        ItemScript newItem = new ItemScript { itemType = itemType, amount = 1 };
        bool success = inventory.AddItem(newItem);
        if (success && uiInventory != null)
        {
            uiInventory.RefreshInventoryItems();
        }
        return success;
    }

    /// <summary>
    /// Cek apakah inventory player penuh (hanya berlaku di level 6 ke atas)
    /// </summary>
    public bool IsInventoryFull()
    {
        // Level 1 sampai 5 tidak menggunakan tempat penyimpanan inventory
        if (GetCurrentLevel() < 6) return false;

        return inventory != null && inventory.IsFull();
    }

    /// <summary>
    /// Simpan inventory untuk dibawa ke scene berikutnya (hanya berlaku di level 6 ke atas)
    /// </summary>
    public void SaveInventory()
    {
        if (inventory != null && GetCurrentLevel() >= 6)
        {
            inventory.SaveForNextScene();
        }
    }

    private void Update()
    {
        // Hotkey 1-4 hanya aktif jika berada di level 6 ke atas
        if (GetCurrentLevel() < 6) return;

        KeyCode[] hotkeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
        ItemScript[] items = inventory.GetItemList();

        for (int i = 0; i < hotkeys.Length && i < items.Length; i++)
        {
            if (Input.GetKeyDown(hotkeys[i]) && items[i] != null)
            {
                // Jika tutorial sedang aktif, tutup tutorial terlebih dahulu agar game unpause (timeScale = 1)
                if (UI_Inventory.IsTutorialActive && uiInventory != null)
                {
                    uiInventory.CloseLevel6Tutorial();
                }

                // Consume item jika skill berhasil diaktifkan
                if (skillManager.ActivateSkill(items[i].itemType))
                {
                    items[i] = null;
                    if (uiInventory != null)
                    {
                        uiInventory.RefreshInventoryItems();
                    }
                }
                break;
            }
        }
    }
}