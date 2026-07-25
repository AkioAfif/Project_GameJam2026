using UnityEngine;
using TMPro;

public class MainMenuScript : MonoBehaviour
{
    [Header("Main Menu UI")]
    [SerializeField] private GameObject mainMenuPanel;

    [Header("HUD to Hide During Main Menu")]
    [SerializeField] private GameObject timerUI;
    [SerializeField] private GameObject inventoryUI;

    [Header("Player Control References")]
    [SerializeField] private Movement playerMovement;
    [SerializeField] private PlayerCam playerCam;

    private bool isMainMenuActive = true;

    // Static flag agar script lain bisa cek apakah main menu sedang aktif
    public static bool IsOpen { get; private set; } = true;

    void Update()
    {
        // Cursor dikelola sepenuhnya oleh GameManagerScript.
        // Update() ini sengaja dikosongkan untuk menghindari konflik.
    }

    void Start()
    {
        IsOpen = true;

        // Tampilkan main menu
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        // Sembunyikan HUD
        if (timerUI != null)
            timerUI.SetActive(false);

        if (inventoryUI != null)
            inventoryUI.SetActive(false);

        // Bekukan player
        SetPlayerLocked(true);

        // Tampilkan cursor untuk klik tombol
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        isMainMenuActive = false;
        IsOpen = false;

        // Sembunyikan main menu
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        // Tampilkan HUD kembali
        if (timerUI != null)
            timerUI.SetActive(true);

        if (inventoryUI != null)
            inventoryUI.SetActive(true);

        // Bebaskan player agar bisa bergerak
        SetPlayerLocked(false);

        // Kunci cursor untuk first-person gameplay
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void SetPlayerLocked(bool locked)
    {
        // Bekukan / bebaskan kontrol movement
        if (playerMovement != null)
            playerMovement.enabled = !locked;

        // Bekukan / bebaskan kamera mouse look
        if (playerCam != null)
            playerCam.SetLocked(locked);
    }
}
