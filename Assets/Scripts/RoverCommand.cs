using System;

/// <summary>The kinds of command mission control can send to the rover.</summary>
public enum RoverCommandType
{
    TurnLeft,
    TurnRight,
    DriveForward,
    CameraLeft,
    CameraRight,
    CameraUp,
    CameraDown,
    TakePhoto
}

/// <summary>One queued command: what to do and how much (degrees for turns and camera moves, metres for drives).</summary>
[Serializable]
public struct RoverCommand
{
    public RoverCommandType type;
    public float amount;

    public RoverCommand(RoverCommandType type, float amount)
    {
        this.type = type;
        this.amount = amount;
    }

    /// <summary>Short text for the queue list in the console UI.</summary>
    public string Label
    {
        get
        {
            switch (type)
            {
                case RoverCommandType.TurnLeft: return "Turn left " + amount.ToString("0") + "°";
                case RoverCommandType.TurnRight: return "Turn right " + amount.ToString("0") + "°";
                case RoverCommandType.DriveForward: return "Drive forward " + amount.ToString("0.#") + " m";
                case RoverCommandType.CameraLeft: return "Camera left " + amount.ToString("0") + "°";
                case RoverCommandType.CameraRight: return "Camera right " + amount.ToString("0") + "°";
                case RoverCommandType.CameraUp: return "Camera up " + amount.ToString("0") + "°";
                case RoverCommandType.CameraDown: return "Camera down " + amount.ToString("0") + "°";
                case RoverCommandType.TakePhoto: return "Take photo";
                default: return type.ToString();
            }
        }
    }
}
