using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public int wheatCount { get; private set; }
    public int seedCount { get; private set; }
    public int coins { get; private set; }

    public event Action OnInventoryChanged;

    [Header("Prices")]
    public int wheatSellPrice = 10;
    public int seedBuyPrice = 10;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
       
    }

    public void AddItem(ItemType type, int amount)
    {
        if (amount <= 0) return;

        switch (type)
        {
            case ItemType.Wheat:
                wheatCount += amount;
                break;
            case ItemType.Seed:
                seedCount += amount;
                break;
        }

        OnInventoryChanged?.Invoke();
    }

    // Sell 1 wheat
    public void SellOneWheat()
    {
        if (wheatCount <= 0) return;

        wheatCount -= 1;
        coins += wheatSellPrice;

        OnInventoryChanged?.Invoke();
    }

    // Sell ALL wheat 
    public void SellAllWheat()
    {
        if (wheatCount <= 0) return;

        coins += wheatCount * wheatSellPrice;
        wheatCount = 0;

        OnInventoryChanged?.Invoke();
    }

    // Buy 1 seed
    public void BuyOneSeed()
    {
        if (coins < seedBuyPrice) return;

        coins -= seedBuyPrice;
        seedCount += 1;

        OnInventoryChanged?.Invoke();
    }

    public bool TryConsumeSeed(int amount = 1)
{
    if (seedCount < amount) return false;
    seedCount -= amount;
    OnInventoryChanged?.Invoke();
    return true;
}

}
