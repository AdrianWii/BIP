using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectScaleAndRotate : MonoBehaviour
{
    [SerializeField] private Camera arCamera;

    private Transform selectedObject;
    private Vector3 startScale;
    private Quaternion startRotation;
    private float startDistance;
    private float startAngle;
    private bool isGestureStarted;

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

        var touch0 = Touchscreen.current.touches[0];
        var touch1 = Touchscreen.current.touches[1];

        if (!touch0.press.isPressed || !touch1.press.isPressed)
        {
            isGestureStarted = false;
            selectedObject = null;
            return;
        }

        Vector2 firstFinger = touch0.position.ReadValue();
        Vector2 secondFinger = touch1.position.ReadValue();

        ScaleAndRotate(firstFinger, secondFinger);
    }

    private void ScaleAndRotate(Vector2 firstFinger, Vector2 secondFinger)
    {
        float currentDistance = Vector2.Distance(firstFinger, secondFinger);
        float currentAngle = GetAngle(firstFinger, secondFinger);

        if (!isGestureStarted)
        {
            Vector2 centerPoint = (firstFinger + secondFinger) / 2f;
            selectedObject = GetObjectUnderFinger(centerPoint);

            if (selectedObject == null)
            {
                return;
            }

            startDistance = currentDistance;
            startAngle = currentAngle;
            startScale = selectedObject.localScale;
            startRotation = selectedObject.rotation;
            isGestureStarted = true;

            return;
        }

        float scaleFactor = currentDistance / startDistance;
        float angleDifference = currentAngle - startAngle;

        selectedObject.localScale = startScale * scaleFactor;
        selectedObject.rotation = startRotation * Quaternion.Euler(0f, -angleDifference, 0f);
    }

    private Transform GetObjectUnderFinger(Vector2 screenPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide))
        {
            return hit.collider.transform;
        }

        return null;
    }

    private float GetAngle(Vector2 firstFinger, Vector2 secondFinger)
    {
        Vector2 direction = secondFinger - firstFinger;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}