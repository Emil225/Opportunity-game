using System.Collections;
using UnityEngine;

/// <summary>
/// Takes a photo from the mast camera. The photo succeeds if a ScienceTarget is
/// inside the camera view, within range, and not hidden behind something.
///
/// EDITOR STEPS
/// 1. Add this component to the "Mast Camera" object.
/// 2. Photo Camera: drag in the Mast Camera's Camera (left empty, it uses the one on this object).
/// 3. Result Screen: drag in the object that has the PhotoResultScreen component.
/// 4. On the rover's RoverController, drag "Mast Camera" into the Photo Capture field.
/// 5. Things that should block the view (rocks, ground) need a Collider.
/// </summary>
public class PhotoCapture : MonoBehaviour
{
    public Camera photoCamera;
    public PhotoResultScreen resultScreen;

    [Tooltip("Metres. Targets further away than this do not count.")]
    public float maxRange = 8f;
    [Tooltip("How close to the edge of the picture a target may be. 0 = anywhere in view, 0.2 = must be near the middle.")]
    [Range(0f, 0.4f)] public float edgeMargin = 0.1f;
    [Tooltip("Layers that can block the view of a target.")]
    public LayerMask blockingLayers = ~0;

    void Awake()
    {
        if (photoCamera == null) photoCamera = GetComponent<Camera>();
    }

    /// <summary>Takes the photo and shows the result. Finishes when the player closes the result.</summary>
    public IEnumerator TakePhoto()
    {
        ScienceTarget target = FindTargetInView();
        if (target != null) target.MarkPhotographed();

        if (resultScreen == null)
        {
            Debug.LogWarning("PhotoCapture has no result screen. Photo result: " + (target != null ? target.targetName : "nothing of interest"), this);
            yield break;
        }

        if (target != null) yield return resultScreen.ShowPhoto(target);
        else yield return resultScreen.ShowNothing();
    }

    /// <summary>The nearest science target the camera can see right now, or null.</summary>
    public ScienceTarget FindTargetInView()
    {
        if (photoCamera == null) return null;

        ScienceTarget best = null;
        float bestDistance = float.MaxValue;
        Vector3 eye = photoCamera.transform.position;

        foreach (ScienceTarget target in ScienceTarget.All)
        {
            Vector3 point = target.AimPosition;
            float distance = Vector3.Distance(eye, point);
            if (distance > maxRange || distance >= bestDistance) continue;

            Vector3 view = photoCamera.WorldToViewportPoint(point);
            bool inFrame = view.z > 0f
                && view.x >= edgeMargin && view.x <= 1f - edgeMargin
                && view.y >= edgeMargin && view.y <= 1f - edgeMargin;
            if (!inFrame) continue;

            if (IsBlocked(eye, point, target)) continue;

            best = target;
            bestDistance = distance;
        }
        return best;
    }

    bool IsBlocked(Vector3 eye, Vector3 point, ScienceTarget target)
    {
        RaycastHit hit;
        if (!Physics.Linecast(eye, point, out hit, blockingLayers, QueryTriggerInteraction.Ignore)) return false;
        // The target's own collider and the rover itself do not count as blocking.
        if (hit.transform.IsChildOf(target.transform)) return false;
        if (hit.transform.IsChildOf(transform.root)) return false;
        // Allow a hit right at the aim point, such as the ground the target is lying on.
        return Vector3.Distance(hit.point, point) > 0.15f;
    }
}
