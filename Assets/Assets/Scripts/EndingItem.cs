using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// EndingItem: Saat player mengambil item ini, layar akan perlahan menjadi putih,
/// sprite berputar lalu berhenti, lalu muncul tulisan "Thanks for Playing".
/// Tempelkan script ini ke objek collectible "ending" di world.
/// </summary>
public class EndingItem : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private float rotationalSpeed = 50f;
    [SerializeField] private AudioClip collectSound;

    [Header("Ending UI (Drag & Drop dari Canvas)")]
    [Tooltip("Panel Image putih full-screen (awalnya transparent, alpha = 0)")]
    [SerializeField] private Image whiteFlashPanel;

    [Tooltip("TextMeshPro untuk tulisan 'Thanks for Playing'")]
    [SerializeField] private TextMeshProUGUI thanksText;

    [Tooltip("TextMeshPro untuk tulisan 'Click anywhere to back to lobby'")]
    [SerializeField] private TextMeshProUGUI clickToLobbyText;

    [Header("Spinning Sprite")]
    [Tooltip("Image UI yang akan berputar saat ending (drag dari Canvas)")]
    [SerializeField] private Image spinningSprite;

    [Tooltip("Jumlah putaran penuh sebelum berhenti")]
    [SerializeField] private int spinRotations = 5;

    [Tooltip("Durasi total berputar (dalam detik)")]
    [SerializeField] private float spinDuration = 3f;

    [Header("Timing Settings")]
    [Tooltip("Durasi layar fade menjadi putih (dalam detik)")]
    [SerializeField] private float fadeToWhiteDuration = 3f;

    [Tooltip("Jeda setelah layar putih sebelum sprite mulai berputar")]
    [SerializeField] private float delayBeforeSpin = 0.5f;

    [Tooltip("Jeda setelah sprite berhenti sebelum tulisan muncul")]
    [SerializeField] private float delayBeforeText = 1.5f;

    [Tooltip("Durasi tulisan fade in")]
    [SerializeField] private float textFadeInDuration = 2f;

    private bool isCollected = false;

    private void Start()
    {
        // Matikan timer di stage ini
        Timer timer = FindAnyObjectByType<Timer>();
        if (timer != null)
        {
            timer.StopTimer();
            Debug.Log("[EndingItem] Timer dimatikan untuk stage ending.");
        }
    }

    void Update()
    {
        // Rotasi item di world
        transform.Rotate(0, rotationalSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            isCollected = true;

            // Mainkan SFX
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // Bekukan player agar tidak bisa gerak lagi
            FreezePlayer(other.transform);

            // Sembunyikan model item
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null) meshRenderer.enabled = false;

            // Sembunyikan collider agar tidak bisa di-trigger lagi
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Mulai ending sequence
            StartCoroutine(EndingSequence());
        }
    }

    private void FreezePlayer(Transform playerTransform)
    {
        // Hentikan movement
        Movement movement = playerTransform.GetComponent<Movement>() 
            ?? playerTransform.GetComponentInParent<Movement>();
        if (movement != null)
            movement.enabled = false;

        // Hentikan kamera
        PlayerCam playerCam = playerTransform.GetComponent<PlayerCam>() 
            ?? playerTransform.GetComponentInParent<PlayerCam>()
            ?? FindAnyObjectByType<PlayerCam>();
        if (playerCam != null)
            playerCam.SetLocked(true);

        // Sembunyikan cursor selama cutscene
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private IEnumerator EndingSequence()
    {
        // Pastikan panel sudah di-setup
        if (whiteFlashPanel == null)
        {
            Debug.LogError("[EndingItem] whiteFlashPanel belum di-assign di Inspector!");
            yield break;
        }

        // Pastikan panel aktif tapi transparan
        whiteFlashPanel.gameObject.SetActive(true);
        whiteFlashPanel.color = new Color(1f, 1f, 1f, 0f);

        // Sembunyikan teks dan sprite dulu
        if (thanksText != null)
            thanksText.gameObject.SetActive(false);

        if (clickToLobbyText != null)
            clickToLobbyText.gameObject.SetActive(false);

        if (spinningSprite != null)
        {
            spinningSprite.gameObject.SetActive(false);
            spinningSprite.color = new Color(1f, 1f, 1f, 0f);
        }

        // ===== FASE 1: Fade layar menjadi putih =====
        float elapsed = 0f;
        while (elapsed < fadeToWhiteDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeToWhiteDuration);

            // Ease-in: mulai lambat, makin cepat di akhir
            float easedAlpha = alpha * alpha;

            whiteFlashPanel.color = new Color(1f, 1f, 1f, easedAlpha);
            yield return null;
        }

        // Pastikan fully white
        whiteFlashPanel.color = new Color(1f, 1f, 1f, 1f);

        // ===== FASE 2: Jeda lalu fade in tulisan "Thanks for Playing" =====
        yield return new WaitForSeconds(delayBeforeText);

        if (thanksText != null)
        {
            thanksText.gameObject.SetActive(true);
            thanksText.color = new Color(thanksText.color.r, thanksText.color.g, thanksText.color.b, 0f);

            elapsed = 0f;
            while (elapsed < textFadeInDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsed / textFadeInDuration);

                thanksText.color = new Color(
                    thanksText.color.r,
                    thanksText.color.g,
                    thanksText.color.b,
                    alpha
                );
                yield return null;
            }

            thanksText.color = new Color(
                thanksText.color.r,
                thanksText.color.g,
                thanksText.color.b,
                1f
            );
        }

        // ===== FASE 3: Jeda lalu Spinning Sprite (dari kecil → membesar sambil berputar → berhenti) =====
        yield return new WaitForSeconds(delayBeforeSpin);

        if (spinningSprite != null)
        {
            spinningSprite.gameObject.SetActive(true);

            // Mulai dari ukuran 0 dan transparan
            spinningSprite.rectTransform.localScale = Vector3.zero;
            spinningSprite.color = new Color(1f, 1f, 1f, 0f);

            float totalAngle = spinRotations * 360f;
            elapsed = 0f;
            float startRotZ = spinningSprite.rectTransform.localEulerAngles.z;

            while (elapsed < spinDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / spinDuration);

                // Ease-out: cepat di awal, melambat di akhir
                float easedT = 1f - (1f - t) * (1f - t);

                // Scale: membesar dari 0 ke 1
                float scale = easedT;
                spinningSprite.rectTransform.localScale = new Vector3(scale, scale, 1f);

                // Rotasi: berputar sambil melambat
                float currentAngle = startRotZ + totalAngle * easedT;
                spinningSprite.rectTransform.localEulerAngles = new Vector3(0f, 0f, -currentAngle);

                // Opacity: fade in cepat di awal
                float alpha = Mathf.Clamp01(t * 4f); // Fully visible di 25% durasi
                spinningSprite.color = new Color(1f, 1f, 1f, alpha);

                yield return null;
            }

            // Pastikan berhenti di ukuran penuh dan posisi akhir
            spinningSprite.rectTransform.localScale = Vector3.one;
            spinningSprite.rectTransform.localEulerAngles = new Vector3(0f, 0f, -(startRotZ + totalAngle));
            spinningSprite.color = new Color(1f, 1f, 1f, 1f);
        }

        // Tampilkan cursor di akhir
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // ===== FASE 4: Fade in tulisan "Click anywhere to back to lobby" =====
        if (clickToLobbyText != null)
        {
            clickToLobbyText.gameObject.SetActive(true);
            clickToLobbyText.color = new Color(
                clickToLobbyText.color.r,
                clickToLobbyText.color.g,
                clickToLobbyText.color.b,
                0f
            );

            elapsed = 0f;
            float lobbyTextFadeIn = 1.5f;
            while (elapsed < lobbyTextFadeIn)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsed / lobbyTextFadeIn);
                clickToLobbyText.color = new Color(
                    clickToLobbyText.color.r,
                    clickToLobbyText.color.g,
                    clickToLobbyText.color.b,
                    alpha
                );
                yield return null;
            }

            clickToLobbyText.color = new Color(
                clickToLobbyText.color.r,
                clickToLobbyText.color.g,
                clickToLobbyText.color.b,
                1f
            );
        }

        Debug.Log("[EndingItem] Menunggu klik mouse untuk kembali ke Lobby...");

        // ===== FASE 5: Tunggu klik mouse lalu pindah ke Lobby =====
        // Tunggu 1 frame agar klik yang sudah tertahan tidak langsung ter-trigger
        yield return null;

        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        // Reset inventory dan pindah ke Lobby
        InventoryScript.ResetSavedInventory();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }
}
