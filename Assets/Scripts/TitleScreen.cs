using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The start screen. Start plays a short present-day prologue (the silent rover
/// in Perseverance Valley), then loads the first level as a flashback to 2004.
/// Credits opens the credits and sources panel.
///
/// EDITOR STEPS
/// 1. Add this component to the Canvas in the StartScreen scene.
/// 2. Menu: the object holding the title and buttons. It is hidden while the prologue plays.
/// 3. Start Button, Credits Button: uGUI buttons. No On Click entries needed.
/// 4. Credits: the object with the CreditsScreen component. Dialogue: the object with DialogueBox.
/// 5. First Level Scene: must match a scene name in the Build Profiles scene list.
/// 6. Every fact in the prologue needs a source in Assets/Data/Credits.asset.
/// </summary>
public class TitleScreen : MonoBehaviour
{
    public GameObject menu;
    public Button startButton;
    public Button creditsButton;
    public CreditsScreen credits;
    public DialogueBox dialogue;

    [Header("Prologue")]
    public string prologueSpeaker = "Mars, today";
    [TextArea(2, 4)] public string[] prologueLines;

    public string firstLevelScene = "Level1_EagleCrater";

    bool starting;

    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(BeginGame);
            startButton.Select();
        }
        if (creditsButton != null && credits != null) creditsButton.onClick.AddListener(credits.Show);
        if (credits != null) credits.Closed += () => { if (startButton != null) startButton.Select(); };
    }

    public void BeginGame()
    {
        if (starting) return;
        starting = true;
        StartCoroutine(Begin());
    }

    IEnumerator Begin()
    {
        if (menu != null) menu.SetActive(false);
        if (dialogue != null && prologueLines != null && prologueLines.Length > 0)
            yield return dialogue.Play(prologueSpeaker, prologueLines);
        SceneManager.LoadScene(firstLevelScene);
    }
}
