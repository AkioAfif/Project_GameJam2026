using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private float rotationalSpeed;
    [SerializeField] private GameObject onCollectEffect;
    public AudioClip collectSound;

    public ItemScript.ItemType itemType;

    void Start()
    {

    }

    void Update()
    {
        transform.Rotate(0, rotationalSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Phase 3: The Handoff - check both self and parent for Player component
            Player player = other.GetComponent<Player>() ?? other.GetComponentInParent<Player>();
            
            if (player == null) return;

            // Cek apakah inventory sudah penuh — jika penuh, jangan collect
            if (player.IsInventoryFull())
            {
                Debug.Log("Inventory penuh! Item tidak bisa di-pickup.");
                return;
            }

            if (player.PickUpItem(itemType))
            {
                // Only destroy if item was successfully picked up
                if (collectSound != null)
                {
                    AudioSource.PlayClipAtPoint(collectSound, transform.position);
                }

                if (onCollectEffect != null)
                {
                    Instantiate(onCollectEffect, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }
}