using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Shows scientist lines a few at a time. The player clicks (or presses Space or
/// Enter) to move on. All dialogue is on-screen text.
///
/// EDITOR STEPS
/// 1. Add this component to an object that is always active (the Canvas is a good place).
/// 2. Panel: the dialogue panel object. This script hides it at the start and shows it when needed.
/// 3. Speaker Text, Body Text: TextMeshPro texts inside the panel.
/// 4. Hint Text (optional): a small "Click to continue" text inside the panel.
/// </summary>
public class DialogueBox : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text speakerText;
    public TMP_Text bodyText;
    public TMP_Text hintText;
    [Tooltip("How many lines are shown before the player has to click.")]
    [Range(1, 3)] public int linesPerPage = 2;

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    /// <summary>Shows the lines page by page. Finishes when the player has clicked past the last page.</summary>
    public IEnumerator Play(string speaker, string[] lines)
    {
        if (lines == null || lines.Length == 0) yield break;

        if (panel != null) panel.SetActive(true);
        if (speakerText != null) speakerText.text = speaker;

        for (int start = 0; start < lines.Length; start += linesPerPage)
        {
            var page = new StringBuilder();
            for (int i = start; i < start + linesPerPage && i < lines.Length; i++)
            {
                if (page.Length > 0) page.Append("\n\n");
                page.Append(lines[i]);
            }
            if (bodyText != null) bodyText.text = page.ToString();
            if (hintText != null)
                hintText.text = start + linesPerPage < lines.Length ? "Click to continue" : "Click to close";

            yield return WaitForAdvance();
        }

        if (panel != null) panel.SetActive(false);
    }

    /// <summary>Waits until the player clicks, taps, or presses Space or Enter.</summary>
    public static IEnumerator WaitForAdvance()
    {
        // Skip one frame so the click that opened this page does not also close it.
        yield return null;
        advanceRequested = false;
        while (!AdvancePressed()) yield return null;
    }

    /// <summary>
    /// Moves the dialogue on, exactly as a click would. Hook a "Continue" button's
    /// On Click to this if a visible button is wanted.
    /// </summary>
    public void Advance()
    {
        advanceRequested = true;
    }

    static bool advanceRequested;

    static bool AdvancePressed()
    {
        if (advanceRequested)
        {
            advanceRequested = false;
            return true;
        }
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame) return true;
        Keyboard keys = Keyboard.current;
        return keys != null && (keys.spaceKey.wasPressedThisFrame || keys.enterKey.wasPressedThisFrame);
    }
}
