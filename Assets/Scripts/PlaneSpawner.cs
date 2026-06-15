using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTrackedImageSpawner : MonoBehaviour
{
    private enum LocalAxis
    {
        PositiveX,
        NegativeX,
        PositiveY,
        NegativeY,
        PositiveZ,
        NegativeZ
    }

    [Header("Prefab")]
    [SerializeField] private GameObject planePrefab;

    [Header("Model Axes")]
    [Tooltip("Ta sama wartoœæ co w PlaneController. Lokalna oœ modelu, która wskazuje NOS samolotu.")]
    [SerializeField] private LocalAxis modelNoseAxis = LocalAxis.PositiveZ;

    [Tooltip("Ta sama wartoœæ co w PlaneController. Lokalna oœ modelu, która wskazuje GÓRÊ samolotu.")]
    [SerializeField] private LocalAxis modelUpAxis = LocalAxis.PositiveY;

    [Header("Spawn")]
    [SerializeField] private float spawnHeightAboveImage = 0.08f;
    [SerializeField] private bool spawnOnlyOnce = true;
    [SerializeField] private bool faceSameDirectionAsCamera = true;

    private GameObject plane;
    private ARTrackedImageManager trackedImageManager;

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
        }
    }

    private void OnDisable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.RemoveListener(OnImageChanged);
        }
    }

    private void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (ARTrackedImage newImage in eventArgs.added)
        {
            TrySpawnPlane(newImage);
        }

        foreach (ARTrackedImage updatedImage in eventArgs.updated)
        {
            if (plane == null)
            {
                TrySpawnPlane(updatedImage);
            }
        }
    }

    private void TrySpawnPlane(ARTrackedImage trackedImage)
    {
        if (planePrefab == null)
        {
            Debug.LogWarning("Plane prefab is not assigned.");
            return;
        }

        if (trackedImage.trackingState != TrackingState.Tracking)
        {
            return;
        }

        if (spawnOnlyOnce && plane != null)
        {
            return;
        }

        Vector3 spawnPosition = trackedImage.transform.position + Vector3.up * spawnHeightAboveImage;
        Quaternion spawnRotation = GetBellyDownSpawnRotation();

        plane = Instantiate(planePrefab, spawnPosition, spawnRotation);
    }

    private Quaternion GetBellyDownSpawnRotation()
    {
        Vector3 desiredNoseDirection = planePrefab.transform.forward;

        if (faceSameDirectionAsCamera && Camera.main != null)
        {
            desiredNoseDirection = Camera.main.transform.forward;
        }

        desiredNoseDirection = Vector3.ProjectOnPlane(desiredNoseDirection, Vector3.up);

        if (desiredNoseDirection.sqrMagnitude < 0.001f)
        {
            desiredNoseDirection = Vector3.forward;
        }

        Vector3 desiredUpDirection = Vector3.up;

        return GetRotationFromModelAxes(
            GetAxisVector(modelNoseAxis),
            GetAxisVector(modelUpAxis),
            desiredNoseDirection,
            desiredUpDirection
        );
    }

    private Quaternion GetRotationFromModelAxes(
        Vector3 localNoseAxis,
        Vector3 localUpAxis,
        Vector3 desiredNoseDirection,
        Vector3 desiredUpDirection)
    {
        Quaternion desiredWorldRotation = Quaternion.LookRotation(
            desiredNoseDirection.normalized,
            desiredUpDirection.normalized
        );

        Quaternion modelAxisCorrection = Quaternion.Inverse(
            Quaternion.LookRotation(localNoseAxis.normalized, localUpAxis.normalized)
        );

        return desiredWorldRotation * modelAxisCorrection;
    }

    private Vector3 GetAxisVector(LocalAxis axis)
    {
        switch (axis)
        {
            case LocalAxis.PositiveX:
                return Vector3.right;

            case LocalAxis.NegativeX:
                return Vector3.left;

            case LocalAxis.PositiveY:
                return Vector3.up;

            case LocalAxis.NegativeY:
                return Vector3.down;

            case LocalAxis.PositiveZ:
                return Vector3.forward;

            case LocalAxis.NegativeZ:
                return Vector3.back;

            default:
                return Vector3.forward;
        }
    }
}