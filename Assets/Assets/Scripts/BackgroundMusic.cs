using UnityEngine;

/// <summary>
/// BackgroundSound: Memainkan musik latar yang tidak terputus saat ganti scene.
/// Gunakan pola Singleton + DontDestroyOnLoad agar hanya ada satu instance.
/// Tempelkan script ini ke sebuah GameObject di scene PERTAMA (misal: Lobby).
/// </summary>
public class BackgroundSound : MonoBehaviour
{
    public static BackgroundSound Instance { get; private set; }

    [Header("Music Settings")]
    [Tooltip("Drag & drop file audio backsound ke sini")]
    [SerializeField] private AudioClip backgroundMusic;

    [Tooltip("Volume musik (0 = mute, 1 = full)")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.5f;

    [Tooltip("Mulai dari awal lagi setelah musik selesai")]
    [SerializeField] private bool loop = true;

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton: jika sudah ada instance lain, hancurkan yang baru
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Agar tidak dihancurkan saat ganti scene
        DontDestroyOnLoad(gameObject);

        // Setup AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;

        // 2D Sound: spatialBlend = 0 agar suara tidak terpengaruh posisi/jarak
        audioSource.spatialBlend = 0f;

        // Mulai putar musik
        if (backgroundMusic != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("[BackgroundSound] Background Music belum di-assign di Inspector!");
        }
    }

    /// <summary>
    /// Ubah volume saat runtime (bisa dipanggil dari script lain).
    /// </summary>
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
            audioSource.volume = volume;
    }

    /// <summary>
    /// Hentikan musik (misalnya saat scene ending).
    /// </summary>
    public void StopMusic()
    {
        if (audioSource != null)
            audioSource.Stop();
    }

    /// <summary>
    /// Mulai putar ulang musik dari awal.
    /// </summary>
    public void PlayMusic()
    {
        if (audioSource != null && backgroundMusic != null)
            audioSource.Play();
    }

    /// <summary>
    /// Ganti musik dengan clip baru.
    /// </summary>
    public void ChangeMusic(AudioClip newClip)
    {
        if (audioSource != null && newClip != null)
        {
            audioSource.Stop();
            audioSource.clip = newClip;
            audioSource.Play();
        }
    }
}
