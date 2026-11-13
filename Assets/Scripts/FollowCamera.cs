using UnityEngine;

/// <summary>
/// Keeps the main camera behind and above the rover without turning with it,
/// so the player watches the rover from a steady viewpoint.
///
/// EDITOR STEPS
/// 1. Add this component to the Main Camera.
/// 2. Target: drag in the rover.
/// 3. Offset: where the camera sits relative to the rover, in world directions.
/// </summary>
public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 5f, -8f);
    [Tooltip("Height above the rover's base that the camera looks at.")]
    public float lookHeight = 0.8f;
    [Tooltip("Higher is snappier, lower is smoother.")]
    public float smoothing = 3f;

    void Start()
    {
        if (target == null) return;
        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * lookHeight);
    }

    void LateUpdate()
    {
        if (target == null) return;
        float t = 1f - Mathf.Exp(-smoothing * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, target.position + offset, t);
        transform.LookAt(target.position + Vector3.up * lookHeight);
    }
}
