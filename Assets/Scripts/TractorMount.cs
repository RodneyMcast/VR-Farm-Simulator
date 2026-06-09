using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;


public class TractorMount : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform xrCamera; 
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable steeringWheelGrab;
    [SerializeField] private Transform seatPoint;
    [SerializeField] private XROrigin xrOrigin; 

    [Header("Optional: disable locomotion while driving")]
    [SerializeField] private Behaviour[] disableWhileDriving; 

    private Transform originalParent;
    private bool mounted;

    private void OnEnable()
    {
        steeringWheelGrab.selectEntered.AddListener(OnDriveStart);
        steeringWheelGrab.selectExited.AddListener(OnDriveStop);
    }

    private void OnDisable()
    {
        steeringWheelGrab.selectEntered.RemoveListener(OnDriveStart);
        steeringWheelGrab.selectExited.RemoveListener(OnDriveStop);
    }

    private void OnDriveStart(SelectEnterEventArgs args)
{
    if (mounted) return;
    mounted = true;

    if (xrOrigin == null || xrCamera == null || seatPoint == null)
    {
        Debug.LogError("Assign xrOrigin, xrCamera, seatPoint in inspector.");
        return;
    }

    originalParent = xrOrigin.transform.parent;

    
    xrOrigin.transform.SetParent(seatPoint, worldPositionStays: false);

    
    xrOrigin.transform.localRotation = Quaternion.identity;

    
    Vector3 cameraLocalPos = xrOrigin.transform.InverseTransformPoint(xrCamera.position);

    
    xrOrigin.transform.localPosition = -cameraLocalPos;

    
    foreach (var b in disableWhileDriving)
        if (b != null) b.enabled = false;
}




   private void OnDriveStop(SelectExitEventArgs args)
{
    if (!mounted) return;
    mounted = false;

    if (xrOrigin == null) return;

    xrOrigin.transform.SetParent(originalParent, worldPositionStays: true);

    foreach (var b in disableWhileDriving)
        if (b != null) b.enabled = true;
}


}
