using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// CurrentStageStatus: Menampilkan nomor stage saat ini ke UI Text.
/// Membaca nama scene yang sedang berjalan (format: "Lvl1" - "Lvl15").
/// Tempelkan script ini ke GameObject yang punya TextMeshProUGUI,
/// atau assign TextMeshProUGUI target via Inspector.
/// </summary>
public class CurrentStageStatus : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Drag & drop TextMeshProUGUI yang akan menampilkan nomor stage")]
    [SerializeField] private TextMeshProUGUI stageText;

    [Header("Display Settings")]
    [Tooltip("Format teks. Gunakan {0} sebagai placeholder nomor stage. Contoh: 'Stage {0}' atau 'Level {0} / 15'")]
    [SerializeField] private string displayFormat = "Stage {0}";

    [Tooltip("Teks yang ditampilkan jika bukan di scene Lvl (misal di Lobby)")]
    [SerializeField] private string fallbackText = "";

    private void Start()
    {
        // Jika belum di-assign, coba ambil dari GameObject ini sendiri
        if (stageText == null)
            stageText = GetComponent<TextMeshProUGUI>();

        UpdateStageDisplay();
    }

    private void UpdateStageDisplay()
    {
        if (stageText == null)
        {
            Debug.LogWarning("[CurrentStageStatus] stageText belum di-assign!");
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        int stageNumber = ParseStageNumber(sceneName);

        if (stageNumber > 0)
        {
            stageText.text = string.Format(displayFormat, stageNumber);
        }
        else
        {
            // Bukan scene Lvl (misalnya Lobby), tampilkan fallback
            stageText.text = fallbackText;
        }
    }

    /// <summary>
    /// Membaca angka dari nama scene format "Lvl1" - "Lvl15".
    /// Mengembalikan 0 jika nama scene tidak sesuai format.
    /// </summary>
    private int ParseStageNumber(string sceneName)
    {
        // Cek apakah nama scene diawali "Lvl" (case-insensitive)
        if (sceneName.Length > 3 && sceneName.ToLower().StartsWith("lvl"))
        {
            string numberPart = sceneName.Substring(3); // Ambil bagian setelah "Lvl"
            if (int.TryParse(numberPart, out int number))
            {
                return number;
            }
        }

        // Juga cek format "MazeLvl1" - "MazeLvl6" untuk kompatibilitas scene lama
        if (sceneName.ToLower().StartsWith("mazelvl"))
        {
            string numberPart = sceneName.Substring(7);
            if (int.TryParse(numberPart, out int number))
            {
                return number;
            }
        }

        return 0; // Tidak ditemukan angka
    }
}
