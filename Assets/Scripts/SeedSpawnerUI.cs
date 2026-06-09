using UnityEngine;

public class SeedSpawnerUI : MonoBehaviour
{
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private Transform spawnPoint;

    public void SpawnSeed()
{
    var inv = InventoryManager.Instance;
    if (inv == null) return;

    if (!inv.TryConsumeSeed(1))
    {
        Debug.Log("No seeds owned!");
        return;
    }

    Vector3 pos = spawnPoint.position + Vector3.up * 0.25f; // lift it up
    var obj = Instantiate(seedPrefab, pos, spawnPoint.rotation);

    Debug.Log("Seed spawned at: " + pos);
}

}
