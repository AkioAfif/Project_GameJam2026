using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WallPassEffect: Memungkinkan player menembus tembok tertentu selama durasi terbatas.
/// Drag & drop objek tembok yang bisa ditembus di Inspector.
/// Jika player masih di dalam tembok saat durasi habis, player akan dipukul mundur
/// ke posisi aman terakhir untuk mencegah stuck.
/// </summary>
public class WallPassEffect : MonoBehaviour
{
    [Header("Target Walls")]
    [Tooltip("Drag & drop objek tembok yang bisa ditembus saat skill aktif")]
    [SerializeField] private GameObject[] passableWalls;

    [Header("Pushback Settings")]
    [Tooltip("Kekuatan dorongan mundur jika player masih di dalam tembok")]
    [SerializeField] private float pushbackForce = 15f;

    // Internal tracking
    private bool isActive = false;
    private Vector3 safePosition;
    private Transform playerTransform;
    private Rigidbody playerRb;

    // Semua collider milik player (termasuk children)
    private Collider[] playerColliders;

    // Semua collider milik tembok target
    private List<Collider> allWallColliders = new List<Collider>();

    // Shimmer effect tracking
    private Shader shimmerShader;
    private List<ShimmerData> activeShimmers = new List<ShimmerData>();

    private struct ShimmerData
    {
        public Renderer renderer;
        public Material[] originalMaterials;
        public Material shimmerMaterial;
    }

    private void Awake()
    {
        shimmerShader = Shader.Find("Custom/HoloShimmer");
        if (shimmerShader == null)
        {
            Debug.LogWarning("[WallPassEffect] Shader 'Custom/HoloShimmer' tidak ditemukan! Efek shimmer tidak akan tampil.");
        }
    }

    /// <summary>
    /// Aktifkan efek wall pass. Dipanggil oleh SkillManager.
    /// </summary>
    public void ActivateEffect(float duration, Transform player)
    {
        if (isActive)
        {
            Debug.LogWarning("[WallPassEffect] Skill D sudah aktif! Menunggu selesai...");
            return;
        }

        playerTransform = player;
        playerRb = player.GetComponent<Rigidbody>();

        // Ambil SEMUA collider dari player (termasuk child objects)
        playerColliders = player.GetComponentsInChildren<Collider>();

        if (playerColliders.Length == 0)
        {
            Debug.LogError("[WallPassEffect] Player tidak punya Collider! Skill D tidak bisa diaktifkan.");
            return;
        }

        Debug.Log($"[WallPassEffect] Ditemukan {playerColliders.Length} collider pada player.");

        StartCoroutine(WallPassRoutine(duration));
    }

    private IEnumerator WallPassRoutine(float duration)
    {
        if (passableWalls == null || passableWalls.Length == 0)
        {
            Debug.LogWarning("[WallPassEffect] Tidak ada wall target yang di-assign di Inspector!");
            yield break;
        }

        isActive = true;

        // Simpan posisi aman (posisi player saat skill diaktifkan)
        safePosition = playerTransform.position;

        Debug.Log($"[WallPassEffect] Skill D Activated! Menembus {passableWalls.Length} tembok selama {duration}s.");

        // Kumpulkan semua collider tembok
        CollectWallColliders();

        // Nonaktifkan collision antara SEMUA collider player dan SEMUA collider tembok
        SetCollisionIgnored(true);

        // Tambahkan efek shimmer holographic ke tembok
        ApplyShimmerEffect();

        // Mulai tracking posisi aman selama skill aktif
        StartCoroutine(TrackSafePosition());

        // Tunggu durasi skill
        yield return new WaitForSeconds(duration);

        // Cek apakah player masih di dalam tembok
        bool isStuck = IsOverlappingAnyWall();

        // Aktifkan kembali collision
        SetCollisionIgnored(false);

        // Hapus efek shimmer dari tembok
        RemoveShimmerEffect();

        if (isStuck)
        {
            Debug.LogWarning("[WallPassEffect] Player stuck di dalam tembok! Memukul mundur...");
            PushPlayerToSafety();
        }

        isActive = false;
        allWallColliders.Clear();
        Debug.Log("[WallPassEffect] Skill D Ended. Tembok kembali solid.");
    }

