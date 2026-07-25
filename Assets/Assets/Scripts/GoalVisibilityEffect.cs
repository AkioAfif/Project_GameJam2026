using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GoalVisibilityEffect: Membuat objek-objek target terlihat menembus dinding
/// seperti X-ray Minecraft. Drag & drop objek yang ingin di-xray di Inspector.
/// </summary>
public class GoalVisibilityEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    [Tooltip("Warna glow X-ray (default: hijau neon)")]
    public Color glowColor = new Color(0f, 1f, 0.5f, 1f);

    [Tooltip("Intensitas cahaya glow")]
    public float glowIntensity = 2.5f;

    [Tooltip("Kecepatan kedip-kedip (pulsing)")]
    public float pulseSpeed = 2f;

    [Header("Target Objects")]
    [Tooltip("Drag & drop objek yang ingin diberi efek X-ray di sini")]
    [SerializeField] private GameObject[] xrayTargets;

    // Internal tracking
    private Shader xrayShader;
    private List<RendererData> activeEffects = new List<RendererData>();

    // Struct untuk menyimpan material asli dan material xray
    private struct RendererData
    {
        public Renderer renderer;
        public Material[] originalMaterials;
        public Material xrayMaterial;
    }

    private void Awake()
    {
        xrayShader = Shader.Find("Custom/XRayGlow");
        if (xrayShader == null)
        {
            Debug.LogError("[GoalVisibilityEffect] Shader 'Custom/XRayGlow' tidak ditemukan! Pastikan file XRayGlow.shader ada di project.");
        }
    }

    /// <summary>
    /// Aktifkan efek X-ray pada semua TeleportPlate di scene.
    /// </summary>
    public void ActivateEffect(float duration)
    {
        StartCoroutine(XRayRoutine(duration));
    }

    private IEnumerator XRayRoutine(float duration)
    {
        if (xrayTargets == null || xrayTargets.Length == 0)
        {
            Debug.LogWarning("[GoalVisibilityEffect] Tidak ada target X-ray yang di-assign di Inspector!");
            yield break;
        }

        Debug.Log($"[GoalVisibilityEffect] Skill A Activated! {xrayTargets.Length} target(s) akan di-xray.");

        // Tambahkan efek xray ke setiap target
        foreach (GameObject target in xrayTargets)
        {
            if (target != null)
                AddXRayToPlate(target);
        }

        // Tunggu selama durasi skill
        yield return new WaitForSeconds(duration);

        // Hapus semua efek xray
        RemoveAllXRayEffects();

        Debug.Log("[GoalVisibilityEffect] Skill A Ended. X-Ray dinonaktifkan.");
    }

    private void AddXRayToPlate(GameObject plateObj)
    {
        // Ambil semua renderer di plate (termasuk child objects)
        Renderer[] renderers = plateObj.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            if (xrayShader == null) continue;

            // Buat material xray baru
            Material xrayMat = new Material(xrayShader);
            xrayMat.SetColor("_GlowColor", glowColor);
            xrayMat.SetFloat("_GlowIntensity", glowIntensity);
            xrayMat.SetFloat("_PulseSpeed", pulseSpeed);

            // Simpan material asli
            RendererData data = new RendererData
            {
                renderer = rend,
                originalMaterials = rend.materials,
                xrayMaterial = xrayMat
            };
            activeEffects.Add(data);

            // Tambahkan material xray di samping material asli
            Material[] newMaterials = new Material[rend.materials.Length + 1];
            for (int i = 0; i < rend.materials.Length; i++)
            {
                newMaterials[i] = rend.materials[i];
            }
            newMaterials[newMaterials.Length - 1] = xrayMat;
            rend.materials = newMaterials;
        }
    }

    private void RemoveAllXRayEffects()
    {
        foreach (RendererData data in activeEffects)
        {
            if (data.renderer != null)
            {
                // Kembalikan material asli
                data.renderer.materials = data.originalMaterials;
            }

            // Bersihkan material xray
            if (data.xrayMaterial != null)
            {
                Destroy(data.xrayMaterial);
            }
        }
        activeEffects.Clear();
    }

    private void OnDestroy()
    {
        // Bersihkan jika scene berganti saat efek masih aktif
        RemoveAllXRayEffects();
    }
}
