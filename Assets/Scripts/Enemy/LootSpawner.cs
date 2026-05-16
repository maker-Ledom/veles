using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [SerializeField] private GameObject lootPrefab; // Сюда перетяни префаб артефакта

    // Метод, который мы вызовем перед смертью босса
    public void SpawnLoot()
    {
        GameObject droppedItem = Instantiate(lootPrefab, transform.position + Vector3.up, Quaternion.identity);
        
        // Если на предмете есть Rigidbody, подбросим его чуть-чуть
        if (droppedItem.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }
}