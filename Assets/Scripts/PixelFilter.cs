using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gives the 3D view a chunky pixel look: the camera draws into a small texture,
/// which is then stretched over the whole screen without smoothing. The console
/// UI is drawn on top at full sharpness.
///
/// EDITOR STEPS
/// 1. Add this component to the Main Camera.
/// 2. Create a second Canvas called "Pixel Canvas" (Screen Space - Overlay) and set its
///    Sort Order to -10 so it sits behind the console.
/// 3. Inside it, add a Raw Image stretched to fill the whole screen, with Raycast Target unticked.
/// 4. Output: drag in that Raw Image.
/// 5. Pixel Height: how many pixel rows the 3D view has. Lower is chunkier.
/// Note: in the editor the Game view may say "No cameras rendering". That is expected
/// and can be switched off in the Game view's menu; it does not appear in builds.
/// </summary>
[RequireComponent(typeof(Camera))]
public class PixelFilter : MonoBehaviour
{
    public RawImage output;
    [Range(90, 540)] public int pixelHeight = 270;

    Camera cam;
    RenderTexture texture;
    int builtForWidth, builtForHeight, builtForPixels;

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        Rebuild();
    }

    void Update()
    {
        if (Screen.width != builtForWidth || Screen.height != builtForHeight || pixelHeight != builtForPixels)
            Rebuild();
    }

    void OnDisable()
    {
        if (cam != null) cam.targetTexture = null;
        if (output != null) output.texture = null;
        Release();
    }

    void Rebuild()
    {
        if (Screen.height <= 0) return;
        Release();

        builtForWidth = Screen.width;
        builtForHeight = Screen.height;
        builtForPixels = pixelHeight;

        int height = Mathf.Max(1, pixelHeight);
        int width = Mathf.Max(1, Mathf.RoundToInt(height * (float)Screen.width / Screen.height));
        texture = new RenderTexture(width, height, 24);
        texture.filterMode = FilterMode.Point;
        texture.name = "Pixel Filter " + width + "x" + height;

        cam.targetTexture = texture;
        if (output != null)
        {
            output.texture = texture;
            output.color = Color.white;
        }
    }

    void Release()
    {
        if (texture == null) return;
        texture.Release();
        Destroy(texture);
        texture = null;
    }
}
