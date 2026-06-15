using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SimpleARCameraSwitcher : MonoBehaviour
{
    [SerializeField] private ARCameraManager arCameraManager;

    private CameraFacingDirection currentCamera = CameraFacingDirection.World;

    private void Awake()
    {
        if (arCameraManager == null)
        {
            arCameraManager = FindAnyObjectByType<ARCameraManager>();
        }
    }

    public void SwitchCamera()
    {
        if (arCameraManager == null)
        {
            Debug.LogWarning("ARCameraManager is not assigned.");
            return;
        }

        if (currentCamera == CameraFacingDirection.World)
        {
            currentCamera = CameraFacingDirection.User;
        }
        else
        {
            currentCamera = CameraFacingDirection.World;
        }

        arCameraManager.requestedFacingDirection = currentCamera;
    }
}