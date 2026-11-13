using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows the result of a photo. On success the real NASA image arrives line by
/// line, then the caption and credit appear, then the scientist dialogue plays.
/// On failure it shows "nothing of interest" for a moment.
///
/// EDITOR STEPS
/// 1. Add this component to an object that is always active (the Canvas is a good place).
/// 2. Panel: the photo panel object. This script hides it at the start.
/// 3. Photo Image: a uGUI Image inside the panel. Title, Caption and Credit Text: TextMeshPro texts inside the panel.
/// 4. Nothing Panel: a small panel with a text such as "Nothing of interest in view."
/// 5. Dialogue: drag in the object that has the DialogueBox component.
/// </summary>
public class PhotoResultScreen : MonoBehaviour
{
    [Header("Photo panel")]
    public GameObject panel;
    public Image photoImage;
    public TMP_Text titleText;
    public TMP_Text captionText;
    public TMP_Text creditText;

    [Header("Nothing found")]
    public GameObject nothingPanel;
    public float nothingSeconds = 2.5f;

    [Header("Reveal")]
    [Tooltip("The image arrives in this many horizontal strips.")]
    public int revealLines = 40;
    [Tooltip("Seconds for the whole image to arrive.")]
    public float revealSeconds = 4f;

    public DialogueBox dialogue;

    /// <summary>True while a photo result is on screen.</summary>
    public bool IsShowing { get; private set; }

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
        if (nothingPanel != null) nothingPanel.SetActive(false);
    }

    /// <summary>Reveals the target's image, then caption, credit and dialogue. Finishes when the player closes it.</summary>
    public IEnumerator ShowPhoto(ScienceTarget target)
    {
        IsShowing = true;
        if (panel != null) panel.SetActive(true);
        if (titleText != null) titleText.text = "Receiving image from Mars...";
        if (captionText != null) captionText.text = "";
        if (creditText != null) creditText.text = "";

        if (photoImage != null)
        {
            photoImage.sprite = target.image;
            photoImage.preserveAspect = true;
            photoImage.type = Image.Type.Filled;
            photoImage.fillMethod = Image.FillMethod.Vertical;
            photoImage.fillOrigin = (int)Image.OriginVertical.Top;
            photoImage.fillAmount = 0f;

            int lines = Mathf.Max(1, revealLines);
            var wait = new WaitForSeconds(revealSeconds / lines);
            for (int line = 1; line <= lines; line++)
            {
                photoImage.fillAmount = (float)line / lines;
                yield return wait;
            }
        }

        if (titleText != null) titleText.text = target.targetName;
        if (captionText != null) captionText.text = target.caption;
        if (creditText != null)
        {
            creditText.text = string.IsNullOrEmpty(target.imageId)
                ? "Image: " + target.credit
                : "Image " + target.imageId + ": " + target.credit;
        }

        if (dialogue != null && target.dialogueLines != null && target.dialogueLines.Length > 0)
            yield return dialogue.Play(target.speakerName, target.dialogueLines);
        else
            yield return DialogueBox.WaitForAdvance();

        if (panel != null) panel.SetActive(false);
        IsShowing = false;
    }

    /// <summary>Tells the player the photo showed nothing useful.</summary>
    public IEnumerator ShowNothing()
    {
        IsShowing = true;
        if (nothingPanel != null) nothingPanel.SetActive(true);
        yield return new WaitForSeconds(nothingSeconds);
        if (nothingPanel != null) nothingPanel.SetActive(false);
        IsShowing = false;
    }
}
