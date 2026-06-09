using UnityEngine;

public enum ItemType
{
    Wheat,
    Seed
}

public class InventoryItem : MonoBehaviour
{
    public ItemType type;
    public int amount = 1; // keep 1 for now
}
