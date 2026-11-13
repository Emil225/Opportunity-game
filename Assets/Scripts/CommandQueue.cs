using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the commands the player has queued, sends them to the rover after a
/// transmit delay, and lets the player clear the queue before sending.
///
/// EDITOR STEPS
/// 1. Create an empty object called "Mission Control" and add this component.
/// 2. Rover: drag in the rover object that has the RoverController.
/// 3. Adjust Turn Step, Drive Step, Transmit Delay and Max Commands if wanted.
/// </summary>
public class CommandQueue : MonoBehaviour
{
    public enum State { Editing, Transmitting, Executing }

    public RoverController rover;

    [Header("Command sizes")]
    [Tooltip("Degrees per turn command.")]
    public float turnStep = 45f;
    [Tooltip("Metres per drive command.")]
    public float driveStep = 2f;
    [Tooltip("Degrees per camera command.")]
    public float cameraStep = 15f;

    [Header("Queue")]
    public int maxCommands = 6;
    [Tooltip("Seconds of 'transmitting' before the rover starts. The real one-way delay to Mars is about 3 to 22 minutes.")]
    public float transmitDelay = 3f;

    /// <summary>Raised whenever the queue contents or the state change. The UI listens to this.</summary>
    public event Action Changed;

    public State CurrentState { get; private set; } = State.Editing;
    public IReadOnlyList<RoverCommand> Commands => commands;
    /// <summary>Index of the command the rover is carrying out, or -1 when it is not executing.</summary>
    public int ExecutingIndex { get; private set; } = -1;
    /// <summary>0 to 1 while transmitting; used for the transmit indicator.</summary>
    public float TransmitProgress { get; private set; }

    /// <summary>While true the player cannot add, clear or send commands (used during dialogue).</summary>
    public bool Locked
    {
        get { return locked; }
        set
        {
            if (locked == value) return;
            locked = value;
            Changed?.Invoke();
        }
    }

    bool locked;
    readonly List<RoverCommand> commands = new List<RoverCommand>();

    public void AddTurnLeft() => Add(new RoverCommand(RoverCommandType.TurnLeft, turnStep));
    public void AddTurnRight() => Add(new RoverCommand(RoverCommandType.TurnRight, turnStep));
    public void AddDriveForward() => Add(new RoverCommand(RoverCommandType.DriveForward, driveStep));
    public void AddCameraLeft() => Add(new RoverCommand(RoverCommandType.CameraLeft, cameraStep));
    public void AddCameraRight() => Add(new RoverCommand(RoverCommandType.CameraRight, cameraStep));
    public void AddCameraUp() => Add(new RoverCommand(RoverCommandType.CameraUp, cameraStep));
    public void AddCameraDown() => Add(new RoverCommand(RoverCommandType.CameraDown, cameraStep));
    public void AddTakePhoto() => Add(new RoverCommand(RoverCommandType.TakePhoto, 0f));

    public void Add(RoverCommand command)
    {
        if (locked || CurrentState != State.Editing || commands.Count >= maxCommands) return;
        commands.Add(command);
        Changed?.Invoke();
    }

    /// <summary>Empties the queue. Only works before the commands are sent.</summary>
    public void Clear()
    {
        if (locked || CurrentState != State.Editing || commands.Count == 0) return;
        commands.Clear();
        Changed?.Invoke();
    }

    /// <summary>Transmits the queue to the rover. Does nothing if the queue is empty or already sent.</summary>
    public void Send()
    {
        if (locked || CurrentState != State.Editing || commands.Count == 0) return;
        if (rover == null)
        {
            Debug.LogError("CommandQueue has no rover assigned.", this);
            return;
        }
        StartCoroutine(SendRoutine());
    }

    IEnumerator SendRoutine()
    {
        CurrentState = State.Transmitting;
        TransmitProgress = 0f;
        Changed?.Invoke();

        float elapsed = 0f;
        while (elapsed < transmitDelay)
        {
            elapsed += Time.deltaTime;
            TransmitProgress = transmitDelay > 0f ? Mathf.Clamp01(elapsed / transmitDelay) : 1f;
            yield return null;
        }

        CurrentState = State.Executing;
        for (int i = 0; i < commands.Count; i++)
        {
            ExecutingIndex = i;
            Changed?.Invoke();
            yield return rover.Execute(commands[i]);
        }

        ExecutingIndex = -1;
        commands.Clear();
        CurrentState = State.Editing;
        Changed?.Invoke();
    }
}
