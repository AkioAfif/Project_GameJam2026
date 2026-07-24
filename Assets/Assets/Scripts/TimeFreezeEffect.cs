using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeFreezeEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    public float transitionSpeed = 2f;

    private Volume volume;
    private ColorAdjustments colorAdjustments;
    private bool isEffectActive = false;
    private float currentBlend = 0f;

    private void Start()
    {
        // Cari kamera utama
        Camera cam = Camera.main;
        if (cam == null)
        {
            cam = FindAnyObjectByType<Camera>();
            Debug.LogWarning("[TimeFreezeEffect] Camera.main null! Menggunakan kamera: " + (cam != null ? cam.name : "TIDAK ADA"));
        }

        if (cam != null)
        {
            var cameraData = cam.GetComponent<UniversalAdditionalCameraData>();
            if (cameraData != null)
            {
                cameraData.renderPostProcessing = true;
                Debug.Log("[TimeFreezeEffect] Post Processing AKTIF pada kamera: " + cam.name);
            }
            else
            {
                Debug.LogError("[TimeFreezeEffect] UniversalAdditionalCameraData TIDAK ditemukan pada kamera " + cam.name);
            }
        }
        else
        {
            Debug.LogError("[TimeFreezeEffect] Tidak ada kamera di scene sama sekali!");
        }

        // Buat Volume component secara otomatis
        volume = gameObject.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 100;
        volume.weight = 0f;

        // Buat profile baru dengan Color Adjustments
        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        colorAdjustments = profile.Add<ColorAdjustments>();
        colorAdjustments.saturation.overrideState = true;
        colorAdjustments.saturation.value = -100f;

        volume.profile = profile;

        Debug.Log("[TimeFreezeEffect] Volume + ColorAdjustments BERHASIL dibuat pada: " + gameObject.name);
    }

    public void SetEffectActive(bool active)
    {
        isEffectActive = active;
        Debug.Log("[TimeFreezeEffect] SetEffectActive(" + active + ") dipanggil!");
    }

    private void Update()
    {
        float targetBlend = isEffectActive ? 1f : 0f;
        currentBlend = Mathf.MoveTowards(currentBlend, targetBlend, transitionSpeed * Time.deltaTime);

        if (volume != null)
        {
            volume.weight = currentBlend;
        }
    }

    private void OnDestroy()
    {
        if (volume != null && volume.profile != null)
        {
            DestroyImmediate(volume.profile);
        }
    }
}