using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportPlate : MonoBehaviour
{
    [Header("Teleport Settings")]
    [Tooltip("Nama scene selanjutnya yang akan dituju")]
    [SerializeField] private string nextSceneName;
    
    [Tooltip("Waktu tunggu sebelum teleport (dalam detik)")]
    [SerializeField] private float teleportDelay = 5f;

    [Header("Visual Effects (VFX)")]
    [Tooltip("Masukkan Prefab VFX (Particle System) untuk teleport")]
    [SerializeField] private GameObject teleportVfxPrefab;
    
    [Tooltip("Titik munculnya VFX. Kosongkan jika ingin muncul tepat di tengah plate")]
    [SerializeField] private Transform vfxSpawnPoint;

    [Header("Audio Settings (SFX)")]
    [Tooltip("Masukkan file SFX untuk teleport ke sini")]
    [SerializeField] private AudioClip teleportSfx;
    
    private AudioSource audioSource;

    [Header("Animation Settings (Opsional)")]
    [Tooltip("Masukkan Animator player jika ingin mentrigger animasi karakter")]
    [SerializeField] private Animator playerAnimator; 
    [SerializeField] private string teleportAnimationTrigger = "Teleport"; 

    // Untuk mencegah trigger berulang kali saat jeda
    private bool isTeleporting = false; 

    private void Start()
    {
        // Mengambil atau menambahkan komponen AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Pastikan yang menginjak adalah Player dan belum dalam proses teleport
        if (other.CompareTag("Player") && !isTeleporting)
        {
            StartCoroutine(TeleportRoutine(other.transform));
        }
    }

    private IEnumerator TeleportRoutine(Transform playerTransform)
    {
        isTeleporting = true; // Tandai bahwa sedang proses teleport

        // 0. Hentikan timer saat player sampai di plate
        Timer timer = FindAnyObjectByType<Timer>();
        if (timer != null)
        {
            timer.StopTimer();
        }

        // 1. Munculkan VFX
        if (teleportVfxPrefab != null)
        {
            // Menentukan posisi VFX: jika vfxSpawnPoint diisi, gunakan itu. 
            // Jika tidak, munculkan VFX tepat di posisi player atau plate.
            Vector3 spawnPosition = vfxSpawnPoint != null ? vfxSpawnPoint.position : playerTransform.position;
            
            // Memunculkan efek visual
            Instantiate(teleportVfxPrefab, spawnPosition, Quaternion.identity);
        }

        // 2. Mainkan SFX
        if (teleportSfx != null)
        {
            audioSource.PlayOneShot(teleportSfx);
        }

        // 3. Mainkan animasi karakter (opsional)
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(teleportAnimationTrigger);
        }

        // 4. Jeda waktu tunggu (misal: 5 detik) sambil menunggu animasi/VFX selesai
        yield return new WaitForSeconds(teleportDelay);

        // 5. Simpan waktu + bonus 5 detik untuk scene berikutnya
        if (timer != null)
        {
            timer.SaveTimeForNextScene(5f);
        }

        // 6. Pindah scene
        SceneManager.LoadScene(nextSceneName);
    }
}
