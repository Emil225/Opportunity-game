using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple top-down map for the console. North (world +Z) is up. Shows the
/// rover, which way it faces, which way the mast camera points, and a marker
/// for every science target.
///
/// EDITOR STEPS
/// 1. Add this component to the map panel on the Canvas.
/// 2. Map Area: the square RectTransform the icons are drawn inside.
/// 3. Rover Icon: a small UI object inside Map Area, anchored to its centre, drawn pointing up.
///    Camera Line (optional): a thin child of Rover Icon, pivot at its bottom, pointing up.
/// 4. Target Icon Template: a small UI Image inside Map Area, anchored to its centre.
///    It is hidden at the start and copied once per science target.
/// 5. Rover: drag in the rover. Mast: drag in the object with the CameraMast.
/// 6. World Centre and World Radius: the point shown at the middle of the map, and
///    how many metres the map shows from the middle to its edge.
/// </summary>
public class MapDisplay : MonoBehaviour
{
    public RectTransform mapArea;
    public RectTransform roverIcon;
    public RectTransform cameraLine;
    public RectTransform targetIconTemplate;

    public Transform rover;
    public CameraMast mast;

    public Vector3 worldCentre = Vector3.zero;
    [Tooltip("Metres from the middle of the map to its edge.")]
    public float worldRadius = 15f;

    public Color targetColor = new Color(1f, 0.8f, 0.2f);
    public Color photographedColor = new Color(0.4f, 0.8f, 0.5f);

    readonly List<ScienceTarget> targets = new List<ScienceTarget>();
    readonly List<Image> targetIcons = new List<Image>();

    void Start()
    {
        if (targetIconTemplate == null || mapArea == null) return;
        targetIconTemplate.gameObject.SetActive(false);
        foreach (ScienceTarget target in ScienceTarget.All)
        {
            RectTransform icon = Instantiate(targetIconTemplate, mapArea);
            icon.gameObject.SetActive(true);
            icon.name = "Target Icon: " + target.targetName;
            icon.anchoredPosition = WorldToMap(target.transform.position);
            targets.Add(target);
            targetIcons.Add(icon.GetComponent<Image>());
        }
        // Keep the rover drawn on top of the target markers.
        if (roverIcon != null) roverIcon.SetAsLastSibling();
    }

    void LateUpdate()
    {
        if (mapArea == null) return;

        if (rover != null && roverIcon != null)
        {
            roverIcon.anchoredPosition = WorldToMap(rover.position);
            // UI rotation is counter-clockwise, compass heading is clockwise.
            roverIcon.localRotation = Quaternion.Euler(0f, 0f, -rover.eulerAngles.y);
            if (cameraLine != null && mast != null)
                cameraLine.localRotation = Quaternion.Euler(0f, 0f, -mast.PanAngle);
        }

        for (int i = 0; i < targets.Count; i++)
        {
            if (targetIcons[i] != null && targets[i] != null)
                targetIcons[i].color = targets[i].IsPhotographed ? photographedColor : targetColor;
        }
    }

    Vector2 WorldToMap(Vector3 world)
    {
        Vector2 half = mapArea.rect.size * 0.5f;
        float scale = worldRadius > 0f ? 1f / worldRadius : 0f;
        float x = Mathf.Clamp((world.x - worldCentre.x) * scale, -1f, 1f);
        float y = Mathf.Clamp((world.z - worldCentre.z) * scale, -1f, 1f);
        return new Vector2(x * half.x, y * half.y);
    }
}
