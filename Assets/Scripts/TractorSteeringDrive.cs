using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TractorSteeringDrive : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable steeringWheelGrab;
    [SerializeField] private Transform steeringWheelVisual; 
    [SerializeField] private Rigidbody tractorRb;

    [Header("Drive Settings")]
    [SerializeField] private float driveSpeed = 2.5f;      
    [SerializeField] private float turnSpeed = 60f;        

    [Header("Wheel Settings")]
    [SerializeField] private float maxWheelAngle = 90f;    
    [SerializeField] private float wheelReturnSpeed = 120f; 

    private bool isHeld = false;

    private float startWheelYaw;
    private float wheelYawOffset; 
    private Quaternion wheelStartLocalRot;

    private void Reset()
    {
        tractorRb = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        if (tractorRb == null) tractorRb = GetComponent<Rigidbody>();
        wheelStartLocalRot = steeringWheelVisual.localRotation;
    }

    private void OnEnable()
    {
        steeringWheelGrab.selectEntered.AddListener(OnGrab);
        steeringWheelGrab.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        steeringWheelGrab.selectEntered.RemoveListener(OnGrab);
        steeringWheelGrab.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;

        
        startWheelYaw = GetInteractorYaw(args);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
    }

    private void FixedUpdate()
    {
        if (isHeld)
        {
            
            Vector3 forwardMove = transform.forward * driveSpeed;
            tractorRb.linearVelocity = new Vector3(forwardMove.x, tractorRb.linearVelocity.y, forwardMove.z);

            
            float steer01 = wheelYawOffset / maxWheelAngle; 
            float turnThisFrame = steer01 * turnSpeed * Time.fixedDeltaTime;
            tractorRb.MoveRotation(tractorRb.rotation * Quaternion.Euler(0f, turnThisFrame, 0f));
        }
        else
        {
            
            tractorRb.linearVelocity = new Vector3(0f, tractorRb.linearVelocity.y, 0f);

            
            wheelYawOffset = Mathf.MoveTowards(wheelYawOffset, 0f, wheelReturnSpeed * Time.fixedDeltaTime);
            ApplyWheelVisual();
        }
    }

    private void Update()
    {
        if (!isHeld) return;

        
        float currentYaw = GetInteractorYaw(steeringWheelGrab.interactorsSelecting[0]);
        float delta = Mathf.DeltaAngle(startWheelYaw, currentYaw);

        wheelYawOffset = Mathf.Clamp(delta, -maxWheelAngle, maxWheelAngle);
        ApplyWheelVisual();
    }

    private float GetInteractorYaw(SelectEnterEventArgs args)
    {
        return GetInteractorYaw(args.interactorObject);
    }

    private float GetInteractorYaw(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor)
    {
        Transform t = interactor.transform;
        return t.eulerAngles.y;
    }

    private void ApplyWheelVisual()
    {
        
        steeringWheelVisual.localRotation = wheelStartLocalRot * Quaternion.Euler(0f, -wheelYawOffset, 0f);


    }
}