    /// <summary>
    /// Kumpulkan semua Collider dari semua wall target.
    /// </summary>
    private void CollectWallColliders()
    {
        allWallColliders.Clear();

        foreach (GameObject wall in passableWalls)
        {
            if (wall == null) continue;

            Collider[] colliders = wall.GetComponentsInChildren<Collider>();
            allWallColliders.AddRange(colliders);
        }

        Debug.Log($"[WallPassEffect] Total {allWallColliders.Count} wall collider(s) ditemukan.");
    }

    /// <summary>
    /// Toggle collision antara semua collider player dan semua collider tembok.
    /// </summary>
    private void SetCollisionIgnored(bool ignored)
    {
        foreach (Collider playerCol in playerColliders)
        {
            if (playerCol == null) continue;

            foreach (Collider wallCol in allWallColliders)
            {
                if (wallCol == null) continue;
                Physics.IgnoreCollision(playerCol, wallCol, ignored);
            }
        }

        Debug.Log($"[WallPassEffect] Collision ignored = {ignored} untuk {playerColliders.Length} player collider(s) x {allWallColliders.Count} wall collider(s).");
    }

    /// <summary>
    /// Track posisi aman terakhir (posisi di mana player TIDAK overlap dengan tembok)
    /// selama skill aktif.
    /// </summary>
    private IEnumerator TrackSafePosition()
    {
        while (isActive)
        {
            if (!IsOverlappingAnyWall())
            {
                safePosition = playerTransform.position;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Cek apakah player sedang overlap dengan tembok manapun menggunakan bounds check.
    /// </summary>
    private bool IsOverlappingAnyWall()
    {
        if (playerColliders == null || playerColliders.Length == 0) return false;

        foreach (Collider playerCol in playerColliders)
        {
            if (playerCol == null) continue;
            Bounds playerBounds = playerCol.bounds;

            foreach (Collider wallCol in allWallColliders)
            {
                if (wallCol == null) continue;
                if (playerBounds.Intersects(wallCol.bounds))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Pukul mundur player ke posisi aman terakhir.
    /// </summary>
    private void PushPlayerToSafety()
    {
        if (playerTransform == null) return;

        // Teleport ke posisi aman terakhir
        playerTransform.position = safePosition;

        // Reset velocity dan beri dorongan kecil ke atas
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.AddForce(Vector3.up * pushbackForce * 0.3f, ForceMode.Impulse);
        }

        Debug.Log($"[WallPassEffect] Player dipukul mundur ke posisi aman: {safePosition}");
    }

    private void OnDestroy()
    {
        // Safety: jika scene berganti saat efek aktif, kembalikan collision
        if (isActive && playerColliders != null)
        {
            SetCollisionIgnored(false);
            RemoveShimmerEffect();
        }
    }

    // ==================== SHIMMER EFFECT ====================

    /// <summary>
    /// Tambahkan material shimmer holographic ke semua renderer tembok target.
    /// </summary>
    private void ApplyShimmerEffect()
    {
        if (shimmerShader == null) return;

        activeShimmers.Clear();

        foreach (GameObject wall in passableWalls)
        {
            if (wall == null) continue;

            Renderer[] renderers = wall.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                // Buat material shimmer baru
                Material shimmerMat = new Material(shimmerShader);

                // Simpan material asli
                ShimmerData data = new ShimmerData
                {
                    renderer = rend,
                    originalMaterials = rend.materials,
                    shimmerMaterial = shimmerMat
                };
                activeShimmers.Add(data);

                // Tambahkan shimmer sebagai material tambahan (overlay)
                Material[] newMaterials = new Material[rend.materials.Length + 1];
                for (int i = 0; i < rend.materials.Length; i++)
                {
                    newMaterials[i] = rend.materials[i];
                }
                newMaterials[newMaterials.Length - 1] = shimmerMat;
                rend.materials = newMaterials;
            }
        }

        Debug.Log($"[WallPassEffect] Shimmer effect ditambahkan ke {activeShimmers.Count} renderer(s).");
    }

    /// <summary>
    /// Hapus material shimmer dan kembalikan material asli.
    /// </summary>
    private void RemoveShimmerEffect()
    {
        foreach (ShimmerData data in activeShimmers)
        {
            if (data.renderer != null)
            {
                data.renderer.materials = data.originalMaterials;
            }

            if (data.shimmerMaterial != null)
            {
                Destroy(data.shimmerMaterial);
            }
        }
        activeShimmers.Clear();
    }
}
