using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] private ItemData collectible;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                bool pickedUp = inventory.PickUp(collectible);
                if (pickedUp)
                {
                    Destroy(gameObject);
                    Debug.Log($"Picked up {gameObject.name}");
                }

            }
        }
    }
}
