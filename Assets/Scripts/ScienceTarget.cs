using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Something worth photographing. Holds the real NASA image and the text shown
/// when the player photographs it.
///
/// EDITOR STEPS
/// 1. Add this component to the object the player should photograph (a rock, the outcrop).
/// 2. Image: drag in the NASA photo. The photo's Texture Type must be "Sprite (2D and UI)".
/// 3. Fill in Target Name, Image Id (the PIA number), Credit, Caption and Dialogue Lines.
///    Every fact here needs a NASA source on the credits screen.
/// 4. Aim Point (optional): an empty child marking the spot the camera must see.
///    Leave empty to use this object's own position.
/// </summary>
public class ScienceTarget : MonoBehaviour
{
    public string targetName = "Unnamed target";

    [Header("NASA image")]
    public Sprite image;
    [Tooltip("JPL Photojournal ID, for example PIA05273.")]
    public string imageId;
    [Tooltip("Credit line exactly as NASA gives it.")]
    public string credit = "NASA/JPL-Caltech";
    [TextArea(2, 4)] public string caption;

    [Header("Scientist dialogue")]
    public string speakerName = "Mission scientist";
    [TextArea(2, 4)] public string[] dialogueLines;

    [Header("Detection")]
    public Transform aimPoint;

    /// <summary>All active targets in the scene.</summary>
    public static IReadOnlyList<ScienceTarget> All => all;
    /// <summary>Raised when any target is photographed for the first time. Level flow listens to this.</summary>
    public static event Action<ScienceTarget> Photographed;

    public bool IsPhotographed { get; private set; }
    public Vector3 AimPosition => aimPoint != null ? aimPoint.position : transform.position;

    static readonly List<ScienceTarget> all = new List<ScienceTarget>();

    void OnEnable() { all.Add(this); }
    void OnDisable() { all.Remove(this); }

    public void MarkPhotographed()
    {
        if (IsPhotographed) return;
        IsPhotographed = true;
        Photographed?.Invoke(this);
    }
}
