using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlaneController : MonoBehaviour
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

    [Header("References")]
    [SerializeField] private FixedJoystick fixedJoystick;

    [Header("Model Axes")]
    [Tooltip("Wybierz lokaln¹ oœ modelu, która wskazuje NOS samolotu. Jeœli samolot leci lewym skrzyd³em, najczêœciej ustaw PositiveX albo NegativeX.")]
    [SerializeField] private LocalAxis modelNoseAxis = LocalAxis.PositiveZ;

    [Tooltip("Wybierz lokaln¹ oœ modelu, która wskazuje GÓRÊ samolotu. Najczêœciej PositiveY.")]
    [SerializeField] private LocalAxis modelUpAxis = LocalAxis.PositiveY;

    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 0.6f;
    [SerializeField] private float climbSpeed = 0.35f;
    [SerializeField] private float turnSpeed = 55f;

    [Header("Plane Tilt")]
    [SerializeField] private float maxPitchAngle = 25f;
    [SerializeField] private float maxRollAngle = 35f;
    [SerializeField] private float rotationSmoothness = 6f;

    [Header("Input")]
    [SerializeField] private float joystickDeadZone = 0.08f;

    private Rigidbody rigidBody;
    private Animator animationController;
    private float currentYaw;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        animationController = GetComponent<Animator>();

        rigidBody.useGravity = false;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void OnEnable()
    {
        if (fixedJoystick == null)
        {
            fixedJoystick = FindAnyObjectByType<FixedJoystick>();
        }

        currentYaw = GetCurrentNoseYaw();

        if (animationController != null)
        {
            animationController.SetBool("flyParam", true);
        }
    }

    private void FixedUpdate()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (fixedJoystick != null)
        {
            horizontalInput = ApplyDeadZone(fixedJoystick.Horizontal);
            verticalInput = ApplyDeadZone(fixedJoystick.Vertical);
        }

        currentYaw += horizontalInput * turnSpeed * Time.fixedDeltaTime;

        Vector3 flatForward = Quaternion.Euler(0f, currentYaw, 0f) * Vector3.forward;
        Vector3 pitchAxis = Vector3.Cross(Vector3.up, flatForward).normalized;

        float pitchAngle = verticalInput * maxPitchAngle;
        float rollAngle = -horizontalInput * maxRollAngle;

        Vector3 desiredNoseDirection = Quaternion.AngleAxis(-pitchAngle, pitchAxis) * flatForward;
        Vector3 desiredUpDirection = Quaternion.AngleAxis(rollAngle, desiredNoseDirection) * Vector3.up;

        Quaternion targetRotation = GetRotationFromModelAxes(
            GetAxisVector(modelNoseAxis),
            GetAxisVector(modelUpAxis),
            desiredNoseDirection,
            desiredUpDirection
        );

        Quaternion smoothedRotation = Quaternion.Slerp(
            rigidBody.rotation,
            targetRotation,
            rotationSmoothness * Time.fixedDeltaTime
        );

        rigidBody.MoveRotation(smoothedRotation);

        Vector3 forwardMovement = desiredNoseDirection.normalized * forwardSpeed;
        Vector3 verticalMovement = Vector3.up * verticalInput * climbSpeed;

        rigidBody.linearVelocity = forwardMovement + verticalMovement;
    }

    private float ApplyDeadZone(float value)
    {
        if (Mathf.Abs(value) < joystickDeadZone)
        {
            return 0f;
        }

        return value;
    }

    private float GetCurrentNoseYaw()
    {
        Vector3 noseDirection = transform.rotation * GetAxisVector(modelNoseAxis);
        noseDirection = Vector3.ProjectOnPlane(noseDirection, Vector3.up);

        if (noseDirection.sqrMagnitude < 0.001f)
        {
            return transform.eulerAngles.y;
        }

        return Mathf.Atan2(noseDirection.x, noseDirection.z) * Mathf.Rad2Deg;
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