using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text inventoryText;

    private void OnEnable()
    {
        InventoryManager.Instance.OnInventoryChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= Refresh;
    }

    public void Refresh()
    {
        var inv = InventoryManager.Instance;
        inventoryText.text =
            $"Wheat: {inv.wheatCount}\n" +
            $"Seeds: {inv.seedCount}\n" +
            $"Coins: {inv.coins}";
    }

   
    public void SellOneWheat() => InventoryManager.Instance.SellOneWheat();
    public void SellAllWheat() => InventoryManager.Instance.SellAllWheat();
    public void BuyOneSeed() => InventoryManager.Instance.BuyOneSeed();
}
