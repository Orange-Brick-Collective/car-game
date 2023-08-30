using Sandbox;

namespace cargame;

// try to use all this stuff later
public struct WheelData {
    public Vector3 Position { get; set; }
    public Angles Rotation { get; set; }

    public float WheelWidth { get; set; }
    public float WheelDiameter { get; set; }
    public float TireDiameter { get; set; }
    public float TirePSI { get; set; }

    public bool Powered { get; set; }
    public bool PoweredReverse { get; set; }
    public bool Steer { get; set; }
    public bool SteerReverse { get; set; }
};

public struct CarData {
    public string ModelPath { get; set; }
    public WheelData[] Wheels { get; set; }
    public Seat[] Seats { get; set; }
    public Engine Engine { get; set; }
};

public struct Engine {
    public float Torque { get; set; }
    public float Horsepower { get; set; }
}

public struct Seat {

};

public class Car : AnimatedEntity, IUse {
    public CarData Data;

    public bool active = false;
    public bool broken = false;

    // spawn is called before anything else, even constructor 
    // need to delay to set and use data
    public override void Spawn() => DelaySpawn();
    public async void DelaySpawn() {
        await GameTask.DelayRealtime(20);
        Predictable = true;
        SetModel(Data.ModelPath);
        SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        CreateWheels();
    }

    public void CreateWheels() {

    }

    public bool IsUsable(Entity user) {
        if (!active) return true;
        return false;
    }

    public bool OnUse(Entity user) {
        Player plr = (Player)user;
        var c = (CarController)plr.ChangeController<CarController>();
        c.Car = this;
        return true;
    }
}