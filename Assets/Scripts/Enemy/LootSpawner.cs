using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [SerializeField] private GameObject lootPrefab;
    public void SpawnLoot()
    {
        GameObject droppedItem = Instantiate(lootPrefab, transform.position + Vector3.up, Quaternion.identity);
        if (droppedItem.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }
}