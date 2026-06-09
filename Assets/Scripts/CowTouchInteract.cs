using System.Collections;
using UnityEngine;

public class CowTouchInteract : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip mooClip;

    [Header("Movement Feel")]
    [SerializeField] private float totalDuration = 6.5f;   
    [SerializeField] private float moveDistance = 3.0f;   
    [SerializeField] private float moveSpeedMultiplier = 1f;

    [Header("Turning")]
    [SerializeField] private float turnInterval = 1f;      
    [SerializeField] private float turnDegrees = 90f;     
    [SerializeField] private int turnSteps = 4;            

    [Header("Trigger")]
    [SerializeField] private string handTag = "PlayerHand";

    private bool isAnimating = false;

    private Vector3 startPos;
    private Quaternion startRot;

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(handTag) && !other.CompareTag(handTag))
            return;

        if (isAnimating) return;

        StartCoroutine(MoveTurnMooRoutine());
    }

    private IEnumerator MoveTurnMooRoutine()
    {
        isAnimating = true;

        
        startPos = transform.position;
        startRot = transform.rotation;

        
        Coroutine turnC = StartCoroutine(TurnRoutine());
        Coroutine mooC  = StartCoroutine(MooRoutine());

        yield return StartCoroutine(MoveRoutine());

        
        if (turnC != null) StopCoroutine(turnC);
        if (mooC != null) StopCoroutine(mooC);

        
        transform.position = startPos;
        transform.rotation = startRot;

        isAnimating = false;
    }

    private IEnumerator MoveRoutine()
    {
        float t = 0f;

        
        float movePerSecond = (moveDistance / totalDuration) * moveSpeedMultiplier;

        while (t < totalDuration)
        {
            transform.position += transform.forward * (movePerSecond * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator TurnRoutine()
{
    
    float turnDuration = 0.3f; 

    for (int i = 0; i < turnSteps; i++)
    {
        
        yield return new WaitForSeconds(turnInterval);

        Quaternion start = transform.rotation;
        Quaternion target = start * Quaternion.Euler(0f, turnDegrees, 0f);

        float t = 0f;
        while (t < turnDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / turnDuration);
            transform.rotation = Quaternion.Slerp(start, target, alpha);
            yield return null;
        }

        transform.rotation = target; 
    }
}


    private IEnumerator MooRoutine()
    {
        
        PlayMoo();

        float elapsed = 0f;
        while (elapsed < totalDuration)
        {
            yield return new WaitForSeconds(3f);
            elapsed += 3f;
            PlayMoo();
        }
    }

    private void PlayMoo()
    {
        if (audioSource != null && mooClip != null)
            audioSource.PlayOneShot(mooClip);
    }
}
