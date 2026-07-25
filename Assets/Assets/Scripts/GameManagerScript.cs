using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject pauseUI;

    [Header("Script References")]
    [SerializeField] private Timer gameTimer;

    private bool isPaused = false;

    void Start()
    {
        // Pastikan pauseUI tidak aktif saat game mulai
        if (pauseUI != null)
            pauseUI.SetActive(false);
    }

    void Update()
    {
        // Deteksi tombol Escape untuk toggle Pause
        // Tidak bisa pause jika game over sedang aktif ATAU main menu masih terbuka
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOverUI.activeInHierarchy && !MainMenuScript.IsOpen)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        // ── CURSOR MANAGEMENT (satu tempat, satu otoritas) ──────────────────
        // Prioritas (tertinggi ke terendah):
        //   1. Main Menu terbuka  → visible, bebas
        //   2. Game Over aktif    → visible, bebas
        //   3. Paused             → visible, bebas
        //   4. Gameplay normal    → tersembunyi, terkunci
        if (MainMenuScript.IsOpen || gameOverUI.activeInHierarchy || isPaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // ==================== GAME OVER ====================

    public void GameOver()
    {
        gameOverUI.SetActive(true);

        // Bekukan semua pergerakan (player tidak bisa gerak/noleh)
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        // Pastikan Time.timeScale normal sebelum restart
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        // Pastikan Time.timeScale normal sebelum pindah scene
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // ==================== PAUSE ====================

    public void PauseGame()
    {
        isPaused = true;

        // Tampilkan UI Pause
        if (pauseUI != null)
            pauseUI.SetActive(true);

        // Freeze timer melalui Timer script (jika ada)
        if (gameTimer != null)
            gameTimer.isTimeFrozen = true;

        // Hentikan waktu game (freeze semua physics, animation, dll)
        Time.timeScale = 0f;

        Debug.Log("Game Paused.");
    }

    public void ResumeGame()
    {
        isPaused = false;

        // Sembunyikan UI Pause
        if (pauseUI != null)
            pauseUI.SetActive(false);

        // Lanjutkan timer kembali (jika ada)
        if (gameTimer != null)
            gameTimer.isTimeFrozen = false;

        // Kembalikan waktu game
        Time.timeScale = 1f;

        Debug.Log("Game Resumed.");
    }

    public void ExitStage()
    {
        // Pastikan Time.timeScale normal sebelum pindah scene
        Time.timeScale = 1f;
        isPaused = false;

        // Tampilkan dan bebaskan kursor sebelum kembali ke MainMenu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("Lobby");
    }
}