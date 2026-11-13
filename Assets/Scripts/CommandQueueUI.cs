using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Console controls for the command queue: buttons, the queue list and a status line.
/// Keyboard shortcuts also work, so the rover can be tested before any UI exists:
/// A / D = turn, W = drive forward, arrow keys = move the camera, P = take photo,
/// Enter = send, Backspace = clear.
///
/// EDITOR STEPS
/// 1. Add this component to the "Mission Control" object (the one with CommandQueue).
/// 2. Queue: drag in the CommandQueue (same object).
/// 3. Create a Canvas with uGUI buttons and two TextMeshPro texts, then drag them
///    into the matching fields. Every field is optional; empty ones are skipped.
///    The buttons need no On Click entries: this script hooks them up itself.
/// 4. The first time a TextMeshPro text is created, Unity asks to import
///    "TMP Essentials". Accept it.
/// </summary>
public class CommandQueueUI : MonoBehaviour
{
    public CommandQueue queue;

    [Header("Drive buttons (optional)")]
    public Button turnLeftButton;
    public Button turnRightButton;
    public Button driveForwardButton;

    [Header("Camera buttons (optional)")]
    public Button cameraLeftButton;
    public Button cameraRightButton;
    public Button cameraUpButton;
    public Button cameraDownButton;
    public Button takePhotoButton;

    [Header("Queue buttons (optional)")]
    public Button sendButton;
    public Button clearButton;

    [Header("Texts (optional)")]
    [Tooltip("Shows the queued commands, one per line.")]
    public TMP_Text queueText;
    [Tooltip("Shows 'Transmitting…', 'Rover executing…' and so on.")]
    public TMP_Text statusText;

    [Header("Testing")]
    public bool keyboardShortcuts = true;

    void OnEnable()
    {
        if (queue == null) queue = GetComponent<CommandQueue>();
        if (queue == null)
        {
            Debug.LogError("CommandQueueUI has no CommandQueue assigned.", this);
            enabled = false;
            return;
        }

        queue.Changed += Refresh;
        HookButtons(true);
        Refresh();
    }

    void OnDisable()
    {
        if (queue == null) return;
        queue.Changed -= Refresh;
        HookButtons(false);
    }

    void HookButtons(bool add)
    {
        Hook(turnLeftButton, queue.AddTurnLeft, add);
        Hook(turnRightButton, queue.AddTurnRight, add);
        Hook(driveForwardButton, queue.AddDriveForward, add);
        Hook(cameraLeftButton, queue.AddCameraLeft, add);
        Hook(cameraRightButton, queue.AddCameraRight, add);
        Hook(cameraUpButton, queue.AddCameraUp, add);
        Hook(cameraDownButton, queue.AddCameraDown, add);
        Hook(takePhotoButton, queue.AddTakePhoto, add);
        Hook(sendButton, queue.Send, add);
        Hook(clearButton, queue.Clear, add);
    }

    static void Hook(Button button, UnityAction action, bool add)
    {
        if (button == null) return;
        if (add) button.onClick.AddListener(action);
        else button.onClick.RemoveListener(action);
    }

    void Update()
    {
        if (keyboardShortcuts && Keyboard.current != null)
        {
            Keyboard keys = Keyboard.current;
            if (keys.aKey.wasPressedThisFrame) queue.AddTurnLeft();
            if (keys.dKey.wasPressedThisFrame) queue.AddTurnRight();
            if (keys.wKey.wasPressedThisFrame) queue.AddDriveForward();
            if (keys.leftArrowKey.wasPressedThisFrame) queue.AddCameraLeft();
            if (keys.rightArrowKey.wasPressedThisFrame) queue.AddCameraRight();
            if (keys.upArrowKey.wasPressedThisFrame) queue.AddCameraUp();
            if (keys.downArrowKey.wasPressedThisFrame) queue.AddCameraDown();
            if (keys.pKey.wasPressedThisFrame) queue.AddTakePhoto();
            if (keys.enterKey.wasPressedThisFrame) queue.Send();
            if (keys.backspaceKey.wasPressedThisFrame) queue.Clear();
        }

        // The transmit dots animate every frame, so the status line is redrawn while transmitting.
        if (queue.CurrentState == CommandQueue.State.Transmitting) RefreshStatus();
    }

    void Refresh()
    {
        bool editing = queue.CurrentState == CommandQueue.State.Editing && !queue.Locked;
        bool hasCommands = queue.Commands.Count > 0;
        bool canAdd = editing && queue.Commands.Count < queue.maxCommands;

        SetInteractable(turnLeftButton, canAdd);
        SetInteractable(turnRightButton, canAdd);
        SetInteractable(driveForwardButton, canAdd);
        SetInteractable(cameraLeftButton, canAdd);
        SetInteractable(cameraRightButton, canAdd);
        SetInteractable(cameraUpButton, canAdd);
        SetInteractable(cameraDownButton, canAdd);
        SetInteractable(takePhotoButton, canAdd);
        SetInteractable(sendButton, editing && hasCommands);
        SetInteractable(clearButton, editing && hasCommands);

        if (queueText != null)
        {
            var text = new StringBuilder();
            for (int i = 0; i < queue.Commands.Count; i++)
            {
                text.Append(i == queue.ExecutingIndex ? "> " : "  ");
                text.Append(i + 1).Append(". ").AppendLine(queue.Commands[i].Label);
            }
            queueText.text = text.ToString();
        }

        RefreshStatus();
    }

    static void SetInteractable(Button button, bool value)
    {
        if (button != null) button.interactable = value;
    }

    void RefreshStatus()
    {
        if (statusText == null) return;
        switch (queue.CurrentState)
        {
            case CommandQueue.State.Editing:
                statusText.text = queue.Commands.Count == 0
                    ? "Queue commands, then send."
                    : queue.Commands.Count + " of " + queue.maxCommands + " commands queued.";
                break;
            case CommandQueue.State.Transmitting:
                int dots = 1 + (int)(Time.time * 2f) % 3;
                statusText.text = "Transmitting" + new string('.', dots);
                break;
            case CommandQueue.State.Executing:
                statusText.text = "Rover executing command " + (queue.ExecutingIndex + 1) + " of " + queue.Commands.Count + ".";
                break;
        }
    }
}
