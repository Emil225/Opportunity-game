using System.Text;
using UnityEngine;

/// <summary>
/// The credits and sources list, kept in one asset so the start screen and the
/// ending show exactly the same thing. Every fact and image in the game needs an
/// entry here.
///
/// EDITOR STEPS
/// 1. The asset already exists at Assets/Data/Credits.asset. To make another one:
///    Project window > right-click > Create > Opportunity > Credits.
/// 2. Add one entry per group (facts, images, 3D model, tools). Heading is the
///    group name, Body is the text, Source is a web address (optional).
/// 3. Drag the asset into the Data field of every CreditsScreen.
/// </summary>
[CreateAssetMenu(menuName = "Opportunity/Credits", fileName = "Credits")]
public class CreditsData : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public string heading;
        [TextArea(2, 8)] public string body;
        [Tooltip("Web address of the source. Shown under the body and clickable in the game.")]
        public string source;
    }

    public string title = "Credits and sources";
    public Entry[] entries;

    /// <summary>Builds the rich text shown by CreditsScreen. Sources become clickable links.</summary>
    public string BuildText()
    {
        var text = new StringBuilder();
        text.Append("<size=140%><b>").Append(title).Append("</b></size>\n\n");
        if (entries == null) return text.ToString();

        foreach (Entry entry in entries)
        {
            if (!string.IsNullOrEmpty(entry.heading))
                text.Append("<b><color=#F2C38B>").Append(entry.heading).Append("</color></b>\n");
            if (!string.IsNullOrEmpty(entry.body))
                text.Append(entry.body).Append('\n');
            if (!string.IsNullOrEmpty(entry.source))
                text.Append("<size=80%><link=\"").Append(entry.source).Append("\"><u>")
                    .Append(entry.source).Append("</u></link></size>\n");
            text.Append('\n');
        }
        return text.ToString().TrimEnd();
    }
}
