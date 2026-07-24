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
        // Aktifkan Post Processing di kamera utama secara otomatis
        Camera cam = Camera.main;
        if (cam != null)
        {
            var cameraData = cam.GetComponent<UniversalAdditionalCameraData>();
            if (cameraData != null)
            {
                cameraData.renderPostProcessing = true;
            }
        }

        // Buat Volume component secara otomatis di GameObject ini
        volume = gameObject.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 100; // Prioritas tinggi agar override volume lain
        volume.weight = 0f;

        // Buat profile baru dengan Color Adjustments
        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        colorAdjustments = profile.Add<ColorAdjustments>();
        colorAdjustments.saturation.overrideState = true;
        colorAdjustments.saturation.value = -100f; // Full grayscale (putih abu-abu)

        volume.profile = profile;
    }

    public void SetEffectActive(bool active)
    {
        isEffectActive = active;
    }

    private void Update()
    {
        // Transisi smooth antara normal dan grayscale
        float targetBlend = isEffectActive ? 1f : 0f;
        currentBlend = Mathf.MoveTowards(currentBlend, targetBlend, transitionSpeed * Time.deltaTime);

        if (volume != null)
        {
            volume.weight = currentBlend;
        }
    }

    private void OnDestroy()
    {
        // Bersihkan profile yang dibuat secara runtime
        if (volume != null && volume.profile != null)
        {
            DestroyImmediate(volume.profile);
        }
    }
}