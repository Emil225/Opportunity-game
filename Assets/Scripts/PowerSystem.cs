using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The rover's power meter. Power slowly drops as dust settles on the solar
/// panels. Now and then a gust of wind blows the dust off and power comes back.
/// Other scripts can read Power (0 to 1) to react to low power.
///
/// EDITOR STEPS
/// 1. Add this component to the "Mission Control" object.
/// 2. Bar Fill: a UI Image inside a bar background, with its anchors stretched to
///    fill the background. This script shortens it from the right as power drops.
/// 3. Label (optional): a TextMeshPro text for "POWER 87%".
/// 4. Message Text (optional): a TextMeshPro text that briefly shows the wind gust message.
/// 5. For a dust storm level: raise Dust Per Minute and untick Wind Gusts.
/// </summary>
public class PowerSystem : MonoBehaviour
{
    [Header("Dust")]
    [Range(0f, 1f)] public float startPower = 1f;
    [Tooltip("How much power (0 to 1) is lost each minute as dust builds up.")]
    public float dustPerMinute = 0.08f;
    [Tooltip("Power never drops below this.")]
    [Range(0f, 1f)] public float minimumPower = 0.1f;

    [Header("Wind gusts")]
    public bool windGusts = true;
    [Tooltip("Shortest and longest wait between gusts, in seconds.")]
    public float minGustWait = 50f;
    public float maxGustWait = 100f;
    [Tooltip("How much power a gust gives back (0 to 1).")]
    [Range(0f, 1f)] public float gustRestore = 0.4f;
    public string gustMessage = "A gust of wind cleaned the solar panels.";

    [Header("UI (optional)")]
    public RectTransform barFill;
    public TMP_Text label;
    public TMP_Text messageText;
    public float messageSeconds = 5f;
    public Color normalColor = new Color(0.9f, 0.6f, 0.2f);
    public Color lowColor = new Color(0.8f, 0.25f, 0.2f);
    [Range(0f, 1f)] public float lowThreshold = 0.3f;

    /// <summary>Current power from 0 (empty) to 1 (clean panels).</summary>
    public float Power { get; private set; }

    float nextGustTime;
    float messageHideTime;
    Image barImage;

    void Start()
    {
        Power = startPower;
        ScheduleGust();
        if (barFill != null) barImage = barFill.GetComponent<Image>();
        if (messageText != null) messageText.text = "";
        RefreshUI();
    }

    void Update()
    {
        Power = Mathf.Max(minimumPower, Power - dustPerMinute / 60f * Time.deltaTime);

        if (windGusts && Time.time >= nextGustTime)
        {
            WindGust();
            ScheduleGust();
        }

        if (messageText != null && messageText.text.Length > 0 && Time.time >= messageHideTime)
            messageText.text = "";

        RefreshUI();
    }

    /// <summary>Blows dust off the panels now. Also called automatically when gusts are on.</summary>
    public void WindGust()
    {
        Power = Mathf.Min(1f, Power + gustRestore);
        if (messageText != null)
        {
            messageText.text = gustMessage;
            messageHideTime = Time.time + messageSeconds;
        }
    }

    void ScheduleGust()
    {
        nextGustTime = Time.time + Random.Range(minGustWait, maxGustWait);
    }

    void RefreshUI()
    {
        if (barFill != null)
        {
            barFill.anchorMax = new Vector2(Mathf.Clamp01(Power), barFill.anchorMax.y);
            if (barImage != null) barImage.color = Power <= lowThreshold ? lowColor : normalColor;
        }
        if (label != null)
        {
            string text = "POWER " + Mathf.RoundToInt(Power * 100f) + "%";
            if (label.text != text) label.text = text;
        }
    }
}
