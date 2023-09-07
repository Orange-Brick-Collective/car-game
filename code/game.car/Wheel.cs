using Sandbox;

namespace CarGame;

public interface IWheel {
    public Vector3 LocalPosition { get; set; }
    public Rotation LocalRotation { get; set; }
    public float WheelWidth { get; set; }
    public float WheelDiameter { get; set; }
    public float TireDiameter { get; set; }
    public float TirePSI { get; set; }
    public bool Powered { get; set; }
    public bool Steer { get; set; }
}

[Prefab]
public partial class Wheel : BaseNetworkable, IWheel {
    [Prefab, Net, Editor.PointLine] public Vector3 LocalPosition { get; set; }
    [Prefab, Net] public Rotation LocalRotation { get; set; }
    [Prefab, Net] public float WheelWidth { get; set; }
    [Prefab, Net] public float WheelDiameter { get; set; }
    [Prefab, Net] public float TireDiameter { get; set; }
    [Prefab, Net] public float TirePSI { get; set; }

    [Prefab, Net] public bool Powered { get; set; }
    [Prefab, Net] public bool Steer { get; set; }

    [Net] public bool Grounded { get; set; }
    // ! todo: grip


    public Transform GetTransform(Vector3 carPos, Rotation carRot) {
        return new Transform(carPos + (LocalPosition * carRot), carRot * LocalRotation, 1);
    }
}