using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Shows the credits and sources panel. Source links can be clicked and open in
/// a new browser tab. Close with the Close button or Escape.
///
/// EDITOR STEPS
/// 1. Add this component to the Canvas (an object that is always active).
/// 2. Data: drag in Assets/Data/Credits.asset.
/// 3. Panel: the credits panel object. This script hides it at the start.
/// 4. Body Text: a TextMeshPro text inside a Scroll View's Content, with Rich Text on.
/// 5. Scroll (optional): the Scroll View, so it starts at the top every time.
/// 6. Close Button: a button inside the panel. No On Click entry needed.
/// </summary>
public class CreditsScreen : MonoBehaviour
{
    public CreditsData data;
    public GameObject panel;
    public TMP_Text bodyText;
    public ScrollRect scroll;
    public Button closeButton;

    /// <summary>Raised when the player closes the panel.</summary>
    public event Action Closed;

    public bool IsOpen => panel != null && panel.activeSelf;

    int shownFrame = -1;

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
        if (closeButton != null) closeButton.onClick.AddListener(Hide);
    }

    public void Show()
    {
        if (bodyText != null && data != null) bodyText.text = data.BuildText();
        if (panel != null) panel.SetActive(true);
        shownFrame = Time.frameCount;
        if (scroll != null)
        {
            Canvas.ForceUpdateCanvases();
            scroll.verticalNormalizedPosition = 1f;
        }
    }

    public void Hide()
    {
        if (!IsOpen) return;
        panel.SetActive(false);
        Closed?.Invoke();
    }

    void Update()
    {
        // Ignore the click that opened the panel.
        if (!IsOpen || Time.frameCount == shownFrame) return;

        Keyboard keys = Keyboard.current;
        if (keys != null && keys.escapeKey.wasPressedThisFrame)
        {
            Hide();
            return;
        }

        // Open a source link when it is clicked.
        Pointer pointer = Pointer.current;
        if (bodyText == null || pointer == null || !pointer.press.wasPressedThisFrame) return;
        Canvas canvas = bodyText.canvas;
        Camera cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
        int link = TMP_TextUtilities.FindIntersectingLink(bodyText, pointer.position.ReadValue(), cam);
        if (link >= 0) Application.OpenURL(bodyText.textInfo.linkInfo[link].GetLinkID());
    }
}
