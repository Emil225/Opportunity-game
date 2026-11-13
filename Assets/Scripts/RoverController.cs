using System.Collections;
using UnityEngine;

/// <summary>
/// Carries out one rover command at a time: turns on the spot or drives in a
/// straight line at a slow fixed speed, spinning the wheels while it moves.
/// The rover follows the ground under it, so it works on terrain and ramps.
///
/// EDITOR STEPS
/// 1. Add this component to the root object of the rover (the object that should move).
///    The rover's forward direction is its blue Z arrow.
/// 2. Left Wheels / Right Wheels: drag in the wheel objects from each side of the rover.
///    Leave empty if the model has no separate wheels; the rover still moves.
/// 3. Wheel Spin Axis: the wheel's local axle axis. X (1,0,0) is the usual one;
///    try Y or Z if the wheels wobble instead of rolling.
/// 4. Wheel Radius and Track Width: measure on the model, in metres.
/// 5. Mast: drag in the object with the CameraMast component.
///    Photo Capture: drag in the object with the PhotoCapture component.
/// 6. Ground: anything the rover should drive on (terrain, lander, plane) needs a Collider.
///    The rover itself must not have a collider on the Ground Layers.
/// </summary>
public class RoverController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Metres per second. The real rover managed about 0.05; this is sped up to stay playable.")]
    public float driveSpeed = 0.5f;
    [Tooltip("Degrees per second when turning on the spot.")]
    public float turnSpeed = 20f;

    [Header("Ground")]
    [Tooltip("Keep the rover on the surface below it and tilt it to match the slope.")]
    public bool followGround = true;
    public LayerMask groundLayers = ~0;
    [Tooltip("Degrees per second the body tilts to match the slope.")]
    public float tiltSpeed = 40f;

    [Header("Wheels (optional)")]
    public Transform[] leftWheels;
    public Transform[] rightWheels;
    public Vector3 wheelSpinAxis = Vector3.right;
    [Tooltip("Metres. Opportunity's wheels were about 0.125 m in radius.")]
    public float wheelRadius = 0.125f;
    [Tooltip("Metres between the left and right wheels. Used to spin the wheels during turns.")]
    public float trackWidth = 1.2f;

    [Header("Camera (optional)")]
    [Tooltip("The CameraMast that carries out camera commands.")]
    public CameraMast mast;
    [Tooltip("The PhotoCapture that carries out the take photo command.")]
    public PhotoCapture photoCapture;

    /// <summary>True while a command is being carried out.</summary>
    public bool IsBusy { get; private set; }
    /// <summary>Compass heading in degrees: 0 is world +Z, 90 is world +X.</summary>
    public float Heading { get; private set; }

    Vector3 groundNormal = Vector3.up;

    void Awake()
    {
        Heading = transform.eulerAngles.y;
    }

    void Start()
    {
        // Settle onto the ground straight away so the rover does not start floating or sunk.
        RaycastHit hit;
        if (followGround && TryGetGround(transform.position, out hit))
        {
            transform.position = hit.point;
            groundNormal = hit.normal;
        }
        ApplyRotation(true);
    }

    /// <summary>Runs one command to completion. Start it as a coroutine, or yield on it from another one.</summary>
    public IEnumerator Execute(RoverCommand command)
    {
        IsBusy = true;
        switch (command.type)
        {
            case RoverCommandType.TurnLeft:
                yield return Turn(-Mathf.Abs(command.amount));
                break;
            case RoverCommandType.TurnRight:
                yield return Turn(Mathf.Abs(command.amount));
                break;
            case RoverCommandType.DriveForward:
                yield return Drive(Mathf.Abs(command.amount));
                break;
            case RoverCommandType.CameraLeft:
                if (mast != null) yield return mast.Pan(-Mathf.Abs(command.amount));
                break;
            case RoverCommandType.CameraRight:
                if (mast != null) yield return mast.Pan(Mathf.Abs(command.amount));
                break;
            case RoverCommandType.CameraUp:
                if (mast != null) yield return mast.Tilt(-Mathf.Abs(command.amount));
                break;
            case RoverCommandType.CameraDown:
                if (mast != null) yield return mast.Tilt(Mathf.Abs(command.amount));
                break;
            case RoverCommandType.TakePhoto:
                if (photoCapture != null) yield return photoCapture.TakePhoto();
                else Debug.LogWarning("RoverController has no Photo Capture assigned.", this);
                break;
        }
        IsBusy = false;
    }

    // Positive degrees turn right (clockwise seen from above).
    IEnumerator Turn(float degrees)
    {
        float remaining = Mathf.Abs(degrees);
        float direction = Mathf.Sign(degrees);
        while (remaining > 0f)
        {
            float step = Mathf.Min(turnSpeed * Time.deltaTime, remaining);
            remaining -= step;
            Heading = Mathf.Repeat(Heading + step * direction, 360f);
            ApplyRotation(false);

            // Each wheel travels along an arc; the two sides roll in opposite directions.
            float wheelTravel = step * Mathf.Deg2Rad * trackWidth * 0.5f;
            SpinWheels(leftWheels, wheelTravel * direction);
            SpinWheels(rightWheels, -wheelTravel * direction);
            yield return null;
        }
    }

    IEnumerator Drive(float metres)
    {
        float remaining = metres;
        while (remaining > 0f)
        {
            float step = Mathf.Min(driveSpeed * Time.deltaTime, remaining);
            Vector3 flatForward = Quaternion.Euler(0f, Heading, 0f) * Vector3.forward;
            Vector3 next = transform.position + flatForward * step;

            if (followGround)
            {
                RaycastHit hit;
                // No ground ahead (the edge of the level): stop this drive early.
                if (!TryGetGround(next, out hit)) yield break;
                next.y = hit.point.y;
                groundNormal = hit.normal;
            }

            remaining -= step;
            transform.position = next;
            ApplyRotation(false);
            SpinWheels(leftWheels, step);
            SpinWheels(rightWheels, step);
            yield return null;
        }
    }

    bool TryGetGround(Vector3 position, out RaycastHit hit)
    {
        Vector3 origin = new Vector3(position.x, position.y + 3f, position.z);
        return Physics.Raycast(origin, Vector3.down, out hit, 30f, groundLayers, QueryTriggerInteraction.Ignore);
    }

    void ApplyRotation(bool instant)
    {
        Quaternion target = Quaternion.FromToRotation(Vector3.up, followGround ? groundNormal : Vector3.up)
            * Quaternion.Euler(0f, Heading, 0f);
        transform.rotation = instant
            ? target
            : Quaternion.RotateTowards(transform.rotation, target, Mathf.Max(tiltSpeed, turnSpeed * 2f) * Time.deltaTime);
    }

    void SpinWheels(Transform[] wheels, float metresRolled)
    {
        if (wheels == null || wheelRadius <= 0f) return;
        float degrees = metresRolled / wheelRadius * Mathf.Rad2Deg;
        foreach (Transform wheel in wheels)
        {
            if (wheel != null) wheel.Rotate(wheelSpinAxis, degrees, Space.Self);
        }
    }
}
