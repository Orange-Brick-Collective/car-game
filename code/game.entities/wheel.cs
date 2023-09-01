using Sandbox;

namespace CarGame;

public interface IWheel {
    public float WheelWidth { get; set; }
    public float WheelDiameter { get; set; }
    public float TireDiameter { get; set; }
    public float TirePSI { get; set; }
    public bool Powered { get; set; }
    public bool Steer { get; set; }
}

[Prefab]
public partial class Wheel : AnimatedEntity, IWheel {
    public Car parentCar;

    [Prefab, Net] public float WheelWidth { get; set; }
    [Prefab, Net] public float WheelDiameter { get; set; }
    [Prefab, Net] public float TireDiameter { get; set; }
    [Prefab, Net] public float TirePSI { get; set; }

    [Prefab, Net] public bool Powered { get; set; }
    [Prefab, Net] public bool Steer { get; set; }

    public override void Spawn() => DelaySpawn();
    public async void DelaySpawn() {
        await GameTask.DelayRealtime(20);
        Vector3 width = new(0, WheelWidth * 0.5f, 0);
        Capsule capsule = new(width, -width, WheelDiameter);
        SetupPhysicsFromCylinder(PhysicsMotionType.Keyframed, capsule);
    }

    public override void Simulate(IClient cl) {
        // Position
    }

}