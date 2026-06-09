using UnityEngine;

using UnityEngine.InputSystem;

public class RotateWhileHeld90_ActionBased : MonoBehaviour
{
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    [Header("Bind these to XR Device Simulator actions")]
    [SerializeField] private InputActionProperty rotateRight;
    [SerializeField] private InputActionProperty rotateLeft;

    [SerializeField] private float stepDegrees = 90f;
    [SerializeField] private float cooldown = 0.25f;

    private float lastRotateTime = -999f;

    private void Reset()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grab == null) grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        rotateRight.action.Enable();
        rotateLeft.action.Enable();

        Debug.Log("Rotate actions enabled: " + rotateRight.action.name + " / " + rotateLeft.action.name);
    }

    private void OnDisable()
    {
        rotateRight.action.Disable();
        rotateLeft.action.Disable();
    }

    private void Update()
    {
        if (grab == null || !grab.isSelected) return;
        if (Time.time - lastRotateTime < cooldown) return;

        if (rotateRight.action.WasPressedThisFrame())
        {
            Debug.Log("Rotate RIGHT pressed");
            transform.Rotate(0f, stepDegrees, 0f, Space.World);
            lastRotateTime = Time.time;
        }
        else if (rotateLeft.action.WasPressedThisFrame())
        {
            Debug.Log("Rotate LEFT pressed");
            transform.Rotate(0f, -stepDegrees, 0f, Space.World);
            lastRotateTime = Time.time;
        }
    }
}
