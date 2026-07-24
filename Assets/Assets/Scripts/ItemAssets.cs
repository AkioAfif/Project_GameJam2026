using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Transform pfItemWorld;

    public Sprite skillA;
    public Sprite skillB;
    public Sprite skillC;
    public Sprite skillD;
}