using UnityEngine;
using UnityEngine.InputSystem;

public class HealthPickupCollector : MonoBehaviour
{
    [SerializeField] private Camera arCamera;

    private void Start()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (arCamera == null || Touchscreen.current == null)
        {
            return;
        }

        var touch = Touchscreen.current.primaryTouch;

        if (!touch.press.wasPressedThisFrame)
        {
            return;
        }

        Vector2 touchPosition = touch.position.ReadValue();
        Ray ray = arCamera.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 
            Physics.AllLayers, QueryTriggerInteraction.Collide))
        {
            HealthPickup pickup = hit.collider.GetComponentInParent<HealthPickup>();

            if (pickup != null)
            {
                pickup.Collect();
            }
        }
    }
}