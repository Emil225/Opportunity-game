using System.Collections;
using UnityEngine;

/// <summary>
/// The rover's camera mast. Pans left and right and tilts up and down on command.
/// The photo is taken from the camera on this mast.
///
/// EDITOR STEPS
/// 1. Under the rover, create an empty object "Mast" at the top of the mast, facing
///    the same way as the rover (blue Z arrow forward). Add this component to it.
/// 2. Create a child of "Mast" called "Mast Camera" with a Camera component.
/// 3. Pan Pivot: drag in "Mast". Tilt Pivot: drag in "Mast Camera".
/// 4. On the rover's RoverController, drag "Mast" into the Mast field.
/// </summary>
public class CameraMast : MonoBehaviour
{
    [Tooltip("Rotates around its local Y axis to pan. Usually this object.")]
    public Transform panPivot;
    [Tooltip("Rotates around its local X axis to tilt. Usually the camera, a child of the pan pivot.")]
    public Transform tiltPivot;

    [Tooltip("Degrees per second.")]
    public float speed = 25f;
    [Tooltip("How far the mast can pan to either side, in degrees.")]
    public float maxPan = 150f;
    [Tooltip("Furthest the camera can look up, in degrees above level.")]
    public float maxTiltUp = 20f;
    [Tooltip("Furthest the camera can look down, in degrees below level.")]
    public float maxTiltDown = 50f;
    [Tooltip("Tilt at the start of the level, in degrees below level.")]
    public float startTiltDown = 15f;

    /// <summary>Current pan in degrees; positive is to the right.</summary>
    public float PanAngle { get; private set; }
    /// <summary>Current tilt in degrees; positive is downward.</summary>
    public float TiltAngle { get; private set; }

    void Awake()
    {
        if (panPivot == null) panPivot = transform;
        TiltAngle = startTiltDown;
        Apply();
    }

    /// <summary>Pans by the given number of degrees. Positive is right. Stops at the pan limit.</summary>
    public IEnumerator Pan(float degrees)
    {
        float target = Mathf.Clamp(PanAngle + degrees, -maxPan, maxPan);
        while (!Mathf.Approximately(PanAngle, target))
        {
            PanAngle = Mathf.MoveTowards(PanAngle, target, speed * Time.deltaTime);
            Apply();
            yield return null;
        }
    }

    /// <summary>Tilts by the given number of degrees. Positive is down. Stops at the tilt limits.</summary>
    public IEnumerator Tilt(float degrees)
    {
        float target = Mathf.Clamp(TiltAngle + degrees, -maxTiltUp, maxTiltDown);
        while (!Mathf.Approximately(TiltAngle, target))
        {
            TiltAngle = Mathf.MoveTowards(TiltAngle, target, speed * Time.deltaTime);
            Apply();
            yield return null;
        }
    }

    void Apply()
    {
        if (panPivot != null) panPivot.localRotation = Quaternion.Euler(0f, PanAngle, 0f);
        if (tiltPivot != null) tiltPivot.localRotation = Quaternion.Euler(TiltAngle, 0f, 0f);
    }
}
