using TMPro;
using UnityEngine;

public class SeedCountUI : MonoBehaviour
{
    [SerializeField] private TMP_Text seedText;

    private void OnEnable()
    {
        Refresh();

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += Refresh;
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= Refresh;
    }

    private void Refresh()
    {
        if (seedText == null) return;

        if (InventoryManager.Instance == null)
        {
            seedText.text = "Seeds: ?";
            return;
        }

        seedText.text = $"Seeds: {InventoryManager.Instance.seedCount}";
    }
}
