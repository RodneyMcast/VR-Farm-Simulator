using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CrateCollector : MonoBehaviour
{
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;

    private void OnEnable()
    {
        socket.selectEntered.AddListener(OnItemDroppedIn);
    }

    private void OnDisable()
    {
        socket.selectEntered.RemoveListener(OnItemDroppedIn);
    }

    private void OnItemDroppedIn(SelectEnterEventArgs args)
    {
        var itemGO = args.interactableObject.transform.gameObject;
        var item = itemGO.GetComponent<InventoryItem>();
        if (item == null) return;

        // Add to inventory
        InventoryManager.Instance.AddItem(item.type, item.amount);

        
        socket.interactionManager.SelectExit(socket, args.interactableObject);

        
        Destroy(itemGO);
    }
}
