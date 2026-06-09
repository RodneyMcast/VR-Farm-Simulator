using UnityEngine;


public class FixedGrabWheel : MonoBehaviour
{
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private Transform originalParent;
    private Vector3 originalLocalPos;

    private void Awake()
    {
        if (grab == null) grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        originalParent = transform.parent;
        originalLocalPos = transform.localPosition;
    }

    private void LateUpdate()
    {
        
        if (transform.parent != originalParent)
            transform.SetParent(originalParent);

        transform.localPosition = originalLocalPos;
    }
}
