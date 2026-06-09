using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlotGrowthSocket : MonoBehaviour
{
    [Header("Socket that receives the seed")]
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor seedSocket;

    [Header("Where the plant visuals appear")]
    [SerializeField] private Transform plantSpawnPoint;

    [Header("Plant stage prefabs (3 stages)")]
    [SerializeField] private GameObject stage1Prefab;
    [SerializeField] private GameObject stage2Prefab;
    [SerializeField] private GameObject stage3Prefab;

    [Header("Fruit/Veg prefab to spawn at the end")]
    [SerializeField] private GameObject fruitPrefab;
    [SerializeField] private Transform fruitSpawnPoint;

    [Header("Timing")]
    [SerializeField] private float timeBetweenStages = 10f;

    private bool isPlanted = false;
    private GameObject currentPlant;
    private GameObject spawnedFruit;

    private void OnEnable()
    {
        seedSocket.selectEntered.AddListener(OnSeedPlaced);
    }

    private void OnDisable()
    {
        seedSocket.selectEntered.RemoveListener(OnSeedPlaced);
    }

    private void OnSeedPlaced(SelectEnterEventArgs args)
    {
        if (isPlanted) return;
        isPlanted = true;

        // Hide the seed after planting
        GameObject seed = args.interactableObject.transform.gameObject;
        seed.SetActive(false);

        StartCoroutine(GrowRoutine());
    }

    private IEnumerator GrowRoutine()
    {
        SpawnStage(stage1Prefab);
        yield return new WaitForSeconds(timeBetweenStages);

        SpawnStage(stage2Prefab);
        yield return new WaitForSeconds(timeBetweenStages);

        // Stage 3 appears
        SpawnStage(stage3Prefab);

        // Veg spawns while stage 3 is present
        SpawnFruitAndHookHarvest();
    }

    private void SpawnStage(GameObject stagePrefab)
    {
        if (currentPlant != null)
            Destroy(currentPlant);

        currentPlant = Instantiate(stagePrefab, plantSpawnPoint.position, plantSpawnPoint.rotation);
    }

    private void SpawnFruitAndHookHarvest()
    {
        spawnedFruit = Instantiate(fruitPrefab, fruitSpawnPoint.position, fruitSpawnPoint.rotation);

        // Ensure it drops
        Rigidbody rb = spawnedFruit.GetComponent<Rigidbody>();
        if (rb == null) rb = spawnedFruit.AddComponent<Rigidbody>();
        rb.useGravity = true;

        // Hook into grab event
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = spawnedFruit.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnFruitGrabbed);
        }
        else
        {
            Debug.LogWarning("Fruit prefab has no XRGrabInteractable. Add it so harvest can trigger.");
        }
    }

    private void OnFruitGrabbed(SelectEnterEventArgs args)
    {
        // When fruit is grabbed, remove stage 3 (harvested)
        if (currentPlant != null)
            Destroy(currentPlant);

        // Optional: stop listening to avoid leaks
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = args.interactableObject.transform.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnFruitGrabbed);
        }

         
         ResetPlot();
    }

   
     private void ResetPlot()
     {
         isPlanted = false;
         spawnedFruit = null;
     }
}
