using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The ending. Plays the closing lines (back to the present day and the silent
/// rover), opens the credits and sources, then offers to play again.
///
/// EDITOR STEPS
/// 1. Add this component to the Canvas in the Ending scene.
/// 2. Dialogue: the object with DialogueBox. Credits: the object with CreditsScreen.
/// 3. Menu: the object holding the "The end" title and the buttons. Hidden until the lines finish.
/// 4. Play Again Button, Credits Button: uGUI buttons. No On Click entries needed.
/// 5. Title Scene: must match a scene name in the Build Profiles scene list.
/// 6. Every fact in the lines needs a source in Assets/Data/Credits.asset.
/// </summary>
public class EndingScreen : MonoBehaviour
{
    public DialogueBox dialogue;
    public CreditsScreen credits;
    public GameObject menu;
    public Button playAgainButton;
    public Button creditsButton;

    [Header("Closing lines")]
    public string speaker = "Mission control";
    [TextArea(2, 4)] public string[] lines;

    public string titleScene = "StartScreen";

    IEnumerator Start()
    {
        if (menu != null) menu.SetActive(false);
        if (playAgainButton != null) playAgainButton.onClick.AddListener(() => SceneManager.LoadScene(titleScene));
        if (creditsButton != null && credits != null) creditsButton.onClick.AddListener(credits.Show);

        // Give the scene a moment to settle before the first line appears.
        yield return new WaitForSeconds(1f);
        if (dialogue != null && lines != null && lines.Length > 0)
            yield return dialogue.Play(speaker, lines);

        if (credits != null)
        {
            credits.Show();
            while (credits.IsOpen) yield return null;
        }

        if (menu != null) menu.SetActive(true);
        if (playAgainButton != null) playAgainButton.Select();
    }
}
