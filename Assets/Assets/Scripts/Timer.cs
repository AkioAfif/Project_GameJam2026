using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;

    public GameManagerScript gameManager;
    private bool isDead;

    // Public boolean for the SkillManager to control
    public bool isTimeFrozen = false;

    // Static variable untuk menyimpan waktu antar scene
    private static float savedRemainingTime = -1f;

    // Flag untuk menghentikan timer (saat player sampai plate)
    private bool isStopped = false;

    // Nama scene lobby — timer tidak berjalan di sini
    private const string LOBBY_SCENE_NAME = "Lobby";

    /// <summary>
    /// Mendapatkan bonus tambahan waktu (detik) sesuai target level
    /// </summary>
    public static float GetTimeBonusForLevel(int level)
    {
        switch (level)
        {
            case 1: return 5f;
            case 2: return 5f;
            case 3: return 5f;
            case 4: return 5f;
            case 5: return 5f;
            case 6: return 5f;
            case 7: return 30f;
            case 8: return 30f;
            case 9: return 30f;
            case 10: return 30f;
            case 11: return 30f;
            case 12: return 30f;
            case 13: return 45f;
            case 14: return 45f;
            case 15: return 60f; // 1 menit
            default: return 5f;
        }
    }

    void Start()
    {
        // Jika ada waktu tersimpan dari scene sebelumnya, gunakan itu
        if (savedRemainingTime >= 0f)
        {
            remainingTime = savedRemainingTime;
            savedRemainingTime = -1f; // Reset agar hanya dipakai sekali
            Debug.Log("[Timer] Melanjutkan waktu dari scene sebelumnya: " + remainingTime);
        }
        else
        {
            // Jika mulai baru dari Level 1 (misal dari awal game/restart)
            int currentLevel = Player.GetCurrentLevel();
            if (currentLevel == 1)
            {
                remainingTime = GetTimeBonusForLevel(1); // 5 detik
                Debug.Log("[Timer] Level 1 Waktu Awal: " + remainingTime + " detik.");
            }
        }
    }

    void Update()
    {
        // Jika berada di scene Lobby, jangan jalankan countdown.
        // Teks yang muncul adalah apa yang kamu ketik di Unity Editor (Text Input).
        if (SceneManager.GetActiveScene().name == LOBBY_SCENE_NAME)
        {
            return;
        }

        // Hanya hitung mundur jika tidak frozen DAN tidak stopped
        if (!isTimeFrozen && !isStopped)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
            }
            else if (remainingTime <= 0 && !isDead)
            {
                remainingTime = 0;
                timerText.color = Color.red;

                isDead = true;
                gameManager.GameOver();
                Debug.Log("Dead");
            }
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// Hentikan timer (dipanggil saat player sampai di plate)
    /// </summary>
    public void StopTimer()
    {
        isStopped = true;
        Debug.Log("[Timer] Timer dihentikan!");
    }

    /// <summary>
    /// Simpan waktu saat ini + bonus untuk scene berikutnya
    /// </summary>
    public void SaveTimeForNextScene(float bonusTime)
    {
        savedRemainingTime = remainingTime + bonusTime;
        Debug.Log("[Timer] Waktu disimpan untuk scene berikutnya: " + savedRemainingTime + "s (bonus +" + bonusTime + "s)");
    }

    /// <summary>
    /// Reset saved time (misal saat mulai game baru dari awal)
    /// </summary>
    public static void ResetSavedTime()
    {
        savedRemainingTime = -1f;
    }
}