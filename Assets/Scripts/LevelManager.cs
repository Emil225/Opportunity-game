using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Runs one level: plays the opening dialogue, lists the science targets, and
/// when every target has been photographed plays the closing dialogue and loads
/// the next level.
///
/// EDITOR STEPS
/// 1. Add this component to the "Mission Control" object.
/// 2. Queue: drag in the CommandQueue. Dialogue: drag in the object with the DialogueBox.
/// 3. Objective Text: a TextMeshPro text that lists the science targets.
/// 4. Fill in the intro and outro lines. Every fact needs a NASA source on the credits screen.
/// 5. Next Scene Name: the name of the next level's scene, which must be in the
///    Build Profiles scene list. Leave empty on the last level.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public CommandQueue queue;
    public DialogueBox dialogue;
    public TMP_Text objectiveText;

    [Header("Opening")]
    public string introSpeaker = "Mission control";
    [TextArea(2, 4)] public string[] introLines;

    [Header("Closing")]
    public string outroSpeaker = "Mission control";
    [TextArea(2, 4)] public string[] outroLines;
    [Tooltip("Scene to load after the closing dialogue. Leave empty to stay in this level.")]
    public string nextSceneName;

    /// <summary>True once every science target has been photographed and the closing dialogue has finished.</summary>
    public bool IsComplete { get; private set; }

    IEnumerator Start()
    {
        RefreshObjectives();

        if (dialogue != null && introLines != null && introLines.Length > 0)
        {
            if (queue != null) queue.Locked = true;
            yield return dialogue.Play(introSpeaker, introLines);
            if (queue != null) queue.Locked = false;
        }

        // Wait for every target to be photographed.
        while (!AllPhotographed())
        {
            RefreshObjectives();
            yield return null;
        }
        RefreshObjectives();

        // Let the rover finish its commands and the player close the photo first.
        while (queue != null && queue.CurrentState != CommandQueue.State.Editing) yield return null;

        if (queue != null) queue.Locked = true;
        if (dialogue != null && outroLines != null && outroLines.Length > 0)
            yield return dialogue.Play(outroSpeaker, outroLines);

        IsComplete = true;
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            if (objectiveText != null) objectiveText.text += "\nLevel complete.";
        }
    }

    static bool AllPhotographed()
    {
        if (ScienceTarget.All.Count == 0) return false;
        foreach (ScienceTarget target in ScienceTarget.All)
        {
            if (!target.IsPhotographed) return false;
        }
        return true;
    }

    void RefreshObjectives()
    {
        if (objectiveText == null) return;
        var text = new StringBuilder("SCIENCE TARGETS\n");
        foreach (ScienceTarget target in ScienceTarget.All)
        {
            text.Append(target.IsPhotographed ? "[x] " : "[  ] ").AppendLine(target.targetName);
        }
        string result = text.ToString().TrimEnd();
        if (objectiveText.text != result) objectiveText.text = result;
    }
}
