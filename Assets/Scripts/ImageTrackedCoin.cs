using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTrackedCoin : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private Camera arCamera;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private string imageName = "coffee_mug_marker";
    [SerializeField] private bool acceptAnyImage = true;
    [SerializeField] private bool showDebugText = true;
    [SerializeField] private Vector3 coinLocalPosition = new(0f, 0.05f, 0f);
    [SerializeField] private Vector3 coinLocalEulerAngles = new(90f, 0f, 0f);
    [SerializeField] private Vector3 coinWorldScale = new(0.15f, 0.15f, 0.15f);

    private readonly Dictionary<TrackableId, GameObject> coins = new();
    private bool collected;
    private string status = "Waiting for image tracking...";

    private void Awake()
    {
        if (trackedImageManager == null)
        {
            trackedImageManager = GetComponent<ARTrackedImageManager>();
        }

        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        if (trackedImageManager == null)
        {
            status = "Missing ARTrackedImageManager.";
            Debug.LogError($"ImageTrackedCoin: {status}");
            return;
        }

        trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
        status = "Point camera at marker.";
    }

    private void OnDisable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
        }
    }

    private void Update()
    {
        if (collected)
        {
            return;
        }

        if (TryGetPressPosition(out var screenPosition))
        {
            TryCollectCoin(screenPosition);
        }
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var trackedImage in args.added)
        {
            UpdateCoin(trackedImage);
        }

        foreach (var trackedImage in args.updated)
        {
            UpdateCoin(trackedImage);
        }

        foreach (var removed in args.removed)
        {
            if (coins.TryGetValue(removed.Key, out var coin))
            {
                Destroy(coin);
                coins.Remove(removed.Key);
            }
        }
    }

    private void UpdateCoin(ARTrackedImage trackedImage)
    {
        var trackedName = trackedImage.referenceImage.name;

        if (!acceptAnyImage && trackedName != imageName)
        {
            status = $"Saw '{trackedName}', waiting for '{imageName}'.";
            Debug.Log($"ImageTrackedCoin: {status}");
            return;
        }

        if (!coins.TryGetValue(trackedImage.trackableId, out var coin))
        {
            coin = CreateCoin(trackedImage.transform);
            coins.Add(trackedImage.trackableId, coin);
            status = $"Detected '{trackedName}', coin created.";
            Debug.Log($"ImageTrackedCoin: {status}");
        }

        coin.transform.SetParent(trackedImage.transform, false);
        coin.transform.localPosition = coinLocalPosition;
        coin.transform.localRotation = Quaternion.Euler(coinLocalEulerAngles);
        SetWorldScale(coin.transform, coinWorldScale);
        coin.SetActive(!collected && trackedImage.trackingState == TrackingState.Tracking);

        if (!collected)
        {
            status = trackedImage.trackingState == TrackingState.Tracking
                ? $"Tracking '{trackedName}'. Tap coin."
                : $"Saw '{trackedName}', state: {trackedImage.trackingState}.";
        }
    }

    private GameObject CreateCoin(Transform parent)
    {
        var coin = coinPrefab != null
            ? Instantiate(coinPrefab)
            : GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        coin.name = "AR Coin";
        coin.transform.SetParent(parent, false);
        coin.transform.localScale = Vector3.one * 0.01f;

        if (coin.GetComponent<TrackedCoin>() == null)
        {
            coin.AddComponent<TrackedCoin>();
        }

        if (coin.GetComponentInChildren<Collider>() == null)
        {
            var collider = coin.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.5f;
        }

        if (coinPrefab == null)
        {
            var renderer = coin.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = CreateCoinMaterial();
        }

        return coin;
    }

    private static void SetWorldScale(Transform target, Vector3 desiredWorldScale)
    {
        var parentScale = target.parent != null ? target.parent.lossyScale : Vector3.one;
        target.localScale = new Vector3(
            SafeDivide(desiredWorldScale.x, parentScale.x),
            SafeDivide(desiredWorldScale.y, parentScale.y),
            SafeDivide(desiredWorldScale.z, parentScale.z));
    }

    private static float SafeDivide(float value, float divisor)
    {
        return Mathf.Abs(divisor) < 0.0001f ? value : value / divisor;
    }

    private Material CreateCoinMaterial()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        var material = new Material(shader);
        var color = new Color(1f, 0.68f, 0.12f);
        material.color = color;

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }

        if (material.HasProperty("_Metallic"))
        {
            material.SetFloat("_Metallic", 0.6f);
        }

        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", 0.45f);
        }

        return material;
    }

    private void TryCollectCoin(Vector2 screenPosition)
    {
        if (arCamera == null)
        {
            return;
        }

        var ray = arCamera.ScreenPointToRay(screenPosition);
        if (!Physics.Raycast(ray, out var hit, float.PositiveInfinity, ~0, QueryTriggerInteraction.Collide))
        {
            return;
        }

        var coin = hit.collider.GetComponentInParent<TrackedCoin>();
        if (coin == null)
        {
            return;
        }

        collected = true;
        coin.gameObject.SetActive(false);
        status = "Coin collected!";
        Debug.Log($"ImageTrackedCoin: {status}");
    }

    private void OnGUI()
    {
        if (!showDebugText)
        {
            return;
        }

        var rect = new Rect(24f, 24f, Screen.width - 48f, 120f);
        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 28,
            normal = { textColor = Color.white },
            wordWrap = true
        };

        GUI.Label(rect, $"ImageTrackedCoin: {status}", style);
    }

    private static bool TryGetPressPosition(out Vector2 screenPosition)
    {
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                screenPosition = touch.position.ReadValue();
                return true;
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            screenPosition = Mouse.current.position.ReadValue();
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                screenPosition = touch.position;
                return true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            screenPosition = Input.mousePosition;
            return true;
        }
#endif

        screenPosition = default;
        return false;
    }
}

public class TrackedCoin : MonoBehaviour
{
}
